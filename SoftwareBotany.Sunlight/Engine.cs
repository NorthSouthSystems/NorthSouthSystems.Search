using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareBotany.Sunlight
{
    public partial class Engine<TItem, TPrimaryKey> : IEngine<TPrimaryKey>
    {
        public Engine(Func<TItem, TPrimaryKey> primaryKeyExtractor)
        {
            _primaryKeyExtractor = primaryKeyExtractor;
        }

        private bool _initializing = true;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        private readonly Func<TItem, TPrimaryKey> _primaryKeyExtractor;
        private List<TPrimaryKey> _primaryKeys = new List<TPrimaryKey>();
        private Dictionary<TPrimaryKey, int> _primaryKeyToBitPositionMap = new Dictionary<TPrimaryKey, int>();

        private readonly Vector _activeItems = new Vector(false);

        private List<CatalogPlusExtractor> _catalogsPlusExtractors = new List<CatalogPlusExtractor>();

        private class CatalogPlusExtractor
        {
            public CatalogPlusExtractor(ICatalog catalog, Func<TItem, object> keysOrKeyExtractor)
            {
                Catalog = catalog;
                KeysOrKeyExtractor = keysOrKeyExtractor;
            }

            public readonly ICatalog Catalog;
            public readonly Func<TItem, object> KeysOrKeyExtractor;
        }

        #region Catalog Management

        public ParameterFactory<TKey> CreateCatalog<TKey>(string name, Func<TItem, IEnumerable<TKey>> keysExtractor, bool isCompressed = true)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return CreateCatalogImpl<TKey>(name, false, item => (object)keysExtractor(item), isCompressed);
        }

        public ParameterFactory<TKey> CreateCatalog<TKey>(string name, Func<TItem, TKey> keyExtractor, bool isCompressed = true)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            // Intentionally box the result of the keyExtractor because it is cheaper than a dynamic resolution.
            return CreateCatalogImpl<TKey>(name, true, item => (object)keyExtractor(item), isCompressed);
        }

        private ParameterFactory<TKey> CreateCatalogImpl<TKey>(string name, bool isOneToOne, Func<TItem, object> keyOrKeysExtractor, bool isCompressed)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            Catalog<TKey> catalog;

            try
            {
                _rwLock.EnterWriteLock();

                if (!_initializing)
                    throw new ApplicationException("Cannot CreateCatalog after AddItem has been called.");

                if (_catalogsPlusExtractors.Any(cpe => cpe.Catalog.Name == name))
                    throw new ArgumentException(string.Format("Catalog already exists with name : {0}.", name), "name");

                catalog = new Catalog<TKey>(this, name, isCompressed);
                _catalogsPlusExtractors.Add(new CatalogPlusExtractor(catalog, keyOrKeysExtractor));
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }

            return new ParameterFactory<TKey>(catalog, isOneToOne);
        }

        #endregion

        #region Rebuild

        public void Rebuild()
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();

                int[] bitPositionShifts = new int[_primaryKeys.Count];
                int i = 0;
                int exclusionCount = 0;

                foreach (bool bit in _activeItems.GetBits())
                {
                    if (bit)
                        bitPositionShifts[i] = exclusionCount;
                    else
                    {
                        bitPositionShifts[i] = -1;
                        exclusionCount++;
                    }

                    i++;

                    // GetBits will return the trailing 0s on the Vector, so we must break out before
                    // we go out of bounds.
                    if (i >= bitPositionShifts.Length)
                        break;
                }

                List<Action> readActions = new List<Action>();
                readActions.Add(() => _activeItems.RebuildHotReadPhase(bitPositionShifts));
                readActions.AddRange(_catalogsPlusExtractors.Select(cpe => new Action(() => cpe.Catalog.RebuildHotReadPhase(bitPositionShifts))));

                Parallel.Invoke(readActions.ToArray());

                List<Action> writeActions = new List<Action>();
                writeActions.Add(() => RebuildPrimaryKeys(bitPositionShifts));
                writeActions.AddRange(_catalogsPlusExtractors.Select(cpe => new Action(() => cpe.Catalog.RebuildHotWritePhase())));

                _rwLock.EnterWriteLock();

                _activeItems.RebuildHotWritePhase();
                Parallel.Invoke(writeActions.ToArray());
            }
            finally
            {
                if (_rwLock.IsWriteLockHeld)
                    _rwLock.ExitWriteLock();

                if (_rwLock.IsUpgradeableReadLockHeld)
                    _rwLock.ExitUpgradeableReadLock();
            }
        }

        private void RebuildPrimaryKeys(int[] bitPositionShifts)
        {
            // PERF : Null the Dictionary instance member. In cases of local variables, I know that this behavior is unneccessary
            // because the garbage collector knows an instruction pointer for after which a given variable is no longer used.
            // For instances members, I do not know this to be the case. So, simply null the now unused parameter here to ensure that
            // it can be garbage collected if needed during this operation which will allocate a significant amount of memory.
            _primaryKeyToBitPositionMap = null;

            _primaryKeys = _primaryKeys.Where((primaryKey, bitPosition) => bitPositionShifts[bitPosition] >= 0)
                .ToList();

            _primaryKeyToBitPositionMap = _primaryKeys.Select((primaryKey, bitPosition) => new { PrimaryKey = primaryKey, BitPosition = bitPosition })
                .ToDictionary(pkbi => pkbi.PrimaryKey, pkbi => pkbi.BitPosition);
        }

        #endregion

        #region Add

        public void Add(TItem item)
        {
            try
            {
                _rwLock.EnterWriteLock();

                _initializing = false;

                AddItem(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void Add(IEnumerable<TItem> items)
        {
            try
            {
                _rwLock.EnterWriteLock();

                _initializing = false;

                foreach (TItem item in items)
                    AddItem(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        private void AddItem(TItem item)
        {
            TPrimaryKey primaryKey = _primaryKeyExtractor(item);

            if (_primaryKeyToBitPositionMap.ContainsKey(primaryKey))
                throw new ArgumentException(string.Format("An item already exists in this Engine with the PrimaryKey : {0}.", primaryKey));

            int bitPosition = _primaryKeys.Count;
            _primaryKeys.Add(primaryKey);
            _primaryKeyToBitPositionMap.Add(primaryKey, bitPosition);
            _activeItems[bitPosition] = true;

            foreach (CatalogPlusExtractor cpe in _catalogsPlusExtractors)
            {
                // Key must use dynamic cast here or else the dynamic resolution on Catalog.Set will look for a signature
                // matching Set(object, int, bool) instead of the true runtime type of the key.
                dynamic key = cpe.KeysOrKeyExtractor(item);
                dynamic catalog = cpe.Catalog;
                catalog.Set(key, bitPosition, true);
            }
        }

        #endregion

        #region Update

        public void Update(TItem item)
        {
            try
            {
                _rwLock.EnterWriteLock();

                UpdateItem(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void Update(IEnumerable<TItem> items)
        {
            try
            {
                _rwLock.EnterWriteLock();

                foreach (TItem item in items)
                    UpdateItem(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        private void UpdateItem(TItem item)
        {
            TPrimaryKey primaryKey = _primaryKeyExtractor(item);

            if (!_primaryKeyToBitPositionMap.ContainsKey(primaryKey))
                throw new ArgumentException(string.Format("No item exists in this Engine with the PrimaryKey : {0}.", primaryKey));

            int fromBitPosition = _primaryKeyToBitPositionMap[primaryKey];
            _activeItems[fromBitPosition] = false;

            int toBitPosition = _primaryKeys.Count;
            _primaryKeys.Add(primaryKey);
            _primaryKeyToBitPositionMap[primaryKey] = toBitPosition;
            _activeItems[toBitPosition] = true;

            foreach (CatalogPlusExtractor cpe in _catalogsPlusExtractors)
            {
                // Key must use dynamic cast here or else the dynamic resolution on Catalog.Set will look for a signature
                // matching Set(object, int, bool) instead of the true runtime type of the key.
                dynamic key = cpe.KeysOrKeyExtractor(item);
                dynamic catalog = cpe.Catalog;
                catalog.Set(key, toBitPosition, true);
            }
        }

        #endregion

        #region Remove

        public void Remove(TItem item)
        {
            TPrimaryKey primaryKey = _primaryKeyExtractor(item);

            try
            {
                _rwLock.EnterWriteLock();

                if (!_primaryKeyToBitPositionMap.ContainsKey(primaryKey))
                    throw new ArgumentException(string.Format("No item exists in this Engine with the PrimaryKey : {0}.", primaryKey));

                _activeItems[_primaryKeyToBitPositionMap[primaryKey]] = false;
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        #endregion

        #region Search

        public Search<TPrimaryKey> CreateSearch() { return new Search<TPrimaryKey>(this); }

        TPrimaryKey[] IEngine<TPrimaryKey>.Search(Search<TPrimaryKey> search, int skip, int take, out int totalCount)
        {
            try
            {
                _rwLock.EnterReadLock();

                Vector result = InitializeSearch(search.AmongstPrimaryKeys);
                SearchCatalogs(result, search.SearchParameters);
                totalCount = result.Population;

                foreach (IProjectionParameter projectionParameter in search.ProjectionParameters)
                    projectionParameter.DynamicProjections = ((dynamic)projectionParameter.Catalog).Projection(result);

                IEnumerable<int> sortedBitPositions = (!search.SortParameters.Any() && !search.SortPrimaryKeyAscending.HasValue)
                    ? result.GetBitPositions(true)
                    : SortBitPositions(result, search.SortParameters.ToArray(), search.SortPrimaryKeyAscending);

                // Distinct is required because of Catalogs created from multi-key columns: e.g. think post/blog tags
                return sortedBitPositions.Distinct()
                    .Select(bitPosition => _primaryKeys[bitPosition])
                    .Skip(skip)
                    .Take(take)
                    .ToArray();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        private Vector InitializeSearch(IEnumerable<TPrimaryKey> amongstPrimaryKeys)
        {
            Vector result = new Vector(_activeItems);

            if (amongstPrimaryKeys.Any())
            {
                Vector amongstPrimaryKeyMask = new Vector(true);

                foreach (int bitPosition in amongstPrimaryKeys
                    .Select(primaryKey =>
                    {
                        int temp;

                        if (_primaryKeyToBitPositionMap.TryGetValue(primaryKey, out temp))
                            return temp;
                        else
                            return -1;
                    })
                    .Where(position => position >= 0)
                    .OrderBy(position => position))
                {
                    amongstPrimaryKeyMask[bitPosition] = true;
                }

                result.And(amongstPrimaryKeyMask);
            }

            return result;
        }

        private void SearchCatalogs(Vector result, IEnumerable<ISearchParameter> searchParameters)
        {
            foreach (ISearchParameter searchParameter in searchParameters)
            {
                dynamic catalog = searchParameter.Catalog;

                switch (searchParameter.ParameterType)
                {
                    case SearchParameterType.Exact:
                        catalog.Search(result, searchParameter.DynamicExact);
                        break;
                    case SearchParameterType.Enumerable:
                        catalog.Search(result, searchParameter.DynamicEnumerable);
                        break;
                    case SearchParameterType.Range:
                        catalog.Search(result, searchParameter.DynamicRangeMin, searchParameter.DynamicRangeMax);
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Unrecognized SearchParameterType : {0}.", searchParameter.ParameterType));
                }
            }
        }

        #endregion

        #region Sort

        private IEnumerable<int> SortBitPositions(Vector result, ISortParameter[] sortParameters, bool? sortPrimaryKeyAscending)
        {
            if (sortParameters.Any())
            {
                IEnumerable<IEnumerable<int>> partialResults = null;

                partialResults = SortBitPositionsByParameter(result, sortParameters.First());

                for (int i = 1; i < sortParameters.Length; i++)
                    partialResults = SortBitPositionsThenByParameter(partialResults, sortParameters[i]);

                if (sortPrimaryKeyAscending.HasValue)
                    return SortBitPositionsThenByPrimaryKey(partialResults, sortPrimaryKeyAscending.Value);
                else
                    return partialResults.SelectMany(partialResult => partialResult);
            }
            else
                return SortBitPositionsByPrimaryKey(result.GetBitPositions(true), sortPrimaryKeyAscending.Value);
        }

        private IEnumerable<IEnumerable<int>> SortBitPositionsByParameter(Vector result, ISortParameter sortParameter)
        {
            dynamic catalog = sortParameter.Catalog;
            IEnumerable<KeyValuePair<dynamic, IEnumerable<int>>> sorted;

            switch (sortParameter.ParameterType)
            {
                case SortParameterType.Directional:
                    sorted = catalog.SortBitPositionsDynamic(result, true, sortParameter.Ascending);
                    break;
                default:
                    throw new NotImplementedException(string.Format("Unrecognized SortParameterType : {0}.", sortParameter.ParameterType));
            }

            return sorted.Select(kvp => kvp.Value);
        }

        private IEnumerable<IEnumerable<int>> SortBitPositionsThenByParameter(IEnumerable<IEnumerable<int>> partialResults, ISortParameter sortParameter)
        {
            return partialResults.SelectMany(partialResult =>
            {
                // AndFilterBitPositions does not support Compressed Vectors. Implementing that feature could
                // offer great performance gains here.
                Vector partialResultVector = new Vector(false);

                foreach (int bitPosition in partialResult)
                    partialResultVector[bitPosition] = true;

                return SortBitPositionsByParameter(partialResultVector, sortParameter);
            });
        }

        private IEnumerable<int> SortBitPositionsByPrimaryKey(IEnumerable<int> result, bool ascending)
        {
            var query = result.Select(bitPosition => new { BitPosition = bitPosition, PrimaryKey = _primaryKeys[bitPosition] });
            query = ascending ? query.OrderBy(x => x.PrimaryKey) : query.OrderByDescending(x => x.PrimaryKey);
            return query.Select(x => x.BitPosition);
        }

        private IEnumerable<int> SortBitPositionsThenByPrimaryKey(IEnumerable<IEnumerable<int>> partialResults, bool ascending)
        {
            return partialResults.SelectMany(partialResult => SortBitPositionsByPrimaryKey(partialResult, ascending));
        }

        #endregion
    }

    internal interface IEngine<TPrimaryKey> : IEngine
    {
        TPrimaryKey[] Search(Search<TPrimaryKey> search, int skip, int take, out int totalCount);
    }

    internal interface IEngine { }
}
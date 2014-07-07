using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareBotany.Sunlight
{
    public sealed partial class Engine<TItem, TPrimaryKey> : IEngine<TPrimaryKey>, IDisposable
    {
        public Engine(bool allowUnsafe, Func<TItem, TPrimaryKey> primaryKeyExtractor)
        {
            _allowUnsafe = allowUnsafe;
            _primaryKeyExtractor = primaryKeyExtractor;
            _activeItems = new Vector(_allowUnsafe, VectorCompression.None);
        }

        private bool _configuring = true;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        public bool AllowUnsafe { get { return _allowUnsafe; } }
        private readonly bool _allowUnsafe;

        private readonly Func<TItem, TPrimaryKey> _primaryKeyExtractor;
        private List<TPrimaryKey> _primaryKeys = new List<TPrimaryKey>();
        private Dictionary<TPrimaryKey, int> _primaryKeyToActiveBitPositionMap = new Dictionary<TPrimaryKey, int>();

        private Vector _activeItems;

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

        public void Dispose() { _rwLock.Dispose(); }

        #region Catalog Management

        public CatalogHandle<TKey> CreateCatalog<TKey>(string name, VectorCompression compression, Func<TItem, TKey> keyExtractor)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return CreateCatalogImpl<TKey>(name, compression, true, item => (object)keyExtractor(item));
        }

        public CatalogHandle<TKey> CreateCatalog<TKey>(string name, VectorCompression compression, Func<TItem, IEnumerable<TKey>> keysExtractor)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return CreateCatalogImpl<TKey>(name, compression, false, item => (object)keysExtractor(item));
        }

        private CatalogHandle<TKey> CreateCatalogImpl<TKey>(string name, VectorCompression compression, bool isOneToOne, Func<TItem, object> keyOrKeysExtractor)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            Catalog<TKey> catalog;

            try
            {
                _rwLock.EnterWriteLock();

                if (!_configuring)
                    throw new NotSupportedException("Cannot create a Catalog in an Engine that has already called Add or CreateQuery.");

                if (_catalogsPlusExtractors.Any(cpe => cpe.Catalog.Name == name))
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "A Catalog already exists with the name : {0}.", name));

                catalog = new Catalog<TKey>(name, _allowUnsafe, compression);
                _catalogsPlusExtractors.Add(new CatalogPlusExtractor(catalog, keyOrKeysExtractor));
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }

            return new CatalogHandle<TKey>(catalog, isOneToOne);
        }

        // NOTE : No locking is necessary here because this is only called from the Query class, and in order to CreateQuery,
        // _configuring is stopped which prevents the addition of Catalogs.
        bool IEngine<TPrimaryKey>.HasCatalog(ICatalog catalog) { return _catalogsPlusExtractors.Any(cpe => cpe.Catalog == catalog); }

        #endregion

        #region Optimize

        public void Optimize()
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();

                int[] bitPositionShifts = new int[_primaryKeys.Count];
                int index = 0;
                int exclusionCounter = 0;

                foreach (bool bit in _activeItems.Bits)
                {
                    if (bit)
                    {
                        bitPositionShifts[index] = exclusionCounter;
                    }
                    else
                    {
                        bitPositionShifts[index] = -1;
                        exclusionCounter++;
                    }

                    index++;

                    // GetBits will return the trailing 0s on the Vector, so we must break out before
                    // we go out of bounds.
                    if (index >= bitPositionShifts.Length)
                        break;
                }

                // A potential optimization would be to construct this Vector in an efficient manner able to leverage
                // the fact that we need exactly _activeItems.Population consecutive 1's.
                Vector optimizedActiveItems = null;

                List<Action> readActions = new List<Action>();
                readActions.Add(() => _activeItems.OptimizeReadPhase(bitPositionShifts, out optimizedActiveItems));
                readActions.AddRange(_catalogsPlusExtractors.Select(cpe => new Action(() => cpe.Catalog.OptimizeReadPhase(bitPositionShifts))));

                Parallel.Invoke(readActions.ToArray());

                List<Action> writeActions = new List<Action>();
                writeActions.Add(() => OptimizePrimaryKeys(bitPositionShifts));
                writeActions.AddRange(_catalogsPlusExtractors.Select(cpe => new Action(() => cpe.Catalog.OptimizeWritePhase())));

                _rwLock.EnterWriteLock();

                _activeItems = optimizedActiveItems;
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

        private void OptimizePrimaryKeys(int[] bitPositionShifts)
        {
            // PERF : Null the Dictionary instance member. In cases of local variables, I know that this behavior is unneccessary
            // because the garbage collector knows an instruction pointer for after which a given variable is no longer used.
            // For instances members, I do not know this to be the case. So, simply null the now unused parameter here to ensure that
            // it can be garbage collected if needed during this operation which will allocate a significant amount of memory.
            _primaryKeyToActiveBitPositionMap = null;

            _primaryKeys = _primaryKeys.Where((primaryKey, bitPosition) => bitPositionShifts[bitPosition] >= 0)
                .ToList();

            _primaryKeyToActiveBitPositionMap = _primaryKeys.Select((primaryKey, bitPosition) => new { PrimaryKey = primaryKey, BitPosition = bitPosition })
                .ToDictionary(pkbi => pkbi.PrimaryKey, pkbi => pkbi.BitPosition);
        }

        #endregion

        #region Add

        public void Add(TItem item)
        {
            try
            {
                _rwLock.EnterWriteLock();

                _configuring = false;

                AddItem(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void Add(IEnumerable<TItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            Contract.EndContractBlock();

            try
            {
                _rwLock.EnterWriteLock();

                _configuring = false;

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

            if (_primaryKeyToActiveBitPositionMap.ContainsKey(primaryKey))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "An item already exists in this Engine with the primary key : {0}.", primaryKey));

            int bitPosition = _primaryKeys.Count;
            _primaryKeys.Add(primaryKey);
            _primaryKeyToActiveBitPositionMap.Add(primaryKey, bitPosition);
            _activeItems[bitPosition] = true;

            foreach (CatalogPlusExtractor cpe in _catalogsPlusExtractors)
                cpe.Catalog.Set(cpe.KeysOrKeyExtractor(item), bitPosition, true);
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
            if (items == null)
                throw new ArgumentNullException("items");

            Contract.EndContractBlock();

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

            if (!_primaryKeyToActiveBitPositionMap.ContainsKey(primaryKey))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No item exists in this Engine with the primary key : {0}.", primaryKey));

            int fromBitPosition = _primaryKeyToActiveBitPositionMap[primaryKey];
            _activeItems[fromBitPosition] = false;

            int toBitPosition = _primaryKeys.Count;
            _primaryKeys.Add(primaryKey);
            _primaryKeyToActiveBitPositionMap[primaryKey] = toBitPosition;
            _activeItems[toBitPosition] = true;

            foreach (CatalogPlusExtractor cpe in _catalogsPlusExtractors)
                cpe.Catalog.Set(cpe.KeysOrKeyExtractor(item), toBitPosition, true);
        }

        #endregion

        #region Remove

        public void Remove(TItem item)
        {
            try
            {
                _rwLock.EnterWriteLock();

                RemoveItem(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void Remove(IEnumerable<TItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            Contract.EndContractBlock();

            try
            {
                _rwLock.EnterWriteLock();

                foreach (TItem item in items)
                    RemoveItem(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        private void RemoveItem(TItem item)
        {
            TPrimaryKey primaryKey = _primaryKeyExtractor(item);

            if (!_primaryKeyToActiveBitPositionMap.ContainsKey(primaryKey))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No item exists in this Engine with the primary key : {0}.", primaryKey));

            int bitPosition = _primaryKeyToActiveBitPositionMap[primaryKey];
            _primaryKeyToActiveBitPositionMap.Remove(primaryKey);
            _activeItems[bitPosition] = false;
        }

        #endregion

        #region Query

        public Query<TPrimaryKey> CreateQuery()
        {
            if (_configuring)
            {
                try
                {
                    _rwLock.EnterWriteLock();
                    _configuring = false;
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }

            return new Query<TPrimaryKey>(this);
        }

        TPrimaryKey[] IEngine<TPrimaryKey>.ExecuteQuery(Query<TPrimaryKey> query, int skip, int take, out int totalCount)
        {
            try
            {
                _rwLock.EnterReadLock();

                Vector result = InitializeResult(query.AmongstPrimaryKeys);
                FilterCatalogs(result, query.FilterParameters);
                totalCount = result.Population;

                Parallel.ForEach(query.FacetParameters, new ParallelOptions { MaxDegreeOfParallelism = query.FacetDisableParallel ? 1 : -1 },
                    facetParameter => facetParameter.Facet = facetParameter.Catalog.Facet(result, query.FacetDisableParallel, query.FacetShortCircuitCounting));

                IEnumerable<int> sortedBitPositions = (!query.SortParameters.Any() && !query.SortPrimaryKeyAscending.HasValue)
                    ? result.GetBitPositions(true)
                    : SortBitPositions(result, query.SortParameters.ToArray(), query.SortPrimaryKeyAscending);

                // Distinct is required because of Catalogs created from multi-key columns: e.g. think post/blog tags
                return sortedBitPositions.Distinct()
                    .Skip(skip)
                    .Take(take)
                    .Select(bitPosition => _primaryKeys[bitPosition])
                    .ToArray();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        private Vector InitializeResult(IEnumerable<TPrimaryKey> amongstPrimaryKeys)
        {
            Vector result;

            if (amongstPrimaryKeys.Any())
            {
                result = new Vector(_allowUnsafe, VectorCompression.None);

                // No benchmarking was done to justify the OrderByDescending; however, the rationale
                // is that if we start by setting the maximum position, the Vector's underlying Array
                // will only have to undergo a single resizing. Obviously it comes at the price of a
                // QuickSort O(n log n).  However, we are already in an n sized loop so resizing  which
                // costs n each time could theoretically cost us O(n^2).
                foreach (int bitPosition in amongstPrimaryKeys
                    .Select(primaryKey =>
                    {
                        int temp;

                        if (_primaryKeyToActiveBitPositionMap.TryGetValue(primaryKey, out temp))
                            return temp;
                        else
                            return -1;
                    })
                    .Where(position => position >= 0)
                    .OrderByDescending(position => position))
                {
                    result[bitPosition] = true;
                }
            }
            else
                result = new Vector(_allowUnsafe, VectorCompression.None, _activeItems);

            return result;
        }

        private static void FilterCatalogs(Vector result, IEnumerable<IFilterParameterInternal> filterParameters)
        {
            foreach (IFilterParameterInternal filterParameter in filterParameters)
            {
                switch (filterParameter.ParameterType)
                {
                    case FilterParameterType.Exact:
                        filterParameter.Catalog.FilterExact(result, filterParameter.Exact);
                        break;
                    case FilterParameterType.Enumerable:
                        filterParameter.Catalog.FilterEnumerable(result, filterParameter.Enumerable);
                        break;
                    case FilterParameterType.Range:
                        filterParameter.Catalog.FilterRange(result, filterParameter.RangeMin, filterParameter.RangeMax);
                        break;
                    default:
                        throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Unrecognized filter parameter type : {0}.", filterParameter.ParameterType));
                }
            }
        }

        #endregion

        #region Sort

        private IEnumerable<int> SortBitPositions(Vector result, ISortParameterInternal[] sortParameters, bool? sortPrimaryKeyAscending)
        {
            if (sortParameters.Any())
            {
                IEnumerable<IEnumerable<int>> partialResults = null;

                var firstSortParameter = sortParameters.First();
                partialResults = firstSortParameter.Catalog.SortBitPositions(result, true, firstSortParameter.Ascending).PartialSortResultsBitPositions;

                for (int i = 1; i < sortParameters.Length; i++)
                    partialResults = SortBitPositionsThenByParameter(_allowUnsafe, partialResults, sortParameters[i]);

                if (sortPrimaryKeyAscending.HasValue)
                    return SortBitPositionsThenByPrimaryKey(partialResults, sortPrimaryKeyAscending.Value);
                else
                    return partialResults.SelectMany(partialResult => partialResult);
            }
            else
                return SortBitPositionsByPrimaryKey(result.GetBitPositions(true), sortPrimaryKeyAscending.Value);
        }

        private static IEnumerable<IEnumerable<int>> SortBitPositionsThenByParameter(bool allowUnsafe, IEnumerable<IEnumerable<int>> partialResults, ISortParameterInternal sortParameter)
        {
            return partialResults.SelectMany(partialResult =>
            {
                // AndFilterBitPositions does not support Compressed Vectors. Implementing that feature could
                // possible offer significant performance gains here.
                Vector partialResultVector = new Vector(allowUnsafe, VectorCompression.None);

                foreach (int bitPosition in partialResult)
                    partialResultVector[bitPosition] = true;

                return sortParameter.Catalog.SortBitPositions(partialResultVector, true, sortParameter.Ascending).PartialSortResultsBitPositions;
            });
        }

        private IEnumerable<int> SortBitPositionsByPrimaryKey(IEnumerable<int> result, bool ascending)
        {
            return ascending
                ? result.OrderBy(bitPosition => _primaryKeys[bitPosition])
                : result.OrderByDescending(bitPosition => _primaryKeys[bitPosition]);
        }

        private IEnumerable<int> SortBitPositionsThenByPrimaryKey(IEnumerable<IEnumerable<int>> partialResults, bool ascending)
        {
            return partialResults.SelectMany(partialResult => SortBitPositionsByPrimaryKey(partialResult, ascending));
        }

        #endregion
    }
}
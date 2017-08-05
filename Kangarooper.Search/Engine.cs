namespace Kangarooper.Search
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed partial class Engine<TItem, TPrimaryKey> : IDisposable
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
            internal CatalogPlusExtractor(ICatalogInEngine catalog, Func<TItem, object> keysOrKeyExtractor)
            {
                Catalog = catalog;
                KeysOrKeyExtractor = keysOrKeyExtractor;
            }

            internal readonly ICatalogInEngine Catalog;
            internal readonly Func<TItem, object> KeysOrKeyExtractor;
        }

        public void Dispose() { _rwLock.Dispose(); }

        #region Catalog Management

        public ICatalogHandle<TKey> CreateCatalog<TKey>(string name, VectorCompression compression, Func<TItem, TKey> keyExtractor)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return CreateCatalogImpl<TKey>(name, compression, true, item => (object)keyExtractor(item));
        }

        public ICatalogHandle<TKey> CreateCatalog<TKey>(string name, VectorCompression compression, Func<TItem, IEnumerable<TKey>> keysExtractor)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return CreateCatalogImpl<TKey>(name, compression, false, item => (object)keysExtractor(item));
        }

        private ICatalogHandle<TKey> CreateCatalogImpl<TKey>(string name, VectorCompression compression, bool isOneToOne, Func<TItem, object> keyOrKeysExtractor)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Contract.EndContractBlock();

            Catalog<TKey> catalog;

            try
            {
                _rwLock.EnterWriteLock();

                if (!_configuring)
                    throw new NotSupportedException("Cannot create a Catalog in an Engine that has already called Add or CreateQuery.");

                if (_catalogsPlusExtractors.Any(cpe => cpe.Catalog.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "A Catalog already exists with the case-insensitive name : {0}.", name));

                catalog = new Catalog<TKey>(name, isOneToOne, _allowUnsafe, compression);
                _catalogsPlusExtractors.Add(new CatalogPlusExtractor(catalog, keyOrKeysExtractor));
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }

            return catalog;
        }

        // NOTE : No locking is necessary for the following Catalog methods because they are only called from the Query class, and in order to CreateQuery,
        // _configuring is stopped which prevents the addition of Catalogs.
        internal bool HasCatalog(ICatalogHandle catalog) { return _catalogsPlusExtractors.Any(cpe => cpe.Catalog == catalog); }

        internal IEnumerable<ICatalogInEngine> GetCatalogs() { return _catalogsPlusExtractors.Select(cpe => cpe.Catalog); }

        internal ICatalogInEngine GetCatalog(string name)
        {
            return _catalogsPlusExtractors.Single(cpe => cpe.Catalog.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Catalog;
        }

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

        public Query<TItem, TPrimaryKey> CreateQuery()
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

            return new Query<TItem, TPrimaryKey>(this);
        }

        internal TPrimaryKey[] ExecuteQuery(Query<TItem, TPrimaryKey> query, int skip, int take, out int totalCount)
        {
            try
            {
                _rwLock.EnterReadLock();

                Vector result = InitializeResult(query.AmongstPrimaryKeys);

                Filter(query, result);
                totalCount = result.Population;

                Facet(query, result);

                // Distinct is required because of Catalogs created from multi-key columns: e.g. think post/blog tags
                return Sort(query, skip + take, result, totalCount)
                    .Distinct()
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

        private static void Filter(Query<TItem, TPrimaryKey> query, Vector result)
        {
            // TODO : Support for nested boolean logic.
            Trace.Assert(query.FilterClause == null || query.FilterClause.Operation == BooleanOperation.And);
            Trace.Assert(query.FilterClause == null || query.FilterClause.SubClauses.All(clause => clause is IFilterParameter));
            var filterParameters = query.FilterClause == null ? Enumerable.Empty<IFilterParameter>() : query.FilterClause.SubClauses.Cast<IFilterParameter>();

            foreach (IFilterParameter filterParameter in filterParameters)
            {
                var catalog = (ICatalogInEngine)filterParameter.Catalog;

                switch (filterParameter.ParameterType)
                {
                    case FilterParameterType.Exact:
                        catalog.FilterExact(result, filterParameter.Exact);
                        break;
                    case FilterParameterType.Enumerable:
                        catalog.FilterEnumerable(result, filterParameter.Enumerable);
                        break;
                    case FilterParameterType.Range:
                        catalog.FilterRange(result, filterParameter.RangeMin, filterParameter.RangeMax);
                        break;
                    default:
                        throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Unrecognized filter parameter type : {0}.", filterParameter.ParameterType));
                }
            }
        }

        private void Facet(Query<TItem, TPrimaryKey> query, Vector filterResult)
        {
            Parallel.ForEach(query.FacetParametersInternal, new ParallelOptions { MaxDegreeOfParallelism = query.FacetDisableParallel ? 1 : -1 },
                facetParameter => facetParameter.Facet = ((ICatalogInEngine)facetParameter.Catalog).Facet(filterResult, query.FacetDisableParallel, query.FacetShortCircuitCounting));
        }

        private IEnumerable<int> Sort(Query<TItem, TPrimaryKey> query, int skipPlusTake, Vector filterResult, int totalCount)
        {
            var sortParameters = query.SortParameters.ToArray();
            int sortCount = sortParameters.Length + (query.SortPrimaryKeyAscending.HasValue ? 1 : 0);

            if (sortParameters.Any())
            {
                CatalogSortResult sortResult = null;

                for (int sortParameterIndex = 0; sortParameterIndex < sortParameters.Length; sortParameterIndex++)
                {
                    // Disable parallel sorting if the user has specified to do so or if this is the last sort and we aren't going to have to iterate the entire result.
                    var sortParameter = sortParameters[sortParameterIndex];
                    bool sortDisableParallel = query.SortDisableParallel || (sortCount == (sortParameterIndex + 1) && skipPlusTake < totalCount);

                    if (sortParameterIndex == 0)
                        sortResult = ((ICatalogInEngine)sortParameter.Catalog).Sort(filterResult, true, sortParameter.Ascending, sortDisableParallel);
                    else
                        sortResult = ((ICatalogInEngine)sortParameter.Catalog).ThenSort(sortResult, true, sortParameter.Ascending, sortDisableParallel);
                }

                if (query.SortPrimaryKeyAscending.HasValue)
                    return sortResult.PartialSorts.SelectMany(partialSort => SortBitPositionsByPrimaryKey(partialSort.GetBitPositions(true), query.SortPrimaryKeyAscending.Value));
                else
                    return sortResult.PartialSorts.SelectMany(partialSort => partialSort.GetBitPositions(true));
            }
            else if (query.SortPrimaryKeyAscending.HasValue)
                return SortBitPositionsByPrimaryKey(filterResult.GetBitPositions(true), query.SortPrimaryKeyAscending.Value);
            else
                return filterResult.GetBitPositions(true);
        }

        private IEnumerable<int> SortBitPositionsByPrimaryKey(IEnumerable<int> bitPositions, bool ascending)
        {
            return ascending
                ? bitPositions.OrderBy(bitPosition => _primaryKeys[bitPosition])
                : bitPositions.OrderByDescending(bitPosition => _primaryKeys[bitPosition]);
        }

        #endregion
    }
}
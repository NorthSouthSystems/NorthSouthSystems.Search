using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace SoftwareBotany.Sunlight
{
    public sealed class Query<TPrimaryKey>
    {
        internal Query(IEngine<TPrimaryKey> engine)
        {
            _engine = engine;
        }

        private readonly IEngine<TPrimaryKey> _engine;
        private int _executed = 0;

        public TimeSpan? ExecuteElapsedTime { get; private set; }

        private void ThrowEngineMismatchException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (!_engine.HasCatalog(factory.Catalog))
                throw new ArgumentException("factory belongs to a different Engine.", "factory");
        }

        #region Amongst

        internal IEnumerable<TPrimaryKey> AmongstPrimaryKeys { get { return _amongstPrimaryKeys; } }
        private HashSet<TPrimaryKey> _amongstPrimaryKeys = new HashSet<TPrimaryKey>();

        public void AddAmongstPrimaryKeys(IEnumerable<TPrimaryKey> primaryKeys)
        {
            if (primaryKeys == null)
                throw new ArgumentNullException("primaryKeys");

            Contract.EndContractBlock();

            _amongstPrimaryKeys.UnionWith(primaryKeys);
        }

        #endregion

        #region Filter

        internal IEnumerable<IFilterParameterInternal> FilterParameters { get { return _filterParameters; } }
        private readonly List<IFilterParameterInternal> _filterParameters = new List<IFilterParameterInternal>();

        public FilterParameter<TKey> AddFilterExactParameter<TKey>(ParameterFactory<TKey> factory, TKey exact)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowDuplicateFilterException(factory);

            var filterParameter = factory.CreateFilterExactParameter(exact);
            _filterParameters.Add(filterParameter);

            return filterParameter;
        }

        public FilterParameter<TKey> AddFilterEnumerableParameter<TKey>(ParameterFactory<TKey> factory, IEnumerable<TKey> enumerable)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowDuplicateFilterException(factory);

            var filterParameter = factory.CreateFilterEnumerableParameter(enumerable);
            _filterParameters.Add(filterParameter);

            return filterParameter;
        }

        public FilterParameter<TKey> AddFilterRangeParameter<TKey>(ParameterFactory<TKey> factory, TKey rangeMin, TKey rangeMax)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowDuplicateFilterException(factory);

            var filterParameter = factory.CreateFilterRangeParameter(rangeMin, rangeMax);
            _filterParameters.Add(filterParameter);

            return filterParameter;
        }

        private void ThrowDuplicateFilterException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory.IsCatalogOneToOne && _filterParameters.Any(parameter => parameter.Catalog == factory.Catalog))
                throw new NotSupportedException("Can only add 1 Filter Parameter per one-to-one Catalog.");
        }

        #endregion

        #region Sort

        internal IEnumerable<ISortParameterInternal> SortParameters { get { return _sortParameters; } }
        private readonly List<ISortParameterInternal> _sortParameters = new List<ISortParameterInternal>();

        public SortParameter<TKey> AddSortParameter<TKey>(ParameterFactory<TKey> factory, bool ascending)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowPrimaryKeySortExistsException();
            ThrowDuplicateSortException(factory);

            var sortParameter = factory.CreateSortParameter(ascending);
            _sortParameters.Add(sortParameter);

            return sortParameter;
        }

        private bool? _sortPrimaryKeyAscending;

        public bool? SortPrimaryKeyAscending
        {
            get { return _sortPrimaryKeyAscending; }
            set
            {
                ThrowPrimaryKeySortExistsException();

                _sortPrimaryKeyAscending = value;
            }
        }

        private void ThrowPrimaryKeySortExistsException()
        {
            if (SortPrimaryKeyAscending.HasValue)
                throw new NotSupportedException("Cannot modify Sort Parameters after a Sort Primary Key has been set.");
        }

        private void ThrowDuplicateSortException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (_sortParameters.Any(parameter => parameter.Catalog == factory.Catalog))
                throw new NotSupportedException("Can only add 1 Sort Parameter per Catalog.");
        }

        #endregion

        #region Facet

        public bool FacetDisableParallel { get; set; }
        public bool FacetShortCircuitCounting { get; set; }

        internal IEnumerable<IFacetParameterInternal> FacetParameters { get { return _facetParameters; } }
        private readonly List<IFacetParameterInternal> _facetParameters = new List<IFacetParameterInternal>();

        public FacetParameter<TKey> AddFacetParameter<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowDuplicateFacetException(factory);

            FacetParameter<TKey> facetParameter = factory.CreateFacetParameter();
            _facetParameters.Add(facetParameter);

            return facetParameter;
        }

        private void ThrowDuplicateFacetException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (_facetParameters.Any(parameter => parameter.Catalog == factory.Catalog))
                throw new NotSupportedException("Can only add 1 Facet Parameter per Catalog.");
        }

        #endregion

        public TPrimaryKey[] Execute(int skip, int take, out int totalCount)
        {
            if (Interlocked.CompareExchange(ref _executed, 1, 0) > 0)
                throw new NotSupportedException("Query already executed.");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            TPrimaryKey[] results = _engine.ExecuteQuery(this, skip, take, out totalCount);

            watch.Stop();
            ExecuteElapsedTime = watch.Elapsed;

            return results;
        }
    }
}
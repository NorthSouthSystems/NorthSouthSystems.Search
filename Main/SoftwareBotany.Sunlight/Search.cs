using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace SoftwareBotany.Sunlight
{
    public sealed class Search<TPrimaryKey>
    {
        internal Search(IEngine<TPrimaryKey> engine)
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

        #region Search

        internal IEnumerable<ISearchParameter> SearchParameters { get { return _searchParameters; } }
        private readonly List<ISearchParameter> _searchParameters = new List<ISearchParameter>();

        public SearchParameter<TKey> AddSearchExactParameter<TKey>(ParameterFactory<TKey> factory, TKey exact)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowDuplicateSearchException(factory);

            var searchParameter = factory.CreateSearchExactParameter(exact);
            _searchParameters.Add(searchParameter);

            return searchParameter;
        }

        public SearchParameter<TKey> AddSearchEnumerableParameter<TKey>(ParameterFactory<TKey> factory, IEnumerable<TKey> enumerable)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowDuplicateSearchException(factory);

            var searchParameter = factory.CreateSearchEnumerableParameter(enumerable);
            _searchParameters.Add(searchParameter);

            return searchParameter;
        }

        public SearchParameter<TKey> AddSearchRangeParameter<TKey>(ParameterFactory<TKey> factory, TKey rangeMin, TKey rangeMax)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowDuplicateSearchException(factory);

            var searchParameter = factory.CreateSearchRangeParameter(rangeMin, rangeMax);
            _searchParameters.Add(searchParameter);

            return searchParameter;
        }

        private void ThrowDuplicateSearchException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory.IsCatalogOneToOne && _searchParameters.Any(parameter => parameter.Catalog == factory.Catalog))
                throw new NotSupportedException("Can only add 1 Search Parameter per one-to-one Catalog.");
        }

        #endregion

        #region Sort

        internal IEnumerable<ISortParameter> SortParameters { get { return _sortParameters; } }
        private readonly List<ISortParameter> _sortParameters = new List<ISortParameter>();

        public SortParameter<TKey> AddSortDirectionalParameter<TKey>(ParameterFactory<TKey> factory, bool ascending)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowPrimaryKeySortExistsException();
            ThrowDuplicateSortException(factory);

            var sortParameter = factory.CreateSortDirectionalParameter(ascending);
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

        internal IEnumerable<IFacetParameter> FacetParameters { get { return _facetParameters; } }
        private readonly List<IFacetParameter> _facetParameters = new List<IFacetParameter>();

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

        #region FacetAny

        internal IEnumerable<IFacetAnyParameter> FacetAnyParameters { get { return _facetAnyParameters; } }
        private readonly List<IFacetAnyParameter> _facetAnyParameters = new List<IFacetAnyParameter>();

        public FacetAnyParameter<TKey> AddFacetAnyParameter<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            Contract.EndContractBlock();

            ThrowEngineMismatchException(factory);
            ThrowDuplicateFacetAnyException(factory);

            FacetAnyParameter<TKey> facetAnyParameter = factory.CreateFacetAnyParameter();
            _facetAnyParameters.Add(facetAnyParameter);

            return facetAnyParameter;
        }

        private void ThrowDuplicateFacetAnyException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (_facetAnyParameters.Any(parameter => parameter.Catalog == factory.Catalog))
                throw new NotSupportedException("Can only add 1 Facet Any Parameter per Catalog.");
        }

        #endregion

        public TPrimaryKey[] Execute(int skip, int take, out int totalCount)
        {
            if (Interlocked.CompareExchange(ref _executed, 1, 0) > 0)
                throw new NotSupportedException("Search already executed.");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            TPrimaryKey[] results = _engine.Search(this, skip, take, out totalCount);

            watch.Stop();
            ExecuteElapsedTime = watch.Elapsed;

            return results;
        }
    }
}
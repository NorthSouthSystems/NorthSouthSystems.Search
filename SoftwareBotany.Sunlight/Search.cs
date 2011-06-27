using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SoftwareBotany.Sunlight
{
    public class Search<TPrimaryKey>
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
            if (_engine != factory.Catalog.Engine)
                throw new ApplicationException("ParameterFactory belongs to a different Engine.");
        }

        #region Amongst

        internal IEnumerable<TPrimaryKey> AmongstPrimaryKeys { get { return _amongstPrimaryKeys; } }
        private HashSet<TPrimaryKey> _amongstPrimaryKeys = new HashSet<TPrimaryKey>();

        public Search<TPrimaryKey> AddAmongstPrimaryKeys(IEnumerable<TPrimaryKey> primaryKeys)
        {
            _amongstPrimaryKeys.UnionWith(primaryKeys);

            return this;
        }

        #endregion

        #region Search

        internal IEnumerable<ISearchParameter> SearchParameters { get { return _searchParameters; } }
        private readonly List<ISearchParameter> _searchParameters = new List<ISearchParameter>();

        public Search<TPrimaryKey> AddSearchExactParameter<TKey>(ParameterFactory<TKey> factory, TKey exact)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            ThrowEngineMismatchException(factory);
            ThrowDuplicateSearchException(factory);

            _searchParameters.Add(factory.CreateSearchExactParameter(exact));

            return this;
        }

        public Search<TPrimaryKey> AddSearchEnumerableParameter<TKey>(ParameterFactory<TKey> factory, IEnumerable<TKey> enumerable)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            ThrowEngineMismatchException(factory);
            ThrowDuplicateSearchException(factory);

            _searchParameters.Add(factory.CreateSearchEnumerableParameter(enumerable));

            return this;
        }

        public Search<TPrimaryKey> AddSearchRangeParameter<TKey>(ParameterFactory<TKey> factory, TKey rangeMin, TKey rangeMax)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            ThrowEngineMismatchException(factory);
            ThrowDuplicateSearchException(factory);

            _searchParameters.Add(factory.CreateSearchRangeParameter(rangeMin, rangeMax));

            return this;
        }

        private void ThrowDuplicateSearchException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (factory.IsCatalogOneToOne && _searchParameters.Any(parameter => parameter.Catalog == factory.Catalog))
                throw new ApplicationException("Can only add 1 SearchParameter per One-to-One Catalog.");
        }

        #endregion

        #region Sort

        internal IEnumerable<ISortParameter> SortParameters { get { return _sortParameters; } }
        private readonly List<ISortParameter> _sortParameters = new List<ISortParameter>();

        public Search<TPrimaryKey> AddSortDirectionalParameter<TKey>(ParameterFactory<TKey> factory, bool ascending)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            ThrowEngineMismatchException(factory);
            ThrowPrimaryKeySortExistsException();
            ThrowDuplicateSortException(factory);

            _sortParameters.Add(factory.CreateSortDirectionalParameter(ascending));

            return this;
        }

        internal bool? SortPrimaryKeyAscending { get; private set; }

        public Search<TPrimaryKey> AddSortPrimaryKey(bool ascending)
        {
            ThrowPrimaryKeySortExistsException();

            SortPrimaryKeyAscending = ascending;

            return this;
        }

        private void ThrowPrimaryKeySortExistsException()
        {
            if (SortPrimaryKeyAscending.HasValue)
                throw new ApplicationException("Cannot add any SortParameters after a SortPrimaryKey has been added.");
        }

        private void ThrowDuplicateSortException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (_sortParameters.Any(parameter => parameter.Catalog == factory.Catalog))
                throw new ApplicationException("Can only add 1 SortParameter per Catalog.");
        }

        #endregion

        #region Projection

        internal IEnumerable<IProjectionParameter> ProjectionParameters { get { return _projectionParameters; } }
        private readonly List<IProjectionParameter> _projectionParameters = new List<IProjectionParameter>();

        public Search<TPrimaryKey> AddProjectionParameter<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            ProjectionParameter<TKey> projectionParameter;
            return AddProjectionParameter(factory, out projectionParameter);
        }

        public Search<TPrimaryKey> AddProjectionParameter<TKey>(ParameterFactory<TKey> factory, out ProjectionParameter<TKey> projectionParameter)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            ThrowEngineMismatchException(factory);
            ThrowDuplicateProjectionException(factory);

            projectionParameter = factory.CreateProjectionParameter();
            _projectionParameters.Add(projectionParameter);

            return this;
        }

        private void ThrowDuplicateProjectionException<TKey>(ParameterFactory<TKey> factory)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (_projectionParameters.Any(parameter => parameter.Catalog == factory.Catalog))
                throw new ApplicationException("Can only add 1 ProjectionParameter per Catalog.");
        }

        #endregion

        public TPrimaryKey[] Execute(int skip, int take, out int totalCount)
        {
            if (Interlocked.CompareExchange(ref _executed, 1, 0) > 0)
                throw new ApplicationException("Search already executed.");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            TPrimaryKey[] results = _engine.Search(this, skip, take, out totalCount);

            watch.Stop();
            ExecuteElapsedTime = watch.Elapsed;

            return results;
        }
    }
}
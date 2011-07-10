using System;
using System.Collections.Generic;

namespace SoftwareBotany.Sunlight
{
    public sealed class ParameterFactory<TKey>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal ParameterFactory(ICatalog catalog, bool isCatalogOneToOne)
        {
            Catalog = catalog;
            IsCatalogOneToOne = isCatalogOneToOne;
        }

        internal readonly ICatalog Catalog;
        internal readonly bool IsCatalogOneToOne;

        #region Search

        internal SearchParameter<TKey> CreateSearchExactParameter(TKey exact)
        {
            return new SearchParameter<TKey>(Catalog, exact);
        }

        internal SearchParameter<TKey> CreateSearchEnumerableParameter(IEnumerable<TKey> enumerable)
        {
            return new SearchParameter<TKey>(Catalog, enumerable);
        }

        internal SearchParameter<TKey> CreateSearchRangeParameter(TKey rangeMin, TKey rangeMax)
        {
            return new SearchParameter<TKey>(Catalog, rangeMin, rangeMax);
        }

        #endregion

        #region Sort

        internal SortParameter<TKey> CreateSortDirectionalParameter(bool ascending)
        {
            return new SortParameter<TKey>(Catalog, ascending);
        }

        #endregion

        #region Projection

        internal ProjectionParameter<TKey> CreateProjectionParameter()
        {
            return new ProjectionParameter<TKey>(Catalog);
        }

        #endregion
    }
}
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

        #region Filter

        internal FilterParameter<TKey> CreateFilterExactParameter(TKey exact)
        {
            return new FilterParameter<TKey>(Catalog, exact);
        }

        internal FilterParameter<TKey> CreateFilterEnumerableParameter(IEnumerable<TKey> enumerable)
        {
            return new FilterParameter<TKey>(Catalog, enumerable);
        }

        internal FilterParameter<TKey> CreateFilterRangeParameter(TKey rangeMin, TKey rangeMax)
        {
            return new FilterParameter<TKey>(Catalog, rangeMin, rangeMax);
        }

        #endregion

        #region Sort

        internal SortParameter<TKey> CreateSortParameter(bool ascending)
        {
            return new SortParameter<TKey>(Catalog, ascending);
        }

        #endregion

        #region Facet

        internal FacetParameter<TKey> CreateFacetParameter()
        {
            return new FacetParameter<TKey>(Catalog);
        }

        #endregion
    }
}
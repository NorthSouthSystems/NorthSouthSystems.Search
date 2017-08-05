namespace Kangarooper.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Facet<TKey> : IFacet
            where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal Facet(IEnumerable<FacetCategory<TKey>> categories)
        {
            _categories = categories.Where(category => category.Count > 0).ToArray();
        }

        public IEnumerable<FacetCategory<TKey>> Categories { get { return _categories; } }
        private readonly FacetCategory<TKey>[] _categories;

        #region IFacet

        IEnumerable<IFacetCategory> IFacet.Categories { get { return _categories.Cast<IFacetCategory>(); } }

        #endregion
    }

    public interface IFacet
    {
        IEnumerable<IFacetCategory> Categories { get; }
    }
}
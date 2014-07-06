using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    public sealed class Facet<TKey> : IEnumerable<FacetCategory<TKey>>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal Facet(IEnumerable<FacetCategory<TKey>> categories)
        {
            _categories = categories.Where(category => category.Count > 0).ToArray();
        }

        private readonly FacetCategory<TKey>[] _categories;

        #region Enumeration

        IEnumerator<FacetCategory<TKey>> IEnumerable<FacetCategory<TKey>>.GetEnumerator() { return ((IEnumerable<FacetCategory<TKey>>)_categories).GetEnumerator(); }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() { return _categories.GetEnumerator(); }

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    public sealed class FacetCollection<TKey> : IEnumerable<Facet<TKey>>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal FacetCollection(IEnumerable<Facet<TKey>> facets)
        {
            _facets = facets.Where(facet => facet.Count > 0).ToArray();
        }

        private readonly Facet<TKey>[] _facets;

        #region Enumeration

        IEnumerator<Facet<TKey>> IEnumerable<Facet<TKey>>.GetEnumerator() { return ((IList<Facet<TKey>>)_facets).GetEnumerator(); }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() { return _facets.GetEnumerator(); }

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    public sealed class ProjectionCollection<TKey> : IEnumerable<Projection<TKey>>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal ProjectionCollection(IEnumerable<Projection<TKey>> projections)
        {
            _projections = projections.Where(projection => projection.Count > 0).ToArray();
        }

        private readonly Projection<TKey>[] _projections;

        #region Enumeration

        IEnumerator<Projection<TKey>> IEnumerable<Projection<TKey>>.GetEnumerator() { return ((IList<Projection<TKey>>)_projections).GetEnumerator(); }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() { return _projections.GetEnumerator(); }

        #endregion
    }
}
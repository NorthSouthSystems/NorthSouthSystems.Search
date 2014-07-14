namespace SoftwareBotany.Sunlight
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class CatalogPartialSortResult<TKey> : IEnumerable<int>
    {
        internal CatalogPartialSortResult(TKey key, IEnumerable<int> sortedBitPositions)
        {
            _key = key;
            _sortedBitPositions = sortedBitPositions;
        }

        public TKey Key { get { return _key; } }
        private readonly TKey _key;

        private readonly IEnumerable<int> _sortedBitPositions;

        public IEnumerator<int> GetEnumerator() { return _sortedBitPositions.GetEnumerator(); }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() { return _sortedBitPositions.GetEnumerator(); }
    }
}
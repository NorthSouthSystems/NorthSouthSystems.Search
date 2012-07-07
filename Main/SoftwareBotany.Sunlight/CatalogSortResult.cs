using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    public sealed class CatalogSortResult<TKey> : IEnumerable<CatalogPartialSortResult<TKey>>, ICatalogSortResult
    {
        internal CatalogSortResult(IEnumerable<CatalogPartialSortResult<TKey>> partialSortResults)
        {
            _partialSortResults = partialSortResults;
        }

        private readonly IEnumerable<CatalogPartialSortResult<TKey>> _partialSortResults;

        public IEnumerator<CatalogPartialSortResult<TKey>> GetEnumerator() { return _partialSortResults.GetEnumerator(); }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() { return _partialSortResults.GetEnumerator(); }

        IEnumerable<IEnumerable<int>> ICatalogSortResult.PartialSortResultsBitPositions
        {
            get { return _partialSortResults.Select(partial => (IEnumerable<int>)partial); }
        }
    }
}
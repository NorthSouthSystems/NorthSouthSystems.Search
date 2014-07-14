namespace SoftwareBotany.Sunlight
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public sealed class CatalogSortResult<TKey> : IEnumerable<CatalogPartialSortResult<TKey>>, ICatalogInEngineSortResult
    {
        internal CatalogSortResult(IEnumerable<CatalogPartialSortResult<TKey>> partialSortResults)
        {
            _partialSortResults = partialSortResults;
        }

        private readonly IEnumerable<CatalogPartialSortResult<TKey>> _partialSortResults;

        public IEnumerator<CatalogPartialSortResult<TKey>> GetEnumerator() { return _partialSortResults.GetEnumerator(); }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() { return _partialSortResults.GetEnumerator(); }

        IEnumerable<IEnumerable<int>> ICatalogInEngineSortResult.PartialSortResultsBitPositions
        {
            get { return _partialSortResults.Select(partial => (IEnumerable<int>)partial); }
        }
    }

    internal interface ICatalogInEngineSortResult
    {
        IEnumerable<IEnumerable<int>> PartialSortResultsBitPositions { get; }
    }
}
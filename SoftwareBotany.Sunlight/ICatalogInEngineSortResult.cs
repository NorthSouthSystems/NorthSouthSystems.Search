using System.Collections.Generic;

namespace SoftwareBotany.Sunlight
{
    internal interface ICatalogInEngineSortResult
    {
        IEnumerable<IEnumerable<int>> PartialSortResultsBitPositions { get; }
    }
}
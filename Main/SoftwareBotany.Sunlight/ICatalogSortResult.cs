using System.Collections.Generic;

namespace SoftwareBotany.Sunlight
{
    internal interface ICatalogSortResult
    {
        IEnumerable<IEnumerable<int>> PartialSortResultsBitPositions { get; }
    }
}
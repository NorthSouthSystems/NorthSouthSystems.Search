namespace FOSStrich.Search;

public sealed class CatalogSortResult
{
    internal CatalogSortResult(IEnumerable<Vector> partialSorts) =>
        PartialSorts = partialSorts;

    public IEnumerable<Vector> PartialSorts { get; }
}
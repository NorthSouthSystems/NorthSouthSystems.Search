namespace FOSStrich.Search;

public sealed class CatalogSortResult
{
    internal CatalogSortResult(IEnumerable<Vector> partialSorts)
    {
        _partialSorts = partialSorts;
    }

    public IEnumerable<Vector> PartialSorts => _partialSorts;
    private readonly IEnumerable<Vector> _partialSorts;
}
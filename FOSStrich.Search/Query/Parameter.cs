namespace FOSStrich.Search;

using FOSStrich.BitVectors;

public interface IParameter
{
    ICatalogHandle Catalog { get; }
}

internal static class ParameterHelper
{
    internal static TIParameter CreateLooselyTyped<TBitVector, TItem, TPrimaryKey, TIParameter>(
            Engine<TBitVector, TItem, TPrimaryKey> engine, string catalogName,
            Func<ICatalogInEngine<TBitVector>, TIParameter> creator)
        where TBitVector : IBitVector<TBitVector>
        where TIParameter : IParameter
    {
        if (engine == null)
            throw new ArgumentNullException(nameof(engine));

        if (string.IsNullOrWhiteSpace(catalogName))
            throw new ArgumentNullException(nameof(catalogName));

        var catalog = engine.GetCatalog(catalogName);

        return creator(catalog);
    }
}
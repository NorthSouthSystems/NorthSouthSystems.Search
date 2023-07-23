namespace FOSStrich.Search;

using System;

public interface IParameter
{
    ICatalogHandle Catalog { get; }
}

internal static class ParameterHelper
{
    internal static TIParameter CreateLooselyTyped<TItem, TPrimaryKey, TIParameter>(Engine<TItem, TPrimaryKey> engine, string catalogName,
        Func<ICatalogInEngine, TIParameter> creator)
        where TIParameter : IParameter
    {
        if (engine == null)
            throw new ArgumentNullException("engine");

        if (string.IsNullOrWhiteSpace(catalogName))
            throw new ArgumentNullException("catalogName");

        var catalog = engine.GetCatalog(catalogName);

        return creator(catalog);
    }
}
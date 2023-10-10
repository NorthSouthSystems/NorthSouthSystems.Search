namespace FOSStrich.Search;

using FOSStrich.BitVectors;

public static class SortParameter
{
    public static SortParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, bool ascending)
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog, ascending);

    internal static ISortParameter Create<TBitVector, TItem, TPrimaryKey>(Engine<TBitVector, TItem, TPrimaryKey> engine, string catalogName, bool ascending)
            where TBitVector : IBitVector<TBitVector> =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateSortParameter(ascending));
}

public sealed class SortParameter<TKey> : ISortParameter
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    internal SortParameter(ICatalogHandle<TKey> catalog, bool ascending)
    {
        Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        Ascending = ascending;
    }

    public ICatalogHandle Catalog { get; }
    public bool Ascending { get; }
}

public interface ISortParameter : IParameter
{
    bool Ascending { get; }
}
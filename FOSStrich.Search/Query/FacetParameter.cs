namespace FOSStrich.Search;

using FOSStrich.BitVectors;

public static class FacetParameter
{
    public static FacetParameter<TBitVector, TKey> Create<TBitVector, TKey>(ICatalogHandle<TKey> catalog)
            where TBitVector : IBitVector<TBitVector>
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog);

    internal static IFacetParameter Create<TBitVector, TItem, TPrimaryKey>(Engine<TBitVector, TItem, TPrimaryKey> engine, string catalogName)
            where TBitVector : IBitVector<TBitVector> =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFacetParameter());
}

public sealed class FacetParameter<TBitVector, TKey> : IFacetParameterInternal
    where TBitVector : IBitVector<TBitVector>
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    internal FacetParameter(ICatalogHandle<TKey> catalog) =>
        Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));

    public ICatalogHandle Catalog { get; }

    public Facet<TKey> Facet
    {
        get
        {
            if (!_facetSet)
                throw new NotSupportedException("Query must be executed before Facet is available.");

            return _facet;
        }
        private set
        {
            _facet = value;
            _facetSet = true;
        }
    }

    private Facet<TKey> _facet;
    private bool _facetSet = false;

    #region IFacetParameterInternal

    IFacet IFacetParameterInternal.Facet
    {
        get => Facet;
        set => Facet = (Facet<TKey>)value;
    }

    #endregion

    #region IFacetParameter

    IFacet IFacetParameter.Facet => Facet;

    #endregion
}

internal interface IFacetParameterInternal : IFacetParameter
{
    new IFacet Facet { get; set; }
}

public interface IFacetParameter : IParameter
{
    IFacet Facet { get; }
}
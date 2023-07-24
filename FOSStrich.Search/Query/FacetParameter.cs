namespace FOSStrich.Search;

public static class FacetParameter
{
    public static FacetParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog)
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog);

    internal static IFacetParameter Create<TItem, TPrimaryKey>(Engine<TItem, TPrimaryKey> engine, string catalogName) =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFacetParameter());
}

public sealed class FacetParameter<TKey> : IFacetParameterInternal
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    internal FacetParameter(ICatalogHandle<TKey> catalog)
    {
        if (catalog == null)
            throw new ArgumentNullException(nameof(catalog));

        _catalog = catalog;
    }

    public ICatalogHandle Catalog { get { return _catalog; } }
    private readonly ICatalogHandle _catalog;

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
        get { return Facet; }
        set { Facet = (Facet<TKey>)value; }
    }

    #endregion

    #region IFacetParameter

    IFacet IFacetParameter.Facet { get { return Facet; } }

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
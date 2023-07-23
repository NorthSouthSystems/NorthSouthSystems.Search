namespace FOSStrich.Search;

using System.Diagnostics;

public sealed class Query<TItem, TPrimaryKey>
{
    internal Query(Engine<TItem, TPrimaryKey> engine)
    {
        _engine = engine;
    }

    private readonly Engine<TItem, TPrimaryKey> _engine;

    private void ThrowIfEngineMismatch(ICatalogHandle catalog)
    {
        if (!_engine.HasCatalog(catalog))
            throw new ArgumentException("Catalog belongs to a different Engine.", "catalog");
    }

    #region Amongst

    public IEnumerable<TPrimaryKey> AmongstPrimaryKeys { get { return _amongstPrimaryKeys; } }
    private readonly List<TPrimaryKey> _amongstPrimaryKeys = new List<TPrimaryKey>();

    public Query<TItem, TPrimaryKey> Amongst(IEnumerable<TPrimaryKey> primaryKeys)
    {
        ThrowIfExecuted();

        if (_amongstPrimaryKeys.Any())
            throw new NotSupportedException("Amongst may only be called once.");

        _amongstPrimaryKeys.AddRange(primaryKeys ?? Enumerable.Empty<TPrimaryKey>());

        return this;
    }

    #endregion

    #region Filter

    public FilterClause FilterClause { get; private set; }

    public Query<TItem, TPrimaryKey> Filter(FilterClause filterClause)
    {
        ThrowIfExecuted();

        if (FilterClause != null)
            throw new NotSupportedException("Filter may only be called once.");

        foreach (var filterParameter in (filterClause == null ? Enumerable.Empty<IFilterParameter>() : filterClause.AllFilterParameters()))
            ThrowIfEngineMismatch(filterParameter.Catalog);

        FilterClause = filterClause;

        return this;
    }

    #endregion

    #region Sort

    public IEnumerable<ISortParameter> SortParameters { get { return _sortParameters; } }
    private readonly List<ISortParameter> _sortParameters = new List<ISortParameter>();

    public Query<TItem, TPrimaryKey> Sort(params ISortParameter[] sortParameters)
    {
        ThrowIfExecuted();

        if (_sortParameters.Any())
            throw new NotSupportedException("Sort may only be called once.");

        if (SortPrimaryKeyAscending.HasValue)
            throw new NotSupportedException("Sort must be called before SortPrimaryKey.");

        foreach (var sortParameter in (sortParameters ?? Enumerable.Empty<ISortParameter>()).Where(p => p != null))
        {
            ThrowIfEngineMismatch(sortParameter.Catalog);

            if (_sortParameters.Any(parameter => parameter.Catalog == sortParameter.Catalog))
                throw new NotSupportedException("Can only add 1 Sort Parameter per Catalog.");

            _sortParameters.Add(sortParameter);
        }

        return this;
    }

    public bool? SortPrimaryKeyAscending { get; private set; }

    public Query<TItem, TPrimaryKey> SortPrimaryKey(bool ascending)
    {
        ThrowIfExecuted();

        if (SortPrimaryKeyAscending.HasValue)
            throw new NotSupportedException("SortPrimaryKey may only be called once.");

        SortPrimaryKeyAscending = ascending;

        return this;
    }

    public bool SortDisableParallel { get; private set; }

    public Query<TItem, TPrimaryKey> WithSortOptions(bool sortDisableParallel = false)
    {
        ThrowIfExecuted();

        SortDisableParallel = sortDisableParallel;

        return this;
    }

    #endregion

    #region Facet

    public IEnumerable<IFacetParameter> FacetParameters { get { return _facetParameters; } }
    internal IEnumerable<IFacetParameterInternal> FacetParametersInternal { get { return _facetParameters; } }
    private readonly List<IFacetParameterInternal> _facetParameters = new List<IFacetParameterInternal>();

    public Query<TItem, TPrimaryKey> FacetAll()
    {
        var facetParameters = _engine.GetCatalogs()
            .Select(catalog => (IFacetParameter)catalog.CreateFacetParameter())
            .ToArray();

        return Facet(facetParameters);
    }

    public Query<TItem, TPrimaryKey> Facet(params IFacetParameter[] facetParameters)
    {
        ThrowIfExecuted();

        if (_facetParameters.Any())
            throw new NotSupportedException("Facet may only be called once.");

        foreach (var facetParameter in (facetParameters ?? Enumerable.Empty<IFacetParameter>()).Where(p => p != null))
        {
            ThrowIfEngineMismatch(facetParameter.Catalog);

            if (_facetParameters.Any(parameter => parameter.Catalog == facetParameter.Catalog))
                throw new NotSupportedException("Can only add 1 Facet Parameter per Catalog.");

            _facetParameters.Add((IFacetParameterInternal)facetParameter);
        }

        return this;
    }

    public bool FacetDisableParallel { get; private set; }
    public bool FacetShortCircuitCounting { get; private set; }

    public Query<TItem, TPrimaryKey> WithFacetOptions(bool facetDisableParallel = false, bool facetShortCircuitCounting = false)
    {
        ThrowIfExecuted();

        FacetDisableParallel = facetDisableParallel;
        FacetShortCircuitCounting = facetShortCircuitCounting;

        return this;
    }

    #endregion

    #region Execution

    public bool Executed { get; private set; }

    private void ThrowIfExecuted()
    {
        if (Executed)
            throw new NotSupportedException("Query has already been executed.");
    }

    private void ThrowIfNotExecuted()
    {
        if (!Executed)
            throw new NotSupportedException("Query has not yet been executed.");
    }

    public TPrimaryKey[] ResultPrimaryKeys
    {
        get
        {
            ThrowIfNotExecuted();

            return _resultPrimaryKeys;
        }
    }
    private TPrimaryKey[] _resultPrimaryKeys;

    public int ResultTotalCount
    {
        get
        {
            ThrowIfNotExecuted();

            return _resultTotalCount;
        }
    }
    private int _resultTotalCount;

    public TimeSpan ResultElapsedTime
    {
        get
        {
            ThrowIfNotExecuted();

            return _resultElapsedTime;
        }
    }
    private TimeSpan _resultElapsedTime;

    public Query<TItem, TPrimaryKey> Execute(int skip, int take)
    {
        ThrowIfExecuted();

        Executed = true;

        Stopwatch watch = new Stopwatch();
        watch.Start();

        int totalCount;
        _resultPrimaryKeys = _engine.ExecuteQuery(this, skip, take, out totalCount);
        _resultTotalCount = totalCount;

        watch.Stop();
        _resultElapsedTime = watch.Elapsed;

        return this;
    }

    #endregion
}
namespace FOSStrich.Search;

using System.Collections;

public static class FilterParameter
{
    public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, TKey exact)
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog, exact);

    public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, IEnumerable<TKey> enumerable)
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog, enumerable);

    public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, TKey rangeMin, TKey rangeMax)
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog, rangeMin, rangeMax);

    internal static IFilterParameter Create<TItem, TPrimaryKey>(Engine<TItem, TPrimaryKey> engine, string catalogName, object exact) =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(exact));

    internal static IFilterParameter Create<TItem, TPrimaryKey>(Engine<TItem, TPrimaryKey> engine, string catalogName, IEnumerable enumerable) =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(enumerable));

    internal static IFilterParameter Create<TItem, TPrimaryKey>(Engine<TItem, TPrimaryKey> engine, string catalogName, object rangeMin, object rangeMax) =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(rangeMin, rangeMax));
}

public sealed class FilterParameter<TKey> : FilterClause, IFilterParameter
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    internal FilterParameter(ICatalogHandle<TKey> catalog, TKey exact)
        : this(catalog, FilterParameterType.Exact, exact: exact)
    { }

    internal FilterParameter(ICatalogHandle<TKey> catalog, IEnumerable<TKey> enumerable)
        : this(catalog, FilterParameterType.Enumerable, enumerable: enumerable)
    { }

    internal FilterParameter(ICatalogHandle<TKey> catalog, TKey rangeMin, TKey rangeMax)
        : this(catalog, FilterParameterType.Range, rangeMin: rangeMin, rangeMax: rangeMax)
    { }

    private FilterParameter(ICatalogHandle<TKey> catalog, FilterParameterType parameterType,
        TKey exact = default(TKey), IEnumerable<TKey> enumerable = null, TKey rangeMin = default(TKey), TKey rangeMax = default(TKey))
    {
        if (catalog == null)
            throw new ArgumentNullException(nameof(catalog));

        if (parameterType == FilterParameterType.Range)
        {
            if (rangeMin == null && rangeMax == null)
                throw new ArgumentNullException(nameof(rangeMin), "Either rangeMin or rangeMax must be non-null.");

            if (rangeMin != null && rangeMax != null && rangeMin.CompareTo(rangeMax) > 0)
                throw new ArgumentOutOfRangeException(nameof(rangeMin), "rangeMin must be <= rangeMax.");
        }

        _catalog = catalog;
        _parameterType = parameterType;
        _exact = exact;
        _enumerable = enumerable;
        _rangeMin = rangeMin;
        _rangeMax = rangeMax;
    }

    public ICatalogHandle Catalog => _catalog;
    private readonly ICatalogHandle _catalog;

    public FilterParameterType ParameterType => _parameterType;
    private readonly FilterParameterType _parameterType;

    public TKey Exact => _exact;
    private readonly TKey _exact;

    public IEnumerable<TKey> Enumerable => _enumerable;
    private readonly IEnumerable<TKey> _enumerable;

    public TKey RangeMin => _rangeMin;
    private readonly TKey _rangeMin;

    public TKey RangeMax => _rangeMax;
    private readonly TKey _rangeMax;

    #region IFilterParameter

    object IFilterParameter.Exact => Exact;
    IEnumerable IFilterParameter.Enumerable => Enumerable;
    object IFilterParameter.RangeMin => RangeMin;
    object IFilterParameter.RangeMax => RangeMax;

    #endregion
}

public interface IFilterParameter : IParameter
{
    FilterParameterType ParameterType { get; }
    object Exact { get; }
    IEnumerable Enumerable { get; }
    object RangeMin { get; }
    object RangeMax { get; }
}

public enum FilterParameterType
{
    Exact,
    Enumerable,
    Range
}
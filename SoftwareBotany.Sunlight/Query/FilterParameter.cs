namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public static class FilterParameter
    {
        public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, TKey exact)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return new FilterParameter<TKey>(catalog, exact);
        }

        public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, IEnumerable<TKey> enumerable)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return new FilterParameter<TKey>(catalog, enumerable);
        }

        public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, TKey rangeMin, TKey rangeMax)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return new FilterParameter<TKey>(catalog, rangeMin, rangeMax);
        }

        public static IFilterParameter Create<TItem, TPrimaryKey>(Engine<TItem, TPrimaryKey> engine, string catalogName, object exact)
        {
            return ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(exact));
        }

        public static IFilterParameter Create<TItem, TPrimaryKey>(Engine<TItem, TPrimaryKey> engine, string catalogName, IEnumerable enumerable)
        {
            return ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(enumerable));
        }

        public static IFilterParameter Create<TItem, TPrimaryKey>(Engine<TItem, TPrimaryKey> engine, string catalogName, object rangeMin, object rangeMax)
        {
            return ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(rangeMin, rangeMax));
        }
    }

    public sealed class FilterParameter<TKey> : Parameter, IFilterParameter
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
            : base(catalog)
        {
            if (parameterType == FilterParameterType.Range)
            {
                if (rangeMin == null && rangeMax == null)
                    throw new ArgumentNullException("rangeMin", "Either rangeMin or rangeMax must be non-null.");

                if (rangeMin != null && rangeMax != null && rangeMin.CompareTo(rangeMax) > 0)
                    throw new ArgumentOutOfRangeException("rangeMin", "rangeMin must be <= rangeMax.");
            }

            Contract.EndContractBlock();

            _parameterType = parameterType;
            _exact = exact;
            _enumerable = enumerable;
            _rangeMin = rangeMin;
            _rangeMax = rangeMax;
        }

        public FilterParameterType ParameterType { get { return _parameterType; } }
        private readonly FilterParameterType _parameterType;

        public TKey Exact { get { return _exact; } }
        private readonly TKey _exact;

        public IEnumerable<TKey> Enumerable { get { return _enumerable; } }
        private readonly IEnumerable<TKey> _enumerable;

        public TKey RangeMin { get { return _rangeMin; } }
        private readonly TKey _rangeMin;

        public TKey RangeMax { get { return _rangeMax; } }
        private readonly TKey _rangeMax;

        #region IFilterParameter

        object IFilterParameter.Exact { get { return Exact; } }
        IEnumerable IFilterParameter.Enumerable { get { return Enumerable; } }
        object IFilterParameter.RangeMin { get { return RangeMin; } }
        object IFilterParameter.RangeMax { get { return RangeMax; } }

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
}
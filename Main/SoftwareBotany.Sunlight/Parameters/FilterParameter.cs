﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SoftwareBotany.Sunlight
{
    public sealed class FilterParameter<TKey> : Parameter, IFilterParameter
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal FilterParameter(ICatalog catalog, TKey exact)
            : this(catalog, FilterParameterType.Exact, exact: exact)
        { }

        internal FilterParameter(ICatalog catalog, IEnumerable<TKey> enumerable)
            : this(catalog, FilterParameterType.Enumerable, enumerable: enumerable)
        { }

        internal FilterParameter(ICatalog catalog, TKey rangeMin, TKey rangeMax)
            : this(catalog, FilterParameterType.Range, rangeMin: rangeMin, rangeMax: rangeMax)
        { }

        private FilterParameter(ICatalog catalog, FilterParameterType parameterType,
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
        object IFilterParameter.Enumerable { get { return Enumerable; } }
        object IFilterParameter.RangeMin { get { return RangeMin; } }
        object IFilterParameter.RangeMax { get { return RangeMax; } }

        #endregion
    }

    internal interface IFilterParameter : IParameter
    {
        FilterParameterType ParameterType { get; }
        object Exact { get; }
        object Enumerable { get; }
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
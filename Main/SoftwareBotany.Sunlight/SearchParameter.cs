﻿using System;
using System.Collections.Generic;

namespace SoftwareBotany.Sunlight
{
    internal class SearchParameter<TKey> : Parameter, ISearchParameter
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal SearchParameter(ICatalog catalog, TKey exact)
            : this(catalog, SearchParameterType.Exact, exact: exact)
        { }

        internal SearchParameter(ICatalog catalog, IEnumerable<TKey> enumerable)
            : this(catalog, SearchParameterType.Enumerable, enumerable: enumerable)
        { }

        internal SearchParameter(ICatalog catalog, TKey rangeMin, TKey rangeMax)
            : this(catalog, SearchParameterType.Range, rangeMin: rangeMin, rangeMax: rangeMax)
        { }

        private SearchParameter(ICatalog catalog, SearchParameterType parameterType,
            TKey exact = default(TKey), IEnumerable<TKey> enumerable = null, TKey rangeMin = default(TKey), TKey rangeMax = default(TKey))
            : base(catalog)
        {
            _parameterType = parameterType;
            _exact = exact;
            _enumerable = enumerable;
            _rangeMin = rangeMin;
            _rangeMax = rangeMax;
        }

        public SearchParameterType ParameterType { get { return _parameterType; } }
        private readonly SearchParameterType _parameterType;

        public TKey Exact { get { return _exact; } }
        private readonly TKey _exact;

        public IEnumerable<TKey> Enumerable { get { return _enumerable; } }
        private readonly IEnumerable<TKey> _enumerable;

        public TKey RangeMin { get { return _rangeMin; } }
        private readonly TKey _rangeMin;

        public TKey RangeMax { get { return _rangeMax; } }
        private readonly TKey _rangeMax;

        #region ISearchParameter

        dynamic ISearchParameter.DynamicExact { get { return _exact; } }
        dynamic ISearchParameter.DynamicEnumerable { get { return _enumerable; } }
        dynamic ISearchParameter.DynamicRangeMin { get { return _rangeMin; } }
        dynamic ISearchParameter.DynamicRangeMax { get { return _rangeMax; } }

        #endregion
    }

    internal interface ISearchParameter : IParameter
    {
        SearchParameterType ParameterType { get; }
        dynamic DynamicExact { get; }
        dynamic DynamicEnumerable { get; }
        dynamic DynamicRangeMin { get; }
        dynamic DynamicRangeMax { get; }
    }

    internal enum SearchParameterType
    {
        Exact,
        Enumerable,
        Range
    }
}
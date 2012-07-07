using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SoftwareBotany.Sunlight
{
    public sealed class SearchParameter<TKey> : Parameter, ISearchParameter
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
            if (parameterType == SearchParameterType.Range)
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

        object ISearchParameter.Exact { get { return Exact; } }
        object ISearchParameter.Enumerable { get { return Enumerable; } }
        object ISearchParameter.RangeMin { get { return RangeMin; } }
        object ISearchParameter.RangeMax { get { return RangeMax; } }

        #endregion
    }

    internal interface ISearchParameter : IParameter
    {
        SearchParameterType ParameterType { get; }
        object Exact { get; }
        object Enumerable { get; }
        object RangeMin { get; }
        object RangeMax { get; }
    }

    public enum SearchParameterType
    {
        Exact,
        Enumerable,
        Range
    }
}
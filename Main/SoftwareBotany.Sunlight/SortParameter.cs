using System;

namespace SoftwareBotany.Sunlight
{
    internal sealed class SortParameter<TKey> : Parameter, ISortParameter
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal SortParameter(ICatalog catalog, bool ascending)
            : base(catalog)
        {
            _parameterType = SortParameterType.Directional;
            _ascending = ascending;
        }

        public SortParameterType ParameterType { get { return _parameterType; } }
        private readonly SortParameterType _parameterType;

        public bool Ascending { get { return _ascending; } }
        private readonly bool _ascending;
    }

    internal interface ISortParameter : IParameter
    {
        SortParameterType ParameterType { get; }
        bool Ascending { get; }
    }

    internal enum SortParameterType
    {
        Directional
    }
}
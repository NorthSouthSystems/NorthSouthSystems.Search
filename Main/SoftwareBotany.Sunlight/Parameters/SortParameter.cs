using System;

namespace SoftwareBotany.Sunlight
{
    public sealed class SortParameter<TKey> : Parameter, ISortParameterInternal
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal SortParameter(ICatalog catalog, bool ascending)
            : base(catalog)
        {
            _ascending = ascending;
        }

        public bool Ascending { get { return _ascending; } }
        private readonly bool _ascending;
    }

    internal interface ISortParameterInternal : IParameterInternal, ISortParameter { }

    public interface ISortParameter : IParameter
    {
        bool Ascending { get; }
    }
}
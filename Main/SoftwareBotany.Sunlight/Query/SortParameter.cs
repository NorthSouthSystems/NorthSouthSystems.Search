namespace SoftwareBotany.Sunlight
{
    using System;

    public sealed class SortParameter<TKey> : Parameter, ISortParameter
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal SortParameter(ICatalogHandle<TKey> catalog, bool ascending)
            : base(catalog)
        {
            _ascending = ascending;
        }

        public bool Ascending { get { return _ascending; } }
        private readonly bool _ascending;
    }

    public interface ISortParameter : IParameter
    {
        bool Ascending { get; }
    }
}
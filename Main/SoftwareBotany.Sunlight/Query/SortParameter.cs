namespace SoftwareBotany.Sunlight
{
    using System;

    public static class SortParameter
    {
        public static SortParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, bool ascending)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return new SortParameter<TKey>(catalog, ascending);
        }

        public static ISortParameter Create<TItem, TPrimaryKey>(Engine<TItem, TPrimaryKey> engine, string catalogName, bool ascending)
        {
            return ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateSortParameter(ascending));
        }
    }

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
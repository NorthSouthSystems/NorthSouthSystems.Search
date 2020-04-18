namespace FreeAndWithBeer.Search
{
    using System;

    public struct FacetCategory<TKey> : IFacetCategory
            where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal FacetCategory(TKey key, int count)
        {
            _key = key;
            _count = count;
        }

        public TKey Key { get { return _key; } }
        private readonly TKey _key;

        public int Count { get { return _count; } }
        private readonly int _count;

        #region IFacetCategory

        object IFacetCategory.Key { get { return Key; } }

        #endregion

        #region Equality & Hashing

        public bool Equals(FacetCategory<TKey> other)
        {
            return _key.Equals(other._key) && _count.Equals(other._count);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(FacetCategory<TKey>) && Equals((FacetCategory<TKey>)obj);
        }

        public static bool operator ==(FacetCategory<TKey> left, FacetCategory<TKey> right) { return left.Equals(right); }
        public static bool operator !=(FacetCategory<TKey> left, FacetCategory<TKey> right) { return !left.Equals(right); }

        public override int GetHashCode()
        {
            return _key.GetHashCode() ^ _count.GetHashCode();
        }

        #endregion
    }

    public interface IFacetCategory
    {
        object Key { get; }
        int Count { get; }
    }
}
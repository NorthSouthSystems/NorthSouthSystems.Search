using System;

namespace SoftwareBotany.Sunlight
{
    public struct Projection<TKey>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal Projection(TKey key, int count)
        {
            _key = key;
            _count = count;
        }

        public TKey Key { get { return _key; } }
        private readonly TKey _key;

        public int Count { get { return _count; } }
        private readonly int _count;

        #region Equality & Hashing

        public bool Equals(Projection<TKey> other)
        {
            return _key.Equals(other._key) && _count.Equals(other._count);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(Projection<TKey>) && this.Equals((Projection<TKey>)obj);
        }

        public static bool operator ==(Projection<TKey> left, Projection<TKey> right) { return left.Equals(right); }
        public static bool operator !=(Projection<TKey> left, Projection<TKey> right) { return !left.Equals(right); }

        public override int GetHashCode()
        {
            return _key.GetHashCode() ^ _count.GetHashCode();
        }

        #endregion
    }
}
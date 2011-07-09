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
    }
}
namespace SoftwareBotany.Sunlight
{
    using System;

    public static class CatalogTestExtensions
    {
        public static void Fill<TKey>(this Catalog<TKey> catalog, TKey key, int[] bitPositions, bool value)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            foreach (int bitPosition in bitPositions)
                catalog.Set(key, bitPosition, value);
        }
    }
}
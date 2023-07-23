namespace FreeAndWithBeer.Search
{
    using System;

    internal static class CatalogExtensions
    {
        internal static void Fill<TKey>(this Catalog<TKey> catalog, TKey key, int[] bitPositions, bool value)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            foreach (int bitPosition in bitPositions)
                catalog.Set(key, bitPosition, value);
        }
    }
}
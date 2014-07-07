using System;

namespace SoftwareBotany.Sunlight
{
    public sealed class CatalogHandle<TKey>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal CatalogHandle(ICatalog catalog, bool isCatalogOneToOne)
        {
            Catalog = catalog;
            IsCatalogOneToOne = isCatalogOneToOne;
        }

        internal readonly ICatalog Catalog;
        internal readonly bool IsCatalogOneToOne;
    }
}
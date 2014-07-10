namespace SoftwareBotany.Sunlight
{
    public abstract class Parameter : IParameter
    {
        internal Parameter(ICatalogHandle catalog)
        {
            _catalog = catalog;
        }

        public ICatalogHandle Catalog { get { return _catalog; } }
        private readonly ICatalogHandle _catalog;
    }

    public interface IParameter
    {
        ICatalogHandle Catalog { get; }
    }
}
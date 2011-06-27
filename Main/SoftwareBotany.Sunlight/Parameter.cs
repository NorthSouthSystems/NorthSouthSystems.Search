namespace SoftwareBotany.Sunlight
{
    public abstract class Parameter : IParameter
    {
        internal Parameter(ICatalog catalog)
        {
            _catalog = catalog;
        }

        ICatalog IParameter.Catalog { get { return _catalog; } }
        private readonly ICatalog _catalog;
    }

    internal interface IParameter
    {
        ICatalog Catalog { get; }
    }
}
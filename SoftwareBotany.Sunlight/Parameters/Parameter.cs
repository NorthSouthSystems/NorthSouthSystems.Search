namespace SoftwareBotany.Sunlight
{
    public abstract class Parameter : IParameterInternal
    {
        internal Parameter(ICatalog catalog)
        {
            _catalog = catalog;
        }

        ICatalog IParameterInternal.Catalog { get { return _catalog; } }
        private readonly ICatalog _catalog;
    }

    internal interface IParameterInternal : IParameter
    {
        ICatalog Catalog { get; }
    }

    public interface IParameter { }
}
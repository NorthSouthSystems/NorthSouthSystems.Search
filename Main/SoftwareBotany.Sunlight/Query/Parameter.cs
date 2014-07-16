namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Diagnostics.Contracts;

    internal static class ParameterHelper
    {
        internal static TIParameter CreateLooselyTyped<TItem, TPrimaryKey, TIParameter>(Engine<TItem, TPrimaryKey> engine, string catalogName,
            Func<ICatalogInEngine, TIParameter> creator)
            where TIParameter : IParameter
        {
            if (engine == null)
                throw new ArgumentNullException("engine");

            if (string.IsNullOrWhiteSpace(catalogName))
                throw new ArgumentNullException("catalogName");

            Contract.EndContractBlock();

            var catalog = engine.GetCatalog(catalogName);

            return creator(catalog);
        }
    }

    public abstract class Parameter : IParameter
    {
        protected internal Parameter(ICatalogHandle catalog)
        {
            if (catalog == null)
                throw new ArgumentNullException("catalog");

            Contract.EndContractBlock();

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
using System;

namespace SoftwareBotany.Sunlight
{
    public sealed class FacetParameter<TKey> : Parameter, IFacetParameterInternal
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal FacetParameter(ICatalog catalog)
            : base(catalog)
        { }

        public Facet<TKey> Facet
        {
            get
            {
                if (!_facetSet)
                    throw new NotSupportedException("Query must be executed before Facet is available.");

                return _facet;
            }
            private set
            {
                _facet = value;
                _facetSet = true;
            }
        }

        private Facet<TKey> _facet;
        private bool _facetSet = false;

        #region IFacetParameterInternal

        IFacet IFacetParameterInternal.Facet
        {
            get { return Facet; }
            set { Facet = (Facet<TKey>)value; }
        }

        #endregion

        #region IFacetParameter

        IFacet IFacetParameter.Facet { get { return Facet; } }

        #endregion
    }

    internal interface IFacetParameterInternal : IParameterInternal, IFacetParameter
    {
        new IFacet Facet { get; set; }
    }

    public interface IFacetParameter : IParameter
    {
        IFacet Facet { get; }
    }
}
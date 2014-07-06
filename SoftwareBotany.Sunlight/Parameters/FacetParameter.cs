using System;

namespace SoftwareBotany.Sunlight
{
    public sealed class FacetParameter<TKey> : Parameter, IFacetParameter
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
                    throw new NotSupportedException("Search must be executed before Facet is available.");

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

        #region IFacetParameter

        object IFacetParameter.Facet
        {
            get { return Facet; }
            set { Facet = (Facet<TKey>)value; }
        }

        #endregion
    }

    internal interface IFacetParameter : IParameter
    {
        object Facet { get; set; }
    }
}
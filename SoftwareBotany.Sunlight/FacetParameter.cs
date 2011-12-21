using System;

namespace SoftwareBotany.Sunlight
{
    public sealed class FacetParameter<TKey> : Parameter, IFacetParameter
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal FacetParameter(ICatalog catalog)
            : base(catalog)
        { }

        public FacetCollection<TKey> Facets
        {
            get
            {
                if (!_facetsSet)
                    throw new NotSupportedException("Search must be executed before Facets are available.");

                return _facets;
            }
            private set
            {
                _facets = value;
                _facetsSet = true;
            }
        }

        private FacetCollection<TKey> _facets;
        private bool _facetsSet = false;

        #region IFacetParameter

        dynamic IFacetParameter.DynamicFacets
        {
            get { return Facets; }
            set { Facets = value; }
        }

        #endregion
    }

    internal interface IFacetParameter : IParameter
    {
        dynamic DynamicFacets { get; set; }
    }
}
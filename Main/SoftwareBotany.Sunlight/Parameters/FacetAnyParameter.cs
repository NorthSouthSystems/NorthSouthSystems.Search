using System;
using System.Collections.Generic;

namespace SoftwareBotany.Sunlight
{
    public sealed class FacetAnyParameter<TKey> : Parameter, IFacetAnyParameter
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal FacetAnyParameter(ICatalog catalog)
            : base(catalog)
        { }

        public IEnumerable<TKey> FacetAnys
        {
            get
            {
                if (!_facetAnysSet)
                    throw new NotSupportedException("Search must be executed before Facet Anys are available.");

                return _facetAnys;
            }
            private set
            {
                _facetAnys = (TKey[])value;
                _facetAnysSet = true;
            }
        }

        private TKey[] _facetAnys;
        private bool _facetAnysSet = false;

        #region IFacetAnyParameter

        object IFacetAnyParameter.FacetAnys
        {
            get { return FacetAnys; }
            set { FacetAnys = (TKey[])value; }
        }

        #endregion
    }

    internal interface IFacetAnyParameter : IParameter
    {
        object FacetAnys { get; set; }
    }
}
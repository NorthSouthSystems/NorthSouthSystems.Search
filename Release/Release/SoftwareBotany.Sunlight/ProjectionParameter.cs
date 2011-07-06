using System;
using System.Collections.Generic;

namespace SoftwareBotany.Sunlight
{
    public class ProjectionParameter<TKey> : Parameter, IProjectionParameter
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal ProjectionParameter(ICatalog catalog)
            : base(catalog)
        { }

        public IEnumerable<Projection<TKey>> Projections
        {
            get
            {
                if (!_projectionsSet)
                    throw new ApplicationException("Search must be Executed before Projections are available.");

                return _projections;
            }
            private set
            {
                _projections = value;
                _projectionsSet = true;
            }
        }

        private IEnumerable<Projection<TKey>> _projections;
        private bool _projectionsSet = false;

        #region IProjectionParameter

        dynamic IProjectionParameter.DynamicProjections
        {
            get { return Projections; }
            set { Projections = value; }
        }

        #endregion
    }

    internal interface IProjectionParameter : IParameter
    {
        dynamic DynamicProjections { get; set; }
    }
}
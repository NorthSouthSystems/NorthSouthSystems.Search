using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    public sealed partial class Catalog<TKey> : ICatalog
      where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal Catalog(IEngine engine, string name)
        {
            _engine = engine;
            _name = name;
        }

        public Catalog(string name)
            : this(null, name)
        { }

        #region Rebuild

        void ICatalog.RebuildHotReadPhase(int[] bitPositionShifts)
        {
            foreach (Vector vector in _vectors.Values)
                vector.RebuildHotReadPhase(bitPositionShifts);
        }

        void ICatalog.RebuildHotWritePhase()
        {
            List<TKey> deadKeys = new List<TKey>();

            foreach (var kvp in _vectors)
                if (!kvp.Value.RebuildHotWritePhase())
                    deadKeys.Add(kvp.Key);

            foreach (TKey key in deadKeys)
            {
                _keys.Remove(key);
                _vectors.Remove(key);
            }
        }

        #endregion

        IEngine ICatalog.Engine { get { return _engine; } }
        private readonly IEngine _engine;

        public string Name { get { return _name; } }
        private readonly string _name;

        private SortedSet<TKey> _keys = new SortedSet<TKey>();
        private Dictionary<TKey, Vector> _vectors = new Dictionary<TKey, Vector>();

        #region Set

        public void Set(TKey key, int bitPosition, bool value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            Contract.EndContractBlock();

            Vector vector;

            if (!_vectors.TryGetValue(key, out vector))
            {
                vector = new Vector(true);
                _keys.Add(key);
                _vectors.Add(key, vector);
            }

            vector[bitPosition] = value;
        }

        public void Set(IEnumerable<TKey> keys, int bitPosition, bool value)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");

            Contract.EndContractBlock();

            foreach (TKey key in keys)
                Set(key, bitPosition, value);
        }

        #endregion

        #region Search

        public void Search(Vector vector, TKey key)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (key == null)
                throw new ArgumentNullException("key");

            Contract.EndContractBlock();

            SearchImpl(vector, new [] { Lookup(key) });
        }

        public void Search(Vector vector, IEnumerable<TKey> keys)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (keys == null)
                throw new ArgumentNullException("keys");

            if (keys.Any(key => key == null))
                throw new ArgumentNullException("keys", "All keys must be non-null.");

            Contract.EndContractBlock();

            SearchImpl(vector, keys.Distinct().Select(key => Lookup(key)));
        }

        public void Search(Vector vector, TKey keyMin, TKey keyMax)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (keyMin == null && keyMax == null)
                throw new ArgumentNullException("keyMin", "Either keyMin or keyMax must be non-null.");

            if (keyMin != null && keyMax != null && keyMin.CompareTo(keyMax) > 0)
                throw new ArgumentOutOfRangeException("keyMin", "keyMin must be <= keyMax.");

            Contract.EndContractBlock();

            if (keyMin == null)
                keyMin = _keys.Min;

            if (keyMax == null)
                keyMax = _keys.Max;

            SearchImpl(vector, _keys.Count == 0 ? new Vector[0] : _keys.GetViewBetween(keyMin, keyMax).Select(key => _vectors[key]));
        }

        private static void SearchImpl(Vector vector, IEnumerable<Vector> lookups)
        {
            Vector[] lookupsArray = lookups.Where(lookup => lookup != null).ToArray();

            if (lookupsArray.Length == 0)
                vector.WordsClear();
            else if (lookupsArray.Length == 1)
                vector.And(lookupsArray[0]);
            else
                vector.And(Vector.CreateUnion(lookupsArray));
        }

        private Vector Lookup(TKey key)
        {
            Vector vector;
            _vectors.TryGetValue(key, out vector);

            return vector;
        }

        #endregion

        #region Faceting

        public FacetCollection<TKey> Facets(Vector vector)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            Contract.EndContractBlock();

            var facets = _vectors
                .Select(keyAndVector => new Facet<TKey>(keyAndVector.Key, vector.AndPopulation(keyAndVector.Value)));

            return new FacetCollection<TKey>(facets);
        }

        #endregion

        #region Sort

        public CatalogSortResult<TKey> SortBitPositions(Vector vector, bool value, bool ascending)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            Contract.EndContractBlock();

            var keys = ascending ? _keys : _keys.Reverse();
            var partialSortResults = keys.Select(key => new CatalogPartialSortResult<TKey>(key, vector.AndFilterBitPositions(_vectors[key], value)));

            return new CatalogSortResult<TKey>(partialSortResults);
        }

        #endregion
    }

    internal interface ICatalog
    {
        IEngine Engine { get; }
        string Name { get; }

        void RebuildHotReadPhase(int[] bitPositionShifts);
        void RebuildHotWritePhase();

        ICatalogStatistics GenerateStatistics();
    }
}
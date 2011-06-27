using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    public partial class Catalog<TKey> : ICatalog
      where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        internal Catalog(IEngine engine, string name, bool isCompressed)
        {
            _engine = engine;
            _name = name;
            _isCompressed = isCompressed;
        }

        public Catalog(string name, bool isCompressed)
            : this(null, name, isCompressed)
        { }

        #region Rebuild

        void ICatalog.RebuildHotReadPhase(int[] bitPositionShifts)
        {
            foreach (Vector vector in _vectorSortedList.Values)
                vector.RebuildHotReadPhase(bitPositionShifts);
        }

        void ICatalog.RebuildHotWritePhase()
        {
            // PERF : Null the Dictionary instance member. In cases of local variables, I know that this behavior is unneccessary
            // because the garbage collector knows an instruction pointer for after which a given variable is no longer used.
            // For instances members, I do not know this to be the case. So, simply null the now unused parameter here to ensure that
            // it can be garbage collected if needed during this operation which will allocate a significant amount of memory.
            _vectorDictionary = null;

            SortedList<TKey, Vector> newVectorSortedList = new SortedList<TKey, Vector>();

            foreach (var kvp in _vectorSortedList)
                if (kvp.Value.RebuildHotWritePhase())
                    newVectorSortedList.Add(kvp.Key, kvp.Value);

            _vectorSortedList = newVectorSortedList;
            _vectorDictionary = new Dictionary<TKey, Vector>(_vectorSortedList);
        }

        #endregion

        IEngine ICatalog.Engine { get { return _engine; } }
        private readonly IEngine _engine;

        public string Name { get { return _name; } }
        private readonly string _name;

        public bool IsCompressed { get { return _isCompressed; } }
        private readonly bool _isCompressed;

        private Dictionary<TKey, Vector> _vectorDictionary = new Dictionary<TKey, Vector>();
        private SortedList<TKey, Vector> _vectorSortedList = new SortedList<TKey, Vector>();

        #region Set

        public void Set(TKey key, int bitPosition, bool value)
        {
            Vector vector;

            if (!_vectorDictionary.TryGetValue(key, out vector))
            {
                vector = new Vector(_isCompressed);
                _vectorDictionary.Add(key, vector);
                _vectorSortedList.Add(key, vector);
            }

            vector[bitPosition] = value;
        }

        public void Set(IEnumerable<TKey> keys, int bitPosition, bool value)
        {
            foreach (TKey key in keys)
                Set(key, bitPosition, value);
        }

        #endregion

        #region Search

        public void Search(Vector vector, TKey key) { SearchImpl(vector, Lookup(key).Select(kvp => kvp.Value)); }

        public void Search(Vector vector, IEnumerable<TKey> keys) { SearchImpl(vector, Lookup(keys).Select(kvp => kvp.Value)); }

        public void Search(Vector vector, TKey keyMin, TKey keyMax) { SearchImpl(vector, Lookup(keyMin, keyMax).Select(kvp => kvp.Value)); }

        private static void SearchImpl(Vector vector, IEnumerable<Vector> lookups)
        {
            Vector[] lookupsArray = lookups.ToArray();

            if (lookupsArray.Length == 0)
                vector.WordsClear();
            else if (lookupsArray.Length == 1)
                vector.And(lookupsArray[0]);
            else
                vector.And(Vector.CreateUnion(lookupsArray));
        }

        #endregion

        #region Projection

        public List<Projection<TKey>> Projection(Vector vector)
        {
            List<Projection<TKey>> projections = new List<Projection<TKey>>();

            IList<TKey> keys = _vectorSortedList.Keys;
            IList<Vector> values = _vectorSortedList.Values;

            for (int i = 0; i < values.Count; i++)
            {
                int population = vector.AndPopulation(values[i]);

                if (population > 0)
                    projections.Add(new Projection<TKey>(keys[i], population));
            }

            return projections;
        }

        #endregion

        #region Sort

        internal IEnumerable<KeyValuePair<dynamic, IEnumerable<int>>> SortBitPositionsDynamic(Vector vector, bool value, bool ascending)
        {
            return SortBitPositions(vector, value, ascending).Select(kvp => new KeyValuePair<dynamic, IEnumerable<int>>(kvp.Key, kvp.Value));
        }

        public IEnumerable<KeyValuePair<TKey, IEnumerable<int>>> SortBitPositions(Vector vector, bool value, bool ascending)
        {
            // Enumerable.Reverse buffers the entire sequence, regardless of runtime type (in this case an IList).
            // Using the indexer behavior on _vectorSortedList should offer a performance improvement; however,
            // this hypothesis has not been formally tested here.
            if (ascending)
            {
                for (int i = 0; i < _vectorSortedList.Count; i++)
                    yield return new KeyValuePair<TKey, IEnumerable<int>>(_vectorSortedList.Keys[i], vector.AndFilterBitPositions(_vectorSortedList.Values[i], value));
            }
            else
            {
                for (int i = _vectorSortedList.Count - 1; i >= 0; i--)
                    yield return new KeyValuePair<TKey, IEnumerable<int>>(_vectorSortedList.Keys[i], vector.AndFilterBitPositions(_vectorSortedList.Values[i], value));
            }
        }

        #endregion

        #region Lookup

        private IEnumerable<KeyValuePair<TKey, Vector>> Lookup(TKey key) { return Lookup(new TKey[1] { key }); }

        private IEnumerable<KeyValuePair<TKey, Vector>> Lookup(IEnumerable<TKey> keys)
        {
            foreach (TKey key in keys)
            {
                Vector vector;
                _vectorDictionary.TryGetValue(key, out vector);

                if (vector != null)
                    yield return new KeyValuePair<TKey, Vector>(key, vector);
            }
        }

        private IEnumerable<KeyValuePair<TKey, Vector>> Lookup(TKey keyMin, TKey keyMax)
        {
            IList<TKey> keys = _vectorSortedList.Keys;

            int indexMin = keys.BinarySearch(keyMin);

            if (indexMin < 0)
                indexMin = ~indexMin;

            int indexMax = keys.BinarySearch(keyMax);

            if (indexMax < 0)
                indexMax = ~indexMax - 1;

            return _vectorSortedList.Skip(indexMin).Take(indexMax - indexMin + 1);
        }

        #endregion
    }

    internal interface ICatalog
    {
        IEngine Engine { get; }
        string Name { get; }
        bool IsCompressed { get; }
        
        void RebuildHotReadPhase(int[] bitPositionShifts);
        void RebuildHotWritePhase();

        ICatalogStatistics GetStatistics();
    }
}
﻿using System.Linq;

namespace SoftwareBotany.Sunlight
{
    public partial class Catalog<TKey>
    {
        public ICatalogStatistics GetStatistics() { return new Statistics(this); }

        private class Statistics : ICatalogStatistics
        {
            internal Statistics(Catalog<TKey> catalog)
            {
                foreach (IVectorStatistics vectorStats in catalog._vectorSortedList.Values.Select(vector => vector.GetStatistics()))
                {
                    _vectorCount++;
                    _wordCount += vectorStats.WordCount;
                    _oneBitPackableWordCount += vectorStats.OneBitPackableWordCount;
                    _twoBitPackableWordCount += vectorStats.TwoBitPackableWordCount;
                }
            }

            private readonly int _vectorCount;
            public int VectorCount { get { return _vectorCount; } }

            private readonly int _wordCount;
            public int WordCount { get { return _wordCount; } }

            private readonly int _oneBitPackableWordCount;
            public int OneBitPackableWordCount { get { return _oneBitPackableWordCount; } }

            private readonly int _twoBitPackableWordCount;
            public int TwoBitPackableWordCount { get { return _twoBitPackableWordCount; } }
        }
    }

    public interface ICatalogStatistics
    {
        int VectorCount { get; }
        int WordCount { get; }
        int OneBitPackableWordCount { get; }
        int TwoBitPackableWordCount { get; }
    }
}
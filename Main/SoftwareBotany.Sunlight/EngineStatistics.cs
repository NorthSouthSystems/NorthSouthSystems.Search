using System.Linq;

namespace SoftwareBotany.Sunlight
{
    public partial class Engine<TItem, TPrimaryKey>
    {
        public IEngineStatistics GenerateStatistics()
        {
            try
            {
                _rwLock.EnterReadLock();
                return new Statistics(this);
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        private class Statistics : IEngineStatistics
        {
            internal Statistics(Engine<TItem, TPrimaryKey> engine)
            {
                foreach (ICatalogStatistics catalogStats in engine._catalogsPlusExtractors.Select(cpe => cpe.Catalog.GenerateStatistics()))
                {
                    _catalogCount++;
                    _vectorCount += catalogStats.VectorCount;
                    _wordCount += catalogStats.WordCount;
                    _oneBitPackableWordCount += catalogStats.OneBitPackableWordCount;
                    _twoBitPackableWordCount += catalogStats.TwoBitPackableWordCount;
                }
            }

            private readonly int _catalogCount;
            public int CatalogCount { get { return _catalogCount; } }

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

    public interface IEngineStatistics
    {
        int CatalogCount { get; }
        int VectorCount { get; }
        int WordCount { get; }
        int OneBitPackableWordCount { get; }
        int TwoBitPackableWordCount { get; }
    }
}
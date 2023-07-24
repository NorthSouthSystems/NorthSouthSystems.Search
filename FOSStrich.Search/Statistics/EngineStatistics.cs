namespace FOSStrich.Search;

public sealed partial class Engine<TItem, TPrimaryKey>
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

    private sealed class Statistics : IEngineStatistics
    {
        internal Statistics(Engine<TItem, TPrimaryKey> engine)
        {
            foreach (ICatalogStatistics catalogStats in engine._catalogsPlusExtractors.Select(cpe => cpe.Catalog.GenerateStatistics()))
            {
                _catalogCount++;
                _vectorCount += catalogStats.VectorCount;
                _wordCount += catalogStats.WordCount;
                _packedWordCount += catalogStats.PackedWordCount;
                _oneBitPackableWordCount += catalogStats.OneBitPackableWordCount;
                _twoBitPackableWordCount += catalogStats.TwoBitPackableWordCount;
            }
        }

        private readonly int _catalogCount;
        public int CatalogCount => _catalogCount;

        private readonly int _vectorCount;
        public int VectorCount => _vectorCount;

        private readonly int _wordCount;
        public int WordCount => _wordCount;

        private readonly int _packedWordCount;
        public int PackedWordCount => _packedWordCount;

        private readonly int _oneBitPackableWordCount;
        public int OneBitPackableWordCount => _oneBitPackableWordCount;

        private readonly int _twoBitPackableWordCount;
        public int TwoBitPackableWordCount => _twoBitPackableWordCount;
    }
}

public interface IEngineStatistics
{
    int CatalogCount { get; }
    int VectorCount { get; }
    int WordCount { get; }
    int PackedWordCount { get; }
    int OneBitPackableWordCount { get; }
    int TwoBitPackableWordCount { get; }
}
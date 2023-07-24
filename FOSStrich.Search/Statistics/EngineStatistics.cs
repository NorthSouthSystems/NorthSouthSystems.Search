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
                CatalogCount++;
                VectorCount += catalogStats.VectorCount;
                WordCount += catalogStats.WordCount;
                PackedWordCount += catalogStats.PackedWordCount;
                OneBitPackableWordCount += catalogStats.OneBitPackableWordCount;
                TwoBitPackableWordCount += catalogStats.TwoBitPackableWordCount;
            }
        }

        public int CatalogCount { get; }
        public int VectorCount { get; }
        public int WordCount { get; }
        public int PackedWordCount { get; }
        public int OneBitPackableWordCount { get; }
        public int TwoBitPackableWordCount { get; }
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
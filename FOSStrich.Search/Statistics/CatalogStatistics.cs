namespace FOSStrich.Search;

public sealed partial class Catalog<TKey>
{
    public ICatalogStatistics GenerateStatistics() => new Statistics(this);

    private sealed class Statistics : ICatalogStatistics
    {
        internal Statistics(Catalog<TKey> catalog)
        {
            foreach (IVectorStatistics vectorStats in catalog._keyToEntryMap.Values.Select(entry => entry.Vector.GenerateStatistics()))
            {
                _vectorCount++;
                _wordCount += vectorStats.WordCount;
                _packedWordCount += vectorStats.PackedWordCount;
                _oneBitPackableWordCount += vectorStats.OneBitPackableWordCount;
                _twoBitPackableWordCount += vectorStats.TwoBitPackableWordCount;
            }
        }

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

public interface ICatalogStatistics
{
    int VectorCount { get; }
    int WordCount { get; }
    int PackedWordCount { get; }
    int OneBitPackableWordCount { get; }
    int TwoBitPackableWordCount { get; }
}
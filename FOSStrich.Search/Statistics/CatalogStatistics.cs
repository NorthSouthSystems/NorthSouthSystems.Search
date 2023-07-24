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
                VectorCount++;
                WordCount += vectorStats.WordCount;
                PackedWordCount += vectorStats.PackedWordCount;
                OneBitPackableWordCount += vectorStats.OneBitPackableWordCount;
                TwoBitPackableWordCount += vectorStats.TwoBitPackableWordCount;
            }
        }

        public int VectorCount { get; }
        public int WordCount { get; }
        public int PackedWordCount { get; }
        public int OneBitPackableWordCount { get; }
        public int TwoBitPackableWordCount { get; }
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
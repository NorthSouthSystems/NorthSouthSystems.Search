namespace FOSStrich.Search;

public sealed partial class Vector
{
    public IVectorStatistics GenerateStatistics() => new Statistics(this);

    private sealed class Statistics : IVectorStatistics
    {
        internal Statistics(Vector vector)
        {
            WordCount = vector._wordCountPhysical;

            bool lastWordCompressed = false;

            for (int i = 0; i < vector._wordCountPhysical; i++)
            {
                Word word = vector._words[i];

                if (word.HasPackedWord)
                    PackedWordCount++;

                if (!word.IsCompressed && lastWordCompressed && i < vector._wordCountPhysical - 1)
                {
                    if (word.Population == 1)
                        OneBitPackableWordCount++;
                    else if (word.Population == 2)
                        TwoBitPackableWordCount++;
                }

                lastWordCompressed = word.IsCompressed;
            }
        }

        public int WordCount { get; }
        public int PackedWordCount { get; }
        public int OneBitPackableWordCount { get; }
        public int TwoBitPackableWordCount { get; }
    }
}

public interface IVectorStatistics
{
    int WordCount { get; }
    int PackedWordCount { get; }
    int OneBitPackableWordCount { get; }
    int TwoBitPackableWordCount { get; }
}
namespace FOSStrich.Search;

public sealed partial class Vector
{
    public IVectorStatistics GenerateStatistics() => new Statistics(this);

    private sealed class Statistics : IVectorStatistics
    {
        internal Statistics(Vector vector)
        {
            _wordCount = vector._wordCountPhysical;

            bool lastWordCompressed = false;

            for (int i = 0; i < vector._wordCountPhysical; i++)
            {
                Word word = vector._words[i];

                if (word.HasPackedWord)
                    _packedWordCount++;

                if (!word.IsCompressed && lastWordCompressed && i < vector._wordCountPhysical - 1)
                {
                    if (word.Population == 1)
                        _oneBitPackableWordCount++;
                    else if (word.Population == 2)
                        _twoBitPackableWordCount++;
                }

                lastWordCompressed = word.IsCompressed;
            }
        }

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

public interface IVectorStatistics
{
    int WordCount { get; }
    int PackedWordCount { get; }
    int OneBitPackableWordCount { get; }
    int TwoBitPackableWordCount { get; }
}
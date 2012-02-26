namespace SoftwareBotany.Sunlight
{
    public sealed partial class Vector
    {
        public IVectorStatistics GenerateStatistics() { return new Statistics(this); }

        private sealed class Statistics : IVectorStatistics
        {
            internal Statistics(Vector vector)
            {
                _wordCount = vector._wordCountPhysical;

                bool lastWordCompressed = false;

                for (int i = 0; i < vector._wordCountPhysical; i++)
                {
                    Word word = vector._words[i];

#if POSITIONLIST
                    if (word.HasPackedWord)
                        _packedWordCount++;
#else
                    _packedWordCount = 0;
#endif

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
            public int WordCount { get { return _wordCount; } }

            private readonly int _packedWordCount;
            public int PackedWordCount { get { return _packedWordCount; } }

            private readonly int _oneBitPackableWordCount;
            public int OneBitPackableWordCount { get { return _oneBitPackableWordCount; } }

            private readonly int _twoBitPackableWordCount;
            public int TwoBitPackableWordCount { get { return _twoBitPackableWordCount; } }
        }
    }

    public interface IVectorStatistics
    {
        int WordCount { get; }
        int PackedWordCount { get; }
        int OneBitPackableWordCount { get; }
        int TwoBitPackableWordCount { get; }
    }
}
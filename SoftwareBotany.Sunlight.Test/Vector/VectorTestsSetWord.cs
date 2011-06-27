using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsSetWord
    {
        [TestMethod]
        public void Uncompressed()
        {
            Vector vector = new Vector(false);

            vector.AssertWordLogicalValues(0, 0, 0);
            vector.AssertWordCounts(1, 1);

            // Ignore 0 Sets When ZeroFilling required
            vector.Set(1, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 0);
            vector.AssertWordCounts(1, 1);

            vector.Set(1, new Word(1));
            vector.AssertWordLogicalValues(0, 1, 0);
            vector.AssertWordCounts(2, 2);

            vector.Set(1, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 0);
            vector.AssertWordCounts(2, 2);

            vector.Set(2, new Word(1));
            vector.AssertWordLogicalValues(0, 0, 1);
            vector.AssertWordCounts(3, 3);

            vector.Set(0, new Word(1));
            vector.AssertWordLogicalValues(1, 0, 1);
            vector.AssertWordCounts(3, 3);

            vector.Set(1, new Word(1));
            vector.AssertWordLogicalValues(1, 1, 1);
            vector.AssertWordCounts(3, 3);

            vector.Set(0, new Word(0));
            vector.AssertWordLogicalValues(0, 1, 1);
            vector.AssertWordCounts(3, 3);

            vector.Set(1, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 1);
            vector.AssertWordCounts(3, 3);

            vector.Set(2, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 0);
            vector.AssertWordCounts(3, 3);
        }

        [TestMethod]
        public void CompressedZeroFillCodeCoverage()
        {
            Vector vector = new Vector(true);

            vector.AssertWordLogicalValues(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(1, 1);

            // Ignore 0 Sets When ZeroFilling required
            vector.Set(1, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(1, 1);

            // Force Compression of Word[0]
            vector.Set(1, new Word(1));
            vector.AssertWordLogicalValues(0, 1, 0, 0, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(2, 2);

            vector.Set(1, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(2, 2);

            // Increment Compression on Word[0]
            vector.Set(2, new Word(1));
            vector.AssertWordLogicalValues(0, 0, 1, 0, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(2, 3);

            vector.Set(2, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(2, 3);

            // End the 0's and have the tail compressed
            vector.Set(3, new Word(1));
            vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(2, 4);

            // Add a 1 and pack the tail
            vector.Set(4, new Word(1));
            vector.AssertWordLogicalValues(0, 0, 0, 1, 1, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(Word.PositionListEnabled ? 2 : 3, 5);

            // Add a 0 Word
            vector.Set(4, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(Word.PositionListEnabled ? 2 : 3, 5);

            // Add a 1 Word far away, forcing a compression
            vector.Set(7, new Word(1));
            vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 1, 0, 0);
            vector.AssertWordCounts(Word.PositionListEnabled ? 3 : 4, 8);

            // Add a 1 Word two spaces away, forcing a pack and 2xZeroFill with overwrite
            vector.Set(9, new Word(1));
            vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 1, 0, 1);
            vector.AssertWordCounts(Word.PositionListEnabled ? 4 : 6, 10);
        }
    }
}
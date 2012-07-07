using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsGetSetWord
    {
        [TestMethod]
        public void Uncompressed()
        {
            Vector vector = new Vector(false, VectorCompression.None);

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
        public void CompressedZeroFillCodeCoverage() { SafetyVectorCompressionTuple.RunAll(CompressedZeroFillCodeCoverageBase); }

        private void CompressedZeroFillCodeCoverageBase(SafetyVectorCompressionTuple safetyVectorCompression)
        {
            if (safetyVectorCompression.Compression == VectorCompression.None)
                return;

            Vector vector = new Vector(safetyVectorCompression.AllowUnsafe, safetyVectorCompression.Compression);

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
            vector.AssertWordCounts(vector.IsPackedPositionEnabled ? 2 : 3, 5);

            // Add a 0 Word
            vector.Set(4, new Word(0));
            vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            vector.AssertWordCounts(vector.IsPackedPositionEnabled ? 2 : 3, 5);

            // Add a 1 Word far away, forcing a compression
            vector.Set(7, new Word(1));
            vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 1, 0, 0);
            vector.AssertWordCounts(vector.IsPackedPositionEnabled ? 3 : 4, 8);

            // Add a 1 Word two spaces away, forcing a pack and 2xZeroFill with overwrite
            vector.Set(9, new Word(1));
            vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 1, 0, 1);
            vector.AssertWordCounts(vector.IsPackedPositionEnabled ? 4 : 6, 10);
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetWordLogicalArgumentOutOfRange()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            vector.GetWordLogical(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetWordArgumentOutOfRange()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            vector.Set(-1, new Word(0x11111111));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetWordNotSupportedCompressed()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            vector.Set(0, new Word(true, 1));
        }

        [TestMethod]
        public void SetWordSupportedForwardOnly()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            vector[30] = true;
            vector.Set(0, new Word(0x00000001u));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetWordNotSupportedForwardOnly()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            vector[31] = true;
            vector.Set(0, new Word(0x00000001u));
        }

        #endregion
    }
}
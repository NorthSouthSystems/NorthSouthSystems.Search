namespace Kangarooper.Search
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WordTestsIndexersAndBits
    {
        [TestMethod]
        public void GetSetPositionsSimple()
        {
            Word word = new Word();

            for (int i = 0; i < Word.SIZE - 1; i++)
            {
                word[i] = true;

                for (int j = 0; j < Word.SIZE - 1; j++)
                    Assert.AreEqual(i == j, word[j]);

                word[i] = false;

                for (int j = 0; j < Word.SIZE - 1; j++)
                    Assert.AreEqual(false, word[j]);
            }
        }

        [TestMethod]
        public void BitsSimple()
        {
            int[] bitPositions = new int[] { 0, 3, 8, 12, 19, 24, 30 };
            BitsBase(bitPositions, true);
            BitsBase(bitPositions, false);
        }

        private void BitsBase(int[] bitPositions, bool value)
        {
            Word word = new Word();

            for (int i = 0; i < Word.SIZE - 1; i++)
                word[i] = bitPositions.Contains(i) ? value : !value;

            bool[] bits = word.Bits;

            Assert.AreEqual(bitPositions.Length, bits.Count(bit => value ? bit : !bit));

            for (int i = 0; i < Word.SIZE - 1; i++)
                Assert.AreEqual(bitPositions.Contains(i), value ? bits[i] : !bits[i]);
        }

        [TestMethod]
        public void GetBitPositionsSimple()
        {
            int[] bitPositions = new int[] { 0, 3, 8, 12, 19, 24, 30 };
            GetBitPositionsBase(bitPositions, true);
            GetBitPositionsBase(bitPositions, false);
        }

        private void GetBitPositionsBase(int[] bitPositions, bool value)
        {
            Word word = new Word();

            for (int i = 0; i < Word.SIZE - 1; i++)
                word[i] = bitPositions.Contains(i) ? value : !value;

            int[] getBitPositions = word.GetBitPositions(value);

            Assert.AreEqual(bitPositions.Length, getBitPositions.Length);

            for (int i = 0; i < bitPositions.Length; i++)
                Assert.AreEqual(bitPositions[i], getBitPositions[i]);
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ComputeIndexerMaskNotSupported()
        {
            Word word = new Word(true, 1);
            bool bit = word[0];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ComputeIndexerMaskArgumentOutOfRange1()
        {
            Word word = new Word(0);
            bool bit = word[-1];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ComputeIndexerMaskArgumentOutOfRange2()
        {
            Word word = new Word(0);
            bool bit = word[Word.SIZE - 1];
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void BitsNotSupported()
        {
            Word word = new Word(true, 1);
            bool[] bits = word.Bits;
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void GetBitPositionsNotSupported()
        {
            Word word = new Word(true, 1);
            int[] bitPositions = word.GetBitPositions(true);
        }

        #endregion
    }
}
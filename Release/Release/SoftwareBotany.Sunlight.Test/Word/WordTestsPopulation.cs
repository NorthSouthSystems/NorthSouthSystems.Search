using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class WordTestsPopulation
    {
        [TestMethod]
        public void NotCompressed()
        {
            Assert.AreEqual(0, (new Word(0x00000000u)).Population);
            Assert.AreEqual(31, (new Word(0x7FFFFFFFu)).Population);
            Assert.AreEqual(1, (new Word(0x00000001u)).Population);
            Assert.AreEqual(1, (new Word(0x40000000u)).Population);
            Assert.AreEqual(30, (new Word(0x7FFFFFFEu)).Population);
            Assert.AreEqual(30, (new Word(0x3FFFFFFFu)).Population);
            Assert.AreEqual(13, (new Word(0x12345678u)).Population);
            Assert.AreEqual(22, (new Word(0x7FEDCBA9u)).Population);

            for (int i = 0; i < Word.SIZE - 1; i++)
                Assert.AreEqual(1, (new Word(1u << i)).Population);
        }

        [TestMethod]
        public void CompressedFillBitFalseNoFill() { CompressedBase(false, 0x00000000); }

        [TestMethod]
        public void CompressedFillBitFalse1Fill() { CompressedBase(false, 0x00000001); }

        [TestMethod]
        public void CompressedFillBitFalseMaxFill() { CompressedBase(false, 0x01FFFFFF); }

        [TestMethod]
        public void CompressedFillBitTrueNoFill() { CompressedBase(true, 0x00000000); }

        [TestMethod]
        public void CompressedFillBitTrue1Fill() { CompressedBase(true, 0x00000001); }

        [TestMethod]
        public void CompressedFillBitTrueMaxFill() { CompressedBase(true, 0x01FFFFFF); }

        private void CompressedBase(bool fillBit, int fillCount)
        {
            Word word = new Word(fillBit, fillCount);
            Assert.AreEqual(fillBit ? (31 * fillCount) : 0, word.Population);
        }

        [TestMethod]
        public void CompressedFullCoverage()
        {
            foreach (bool fillBit in new bool[] { false, true })
            {
                for (int i = 0; i < 0x02000000; i += WordTestExtensions.LARGEPRIME)
                {
                    Word word = new Word(fillBit, i);
                    Assert.AreEqual(fillBit ? (31 * i) : 0, word.Population, word.ToString());
                }
            }
        }

#if POSITIONLIST
        [TestMethod]
        public void Packed()
        {
            Word word = new Word(false, 1);
            Assert.AreEqual(0, word.Population);
            word.Pack(new Word(1));
            Assert.AreEqual(1, word.Population);

            word = new Word(true, 1);
            Assert.AreEqual(Word.SIZE - 1, word.Population);
            word.Pack(new Word(1));
            Assert.AreEqual(Word.SIZE, word.Population);
        }
#endif
    }
}
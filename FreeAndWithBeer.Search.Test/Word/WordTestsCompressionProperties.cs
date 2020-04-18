namespace FreeAndWithBeer.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WordTestsCompressionProperties
    {
        [TestMethod]
        public void IsCompressibleTrue()
        {
            Word word = new Word(0x00000000u);
            Assert.AreEqual(true, word.IsCompressible);
            Assert.AreEqual(false, word.CompressibleFillBit);
            Assert.AreEqual(false, word.IsCompressed);

            word = new Word(Word.COMPRESSIBLEMASK);
            Assert.AreEqual(true, word.IsCompressible);
            Assert.AreEqual(true, word.CompressibleFillBit);
            Assert.AreEqual(false, word.IsCompressed);
        }

        [TestMethod]
        public void IsCompressibleFalse()
        {
            foreach (uint u in new uint[] { 0x00000001u, 0x40000000u, 0x7FFFFFFEu, 0x3FFFFFFFu, 0x12345678u, 0x7FEDCBA9u })
            {
                Word word = new Word(u);
                Assert.AreEqual(false, word.IsCompressible);
                Assert.AreEqual(false, word.IsCompressed);
            }
        }

        [TestMethod]
        public void IsCompressibleFalseFullCoverage()
        {
            for (uint i = 1; i < 0x7FFFFFFFu; i += WordExtensions.LARGEPRIME)
            {
                Word word = new Word(i);
                Assert.AreEqual(false, word.IsCompressible);
                Assert.AreEqual(false, word.IsCompressed);
            }
        }

        [TestMethod]
        public void CompressedFillBitFalseNoFill() { CompressedBase(false, 0x00000000, 0x80000000u); }

        [TestMethod]
        public void CompressedFillBitFalse1Fill() { CompressedBase(false, 0x00000001, 0x80000001u); }

        [TestMethod]
        public void CompressedFillBitFalseMaxFill() { CompressedBase(false, 0x01FFFFFF, 0x81FFFFFFu); }

        [TestMethod]
        public void CompressedFillBitTrueNoFill() { CompressedBase(true, 0x00000000, 0xC0000000u); }

        [TestMethod]
        public void CompressedFillBitTrue1Fill() { CompressedBase(true, 0x00000001, 0xC0000001u); }

        [TestMethod]
        public void CompressedFillBitTrueMaxFill() { CompressedBase(true, 0x01FFFFFF, 0xC1FFFFFFu); }

        private void CompressedBase(bool fillBit, int fillCount, uint wordValue)
        {
            Word word = new Word(fillBit, fillCount);
            Assert.AreEqual(wordValue, word.Raw);
            Assert.AreEqual(true, word.IsCompressed);
            Assert.AreEqual(fillBit, word.FillBit);
            Assert.AreEqual(fillCount, word.FillCount);
        }

        [TestMethod]
        public void CompressedFullCoverage()
        {
            foreach (bool fillBit in new bool[] { false, true })
            {
                for (int i = 0; i < 0x02000000; i += WordExtensions.LARGEPRIME)
                {
                    Word word = new Word(fillBit, i);
                    Assert.AreEqual((fillBit ? 0xC0000000 : 0x80000000) + i, word.Raw);
                    Assert.AreEqual(true, word.IsCompressed, word.ToString());
                    Assert.AreEqual(fillBit, word.FillBit, word.ToString());
                    Assert.AreEqual(i, word.FillCount, word.ToString());
                }
            }
        }
    }
}
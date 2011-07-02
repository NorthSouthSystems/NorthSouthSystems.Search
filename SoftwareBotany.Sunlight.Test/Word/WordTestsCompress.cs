using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class WordTestsCompress
    {
        [TestMethod]
        public void Compressible()
        {
            Word word = new Word(0);
            Assert.AreEqual(true, word.IsCompressible);
            word.Compress();
            Assert.AreEqual(0x80000001, word.Raw);
            Assert.AreEqual(true, word.IsCompressed);
            Assert.AreEqual(false, word.FillBit);
            Assert.AreEqual(1, word.FillCount);

            word = new Word(Word.COMPRESSIBLEMASK);
            Assert.AreEqual(true, word.IsCompressible);
            word.Compress();
            Assert.AreEqual(0xC0000001, word.Raw);
            Assert.AreEqual(true, word.IsCompressed);
            Assert.AreEqual(true, word.FillBit);
            Assert.AreEqual(1, word.FillCount);
        }

        [TestMethod]
        public void NotCompressible()
        {
            foreach (uint wordValue in new uint[] { 0x00000001u, 0x40000000u, 0x7FFFFFFEu, 0x3FFFFFFFu, 0x12345678u, 0x7FEDCBA9u })
            {
                Word word = new Word(wordValue);
                word.Compress();
                Assert.AreEqual(wordValue, word.Raw);
                Assert.AreEqual(false, word.IsCompressible);
                Assert.AreEqual(false, word.IsCompressed);
            }
        }

        [TestMethod]
        public void NotCompressibleFullCoverage()
        {
            for (uint i = 1; i < 0x7FFFFFFF; i += WordTestExtensions.LARGEPRIME)
            {
                Word word = new Word(i);
                word.Compress();
                Assert.AreEqual(i, word.Raw);
                Assert.AreEqual(false, word.IsCompressible);
                Assert.AreEqual(false, word.IsCompressed);
            }
        }

        [TestMethod]
        public void CompressedFullCoverage()
        {
            for (uint i = 0x80000000; i > 0x80000000 && i <= 0xFFFFFFFF; i += WordTestExtensions.LARGEPRIME)
            {
                Word word = new Word(i);
                word.Compress();
                Assert.AreEqual(i, word.Raw, word.ToString());
                Assert.AreEqual(true, word.IsCompressed, word.ToString());
            }
        }

#if POSITIONLISTENABLED
        [TestMethod]
        public void Pack()
        {
            Word word = new Word(true, 1);

            Assert.IsTrue(word.IsCompressed);
            Assert.IsTrue(word.FillBit);
            Assert.AreEqual(1, word.FillCount);
            Assert.IsFalse(word.HasPackedWord);

            word.Pack(new Word(1));

            Assert.IsTrue(word.IsCompressed);
            Assert.IsTrue(word.FillBit);
            Assert.AreEqual(1, word.FillCount);
            Assert.IsTrue(word.HasPackedWord);
            Assert.AreEqual(30, word.PackedPosition);
            Assert.AreEqual((uint)1, word.PackedWord.Raw);

            word = new Word(true, 1);

            Assert.IsTrue(word.IsCompressed);
            Assert.IsTrue(word.FillBit);
            Assert.AreEqual(1, word.FillCount);
            Assert.IsFalse(word.HasPackedWord);

            word.Pack(new Word(1 << 30));

            Assert.IsTrue(word.IsCompressed);
            Assert.IsTrue(word.FillBit);
            Assert.AreEqual(1, word.FillCount);
            Assert.IsTrue(word.HasPackedWord);
            Assert.AreEqual(0, word.PackedPosition);
            Assert.AreEqual((uint)1 << 30, word.PackedWord.Raw);
        }
#endif

        #region Exceptions

#if POSITIONLISTENABLED
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PackNotSupported1()
        {
            Word word = new Word(0);
            word.Pack(new Word(1));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PackNotSupported2()
        {
            Word word = new Word(true, 1);
            word.Pack(new Word(true, 1));
        }
#endif

        #endregion
    }
}
namespace FreeAndWithBeer.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class WordTestsConstruction
    {
        [TestMethod]
        public void Bounds()
        {
            Word word = new Word(0);
            word = new Word(Word.COMPRESSEDMASK - 1);
        }

        [TestMethod]
        public void Compressed()
        {
            Word word = new Word(false, 0);
            Assert.AreEqual(false, word.FillBit);
            Assert.AreEqual(0, word.FillCount);

            word = new Word(false, 1);
            Assert.AreEqual(false, word.FillBit);
            Assert.AreEqual(1, word.FillCount);

            word = new Word(false, 22);
            Assert.AreEqual(false, word.FillBit);
            Assert.AreEqual(22, word.FillCount);

            word = new Word(true, 0);
            Assert.AreEqual(true, word.FillBit);
            Assert.AreEqual(0, word.FillCount);

            word = new Word(true, 1);
            Assert.AreEqual(true, word.FillBit);
            Assert.AreEqual(1, word.FillCount);

            word = new Word(true, 22);
            Assert.AreEqual(true, word.FillBit);
            Assert.AreEqual(22, word.FillCount);
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RawArgumentOutOfRange()
        {
            Word word = new Word(Word.COMPRESSEDMASK);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FillCountOutOfRange1()
        {
            Word word = new Word(true, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FillCountOutOfRange2()
        {
            Word word = new Word(true, 0x02000000);
        }

        #endregion
    }
}
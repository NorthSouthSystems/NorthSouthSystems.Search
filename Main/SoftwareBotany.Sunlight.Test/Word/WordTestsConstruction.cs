using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class WordTestsConstruction
    {
        [TestMethod]
        public void Bounds()
        {
            Word word = new Word(0);
            word = new Word(Word.COMPRESSEDMASK - 1);
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
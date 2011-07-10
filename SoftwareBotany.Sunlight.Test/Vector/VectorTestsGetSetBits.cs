using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsGetSetBits
    {
        [TestMethod]
        public void Full()
        {
            Vector vector = new Vector(true);
            int[] bitPositions = vector.RandomFill(1000, 100);
            vector[2000] = false;
            vector.AssertBitPositions(bitPositions);
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexArgumentOutOfRange1()
        {
            Vector vector = new Vector(true);
            vector[-1] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexArgumentOutOfRange2()
        {
            Vector vector = new Vector(true);
            bool value = vector[-1];
        }

        [TestMethod]
        public void SetBitSupportedForwardOnly()
        {
            Vector vector = new Vector(true);
            vector[30] = true;
            vector[61] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetBitNotSupportedForwardOnly()
        {
            Vector vector = new Vector(true);
            vector[30] = true;
            vector[31] = true;
            vector[30] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void GetBitsCompressedNotSupported()
        {
            Vector vector = new Vector(true);
            vector.Bits.ToArray();
        }

        #endregion
    }
}
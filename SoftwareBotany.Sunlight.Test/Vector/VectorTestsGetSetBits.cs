namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VectorTestsGetSetBits
    {
        [TestMethod]
        public void Full() { SafetyVectorCompressionTuple.RunAll(FullBase); }

        private static void FullBase(SafetyVectorCompressionTuple safetyVectorCompression)
        {
            Vector vector = new Vector(safetyVectorCompression.AllowUnsafe, safetyVectorCompression.Compression);
            int[] bitPositions = vector.RandomFill(1000, 100);
            vector[2000] = false;
            vector.AssertBitPositions(bitPositions);
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexArgumentOutOfRange1()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            vector[-1] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexArgumentOutOfRange2()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            bool value = vector[-1];
        }

        [TestMethod]
        public void SetBitSupportedForwardOnly()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            vector[30] = true;
            vector[61] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetBitNotSupportedForwardOnly()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            vector[30] = true;
            vector[31] = true;
            vector[30] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void GetBitsCompressedNotSupported()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            vector.Bits.ToArray();
        }

        #endregion
    }
}
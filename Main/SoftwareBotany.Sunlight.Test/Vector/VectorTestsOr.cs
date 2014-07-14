namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VectorTestsOr
    {
        [TestMethod]
        public void OrCompressedWithCompressedTrueInput() { SafetyVectorCompressionTuple.RunAll(OrCompressedWithCompressedTrueInputBase); }

        private static void OrCompressedWithCompressedTrueInputBase(SafetyVectorCompressionTuple safetyVectorCompression)
        {
            Vector vector = new Vector(safetyVectorCompression.AllowUnsafe, VectorCompression.None);
            vector[100] = true;

            Vector compressedTrue = new Vector(safetyVectorCompression.AllowUnsafe, safetyVectorCompression.Compression);
            compressedTrue.Fill(Enumerable.Range(0, 32).ToArray(), true);

            vector.Or(compressedTrue);

            vector.AssertBitPositions(Enumerable.Range(0, 32), new[] { 100 });
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrArgumentNull()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            vector.Or(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void OrNotSupported()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            Vector input = new Vector(false, VectorCompression.None);
            vector.Or(input);
        }

        #endregion
    }
}
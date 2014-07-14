namespace SoftwareBotany.Sunlight
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VectorTestsStatics
    {
        [TestMethod]
        public void CreateUnion() { SafetyVectorCompressionTuple.RunAll(CreateUnionBase); }

        private static void CreateUnionBase(SafetyVectorCompressionTuple safetyVectorCompression)
        {
            int[] bitPositionsA = new int[] { 0, 12, 16, 22, 34, 55, 110 };
            int[] bitPositionsB = new int[] { 0, 11, 16, 23, 34, 54, 110, 120 };
            int[] bitPositionsC = new int[] { 5, 10, 15, 20 };

            Vector vectorA = new Vector(safetyVectorCompression.AllowUnsafe, safetyVectorCompression.Compression);
            vectorA.Fill(bitPositionsA, true);

            Vector vectorB = new Vector(safetyVectorCompression.AllowUnsafe, safetyVectorCompression.Compression);
            vectorB.Fill(bitPositionsB, true);

            Vector vectorC = new Vector(safetyVectorCompression.AllowUnsafe, safetyVectorCompression.Compression);
            vectorC.Fill(bitPositionsC, true);

            Vector.CreateUnion(vectorA, vectorB).AssertBitPositions(bitPositionsA, bitPositionsB);
            Vector.CreateUnion(vectorA, vectorB, vectorC).AssertBitPositions(bitPositionsA, bitPositionsB, bitPositionsC);
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateUnionArgumentNull1()
        {
            Vector[] vectors = null;
            Vector.CreateUnion(vectors);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateUnionArgumentNull2()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            Vector.CreateUnion(vector, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateUnionArgumentOutOfRange1()
        {
            Vector vector = Vector.CreateUnion();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateUnionArgumentOutOfRange2()
        {
            Vector vector1 = new Vector(false, VectorCompression.None);
            Vector vector = Vector.CreateUnion(vector1);
        }

        #endregion
    }
}
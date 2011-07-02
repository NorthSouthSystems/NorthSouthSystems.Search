using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsStatics
    {
        [TestMethod]
        public void CreateUnion()
        {
            Vector result = Vector.CreateUnion();
            Assert.AreEqual(0, result.Population);

            int[] bitPositionsA = new int[] { 0, 12, 16, 22, 34, 55, 110 };
            int[] bitPositionsB = new int[] { 0, 11, 16, 23, 34, 54, 110, 120 };
            int[] bitPositionsC = new int[] { 5, 10, 15, 20 };

            Vector vectorA = new Vector(false);
            vectorA.Fill(bitPositionsA, true);

            Vector vectorB = new Vector(false);
            vectorB.Fill(bitPositionsB, true);

            Vector vectorC = new Vector(false);
            vectorC.Fill(bitPositionsC, true);

            Vector.CreateUnion(vectorA).AssertBitPositions(vectorA.GetBitPositions(true));
            Vector.CreateUnion(vectorA).AssertBitPositions(bitPositionsA);
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
            Vector vector = new Vector(true);
            Vector.CreateUnion(vector, null);
        }

        #endregion
    }
}
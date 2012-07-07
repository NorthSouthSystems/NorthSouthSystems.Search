using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsAnd
    {
        [TestMethod]
        public void AndPopulation() { SafetyVectorCompressionTuple.RunAll(AndPopulationBase); }

        private static void AndPopulationBase(SafetyVectorCompressionTuple safetyVectorCompression)
        {
            int[] fillCounts = new [] { 0, 1, 5, 10, 20, 30, 40, 50, 100, 200, 300, 400, 450, 460, 470, 480, 490, 495, 499, 500 };

            foreach (int fillCount1 in fillCounts)
            {
                foreach (int fillCount2 in fillCounts)
                {
                    Vector vector1 = new Vector(safetyVectorCompression.AllowUnsafe, VectorCompression.None);
                    vector1.RandomFill(500, fillCount1);
                    Vector vector2 = new Vector(safetyVectorCompression.AllowUnsafe, safetyVectorCompression.Compression);
                    vector2.RandomFill(500, fillCount2);

                    HashSet<int> bitPositions = new HashSet<int>(vector1.GetBitPositions(true));
                    bitPositions.IntersectWith(vector2.GetBitPositions(true));

                    int andPopulation = vector1.AndPopulation(vector2);

                    Assert.AreEqual(bitPositions.Count, andPopulation);
                }
            }

        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndArgumentNull()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            vector.And(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndNotSupported()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            Vector input = new Vector(false, VectorCompression.None);
            vector.And(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndPopulationArgumentNull()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            vector.AndPopulation(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndPopulationNotSupported()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            Vector input = new Vector(false, VectorCompression.None);
            vector.AndPopulation(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndFilterArgumentNull()
        {
            Vector vector = new Vector(false, VectorCompression.None);
            vector.AndFilterBitPositions(null, true).ToArray();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndFilterNotSupported()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);
            Vector input = new Vector(false, VectorCompression.None);
            vector.AndFilterBitPositions(input, true).ToArray();
        }

        #endregion
    }
}
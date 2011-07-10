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
        public void AndPopulationCompressed() { AndPopulationBase(true); }

        [TestMethod]
        public void AndPopulationUncompressed() { AndPopulationBase(false); }

        private void AndPopulationBase(bool isCompressed)
        {
            int[] fillCounts = new [] { 0, 1, 5, 10, 20, 30, 40, 50, 100, 200, 300, 400, 450, 460, 470, 480, 490, 495, 499, 500 };

            foreach (int fillCount1 in fillCounts)
            {
                foreach (int fillCount2 in fillCounts)
                {
                    Vector vector1 = new Vector(false);
                    vector1.RandomFill(500, fillCount1);
                    Vector vector2 = new Vector(isCompressed);
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
            Vector vector = new Vector(false);
            vector.And(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndNotSupported()
        {
            Vector vector = new Vector(true);
            Vector input = new Vector(false);
            vector.And(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndPopulationArgumentNull()
        {
            Vector vector = new Vector(false);
            vector.AndPopulation(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndPopulationNotSupported()
        {
            Vector vector = new Vector(true);
            Vector input = new Vector(false);
            vector.AndPopulation(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndFilterArgumentNull()
        {
            Vector vector = new Vector(false);
            vector.AndFilterBitPositions(null, true).ToArray();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndFilterNotSupported()
        {
            Vector vector = new Vector(true);
            Vector input = new Vector(false);
            vector.AndFilterBitPositions(input, true).ToArray();
        }

        #endregion
    }
}
namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VectorTestsAndPopulation
    {
        [TestMethod]
        public void AndPopulation()
        {
            SafetyAndCompression.RunAll(safetyAndCompression =>
            {
                int[] fillCounts = new[] { 0, 1, 5, 10, 20, 30, 40, 50, 100, 200, 300, 400, 450, 460, 470, 480, 490, 495, 499, 500 };

                foreach (int fillCount1 in fillCounts)
                {
                    foreach (int fillCount2 in fillCounts)
                    {
                        var vector1 = new Vector(safetyAndCompression.AllowUnsafe, VectorCompression.None);
                        int[] bitPositions1 = vector1.SetBitsRandom(499, fillCount1, true);
                        var vector2 = new Vector(safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
                        int[] bitPositions2 = vector2.SetBitsRandom(499, fillCount2, true);

                        var bitPositions = new HashSet<int>(bitPositions1);
                        bitPositions.IntersectWith(bitPositions2);

                        int andPopulation = vector1.AndPopulation(vector2);

                        Assert.AreEqual(bitPositions.Count, andPopulation);
                    }
                }
            });
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndPopulationArgumentNull()
        {
            var vector = new Vector(false, VectorCompression.None);
            vector.AndPopulation(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void AndPopulationNotSupported()
        {
            var vector = new Vector(false, VectorCompression.Compressed);
            var input = new Vector(false, VectorCompression.Compressed);
            vector.AndPopulation(input);
        }

        #endregion
    }
}
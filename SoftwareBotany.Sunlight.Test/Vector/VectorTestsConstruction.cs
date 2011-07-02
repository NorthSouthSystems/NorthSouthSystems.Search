using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsConstruction
    {
        [TestMethod]
        public void ConstructCopy()
        {
            int[] fillCounts = new int[] { 0, 1, 2, 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 95, 98, 99, 100 };

            foreach (int fillCount in fillCounts)
            {
                ConstructCopyBase(false, false, 100, fillCount);
                ConstructCopyBase(false, true, 100, fillCount);
                ConstructCopyBase(true, false, 100, fillCount);
                ConstructCopyBase(true, true, 100, fillCount);
            }

            fillCounts = new int[] { 0, 1, 2, 5, 10, 20, 30, 40, 50, 100, 200, 300, 400, 450, 460, 470, 480, 490, 495, 498, 499, 500 };

            foreach (int fillCount in fillCounts)
            {
                ConstructCopyBase(false, false, 500, fillCount);
                ConstructCopyBase(false, true, 500, fillCount);
                ConstructCopyBase(true, false, 500, fillCount);
                ConstructCopyBase(true, true, 500, fillCount);
            }
        }

        private void ConstructCopyBase(bool compressedSource, bool compressedResult, int randomFillMaxBitPosition, int randomFillCount)
        {
            Vector source = new Vector(compressedSource);
            int[] bitPositions = source.RandomFill(randomFillMaxBitPosition, randomFillCount);
            Vector result = compressedSource == compressedResult ? new Vector(source) : new Vector(compressedResult, source);
            result.AssertBitPositions(bitPositions);

            source.WordsClear();
            Assert.AreEqual(0, source.Population);
            source.AssertBitPositions();

            result.WordsClear();
            Assert.AreEqual(0, result.Population);
            result.AssertBitPositions();
        }
    }
}
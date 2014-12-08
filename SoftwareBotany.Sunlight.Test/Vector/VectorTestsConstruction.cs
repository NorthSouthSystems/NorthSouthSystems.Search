namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VectorTestsConstruction
    {
        [TestMethod]
        public void ConstructCopy()
        {
            int[] fillMaxBitPositions = new int[] { 99, 499 };
            int[] fillCounts = new int[] { 0, 1, 2, 5, 10, 20, 30, 40, 50, 100, 200, 300, 400, 450, 460, 470, 480, 490, 495, 498, 499, 500 };

            var instructions =
                 from allowUnsafe in new[] { false, true }
                 from sourceCompression in (int[])Enum.GetValues(typeof(VectorCompression))
                 from resultCompression in (int[])Enum.GetValues(typeof(VectorCompression))
                 from fillMaxBitPosition in fillMaxBitPositions
                 from fillCount in fillCounts
                 where fillCount <= fillMaxBitPosition + 1
                 select new
                 {
                     AllowUnsafe = allowUnsafe,
                     SourceCompression = (VectorCompression)sourceCompression,
                     ResultCompression = (VectorCompression)resultCompression,
                     FillMaxBitPosition = fillMaxBitPosition,
                     FillCount = fillCount
                 };

            foreach (var instruction in instructions)
            {
                var source = new Vector(instruction.AllowUnsafe, instruction.SourceCompression);
                Assert.AreEqual(instruction.AllowUnsafe, source.AllowUnsafe);
                Assert.AreEqual(instruction.SourceCompression, source.Compression);

                int[] bitPositions = source.SetBitsRandom(instruction.FillMaxBitPosition, instruction.FillCount, true);

                var result = new Vector(instruction.AllowUnsafe, instruction.ResultCompression, source);
                Assert.AreEqual(instruction.AllowUnsafe, result.AllowUnsafe);
                Assert.AreEqual(instruction.ResultCompression, result.Compression);

                result.AssertBitPositions(bitPositions);

                var resultWords = (Word[])typeof(Vector).GetField("_words", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result);

                if (instruction.ResultCompression == VectorCompression.None)
                    Assert.IsTrue(!resultWords.Any(word => word.IsCompressed));
                else if (instruction.ResultCompression == VectorCompression.Compressed)
                    Assert.IsTrue(!resultWords.Any(word => word.HasPackedWord));

                source.WordsClear();
                Assert.AreEqual(0, source.Population);
                source.AssertBitPositions();

                result.WordsClear();
                Assert.AreEqual(0, result.Population);
                result.AssertBitPositions();
            }
        }
    }
}
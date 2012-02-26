using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsStatistics
    {
        [TestMethod]
        public void Full()
        {
            Vector vector = new Vector(true);

            IVectorStatistics stats = vector.GenerateStatistics();
            Assert.AreEqual(1, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);

            vector.Fill(Enumerable.Range(0, 32).ToArray(), true);
            vector[62] = true;

            stats = vector.GenerateStatistics();
#if POSITIONLIST
            Assert.AreEqual(2, stats.WordCount);
            Assert.AreEqual(1, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);
#else
            Assert.AreEqual(3, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(1, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);
#endif

            vector.Fill(Enumerable.Range(63, 31).ToArray(), true);
            vector[124] = true;

            stats = vector.GenerateStatistics();
#if POSITIONLIST
            Assert.AreEqual(3, stats.WordCount);
            Assert.AreEqual(2, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);
#else
            Assert.AreEqual(5, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(2, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);
#endif

            vector.Fill(Enumerable.Range(125, 32).ToArray(), true);
            vector[186] = true;

            stats = vector.GenerateStatistics();
#if POSITIONLIST
            Assert.AreEqual(5, stats.WordCount);
            Assert.AreEqual(2, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(1, stats.TwoBitPackableWordCount);
#else
            Assert.AreEqual(7, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(2, stats.OneBitPackableWordCount);
            Assert.AreEqual(1, stats.TwoBitPackableWordCount);
#endif

            vector.Fill(Enumerable.Range(187, 32).ToArray(), true);
            vector[248] = true;

            stats = vector.GenerateStatistics();
#if POSITIONLIST
            Assert.AreEqual(7, stats.WordCount);
            Assert.AreEqual(2, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
#else
            Assert.AreEqual(9, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(2, stats.OneBitPackableWordCount);
            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
#endif
        }
    }
}
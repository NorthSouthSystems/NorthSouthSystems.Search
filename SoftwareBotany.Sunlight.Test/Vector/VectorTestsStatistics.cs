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

            IVectorStatistics stats = vector.GetStatistics();
            Assert.AreEqual(1, stats.WordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);

            vector.Fill(Enumerable.Range(0, 32).ToArray(), true);
            vector[62] = true;

            stats = vector.GetStatistics();
            Assert.AreEqual(3, stats.WordCount);
            Assert.AreEqual(1, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);

            vector.Fill(Enumerable.Range(63, 31).ToArray(), true);
            vector[124] = true;

            stats = vector.GetStatistics();
            Assert.AreEqual(5, stats.WordCount);
            Assert.AreEqual(2, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);

            vector.Fill(Enumerable.Range(125, 32).ToArray(), true);
            vector[186] = true;

            stats = vector.GetStatistics();
            Assert.AreEqual(7, stats.WordCount);
            Assert.AreEqual(2, stats.OneBitPackableWordCount);
            Assert.AreEqual(1, stats.TwoBitPackableWordCount);

            vector.Fill(Enumerable.Range(187, 32).ToArray(), true);
            vector[248] = true;

            stats = vector.GetStatistics();
            Assert.AreEqual(9, stats.WordCount);
            Assert.AreEqual(2, stats.OneBitPackableWordCount);
            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
        }
    }
}
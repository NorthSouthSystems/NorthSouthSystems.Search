using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class CatalogTestsStatistics
    {
        [TestMethod]
        public void Full()
        {
            Catalog<int> catalog = new Catalog<int>("SomeInt");
            ICatalogStatistics stats = catalog.GenerateStatistics();
            Assert.AreEqual(0, stats.VectorCount);
            Assert.AreEqual(0, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);

            catalog.Fill(0, Enumerable.Range(0, 32).ToArray(), true);
            catalog.Set(0, 62, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(1, stats.VectorCount);
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

            catalog.Fill(0, Enumerable.Range(63, 31).ToArray(), true);
            catalog.Set(0, 124, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(1, stats.VectorCount);
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

            catalog.Fill(0, Enumerable.Range(125, 32).ToArray(), true);
            catalog.Set(0, 186, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(1, stats.VectorCount);
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

            catalog.Fill(0, Enumerable.Range(187, 32).ToArray(), true);
            catalog.Set(0, 248, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(1, stats.VectorCount);
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

            catalog.Fill(1, Enumerable.Range(0, 32).ToArray(), true);
            catalog.Set(1, 62, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(2, stats.VectorCount);
#if POSITIONLIST
            Assert.AreEqual(9, stats.WordCount);
            Assert.AreEqual(3, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
#else
            Assert.AreEqual(12, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(3, stats.OneBitPackableWordCount);
            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
#endif

            catalog.Fill(1, Enumerable.Range(63, 31).ToArray(), true);
            catalog.Set(1, 124, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(2, stats.VectorCount);
#if POSITIONLIST
            Assert.AreEqual(10, stats.WordCount);
            Assert.AreEqual(4, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
#else
            Assert.AreEqual(14, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(4, stats.OneBitPackableWordCount);
            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
#endif
        }
    }
}
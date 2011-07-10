using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class EngineTestsStatistics
    {
        [TestMethod]
        public void Full()
        {
            using (Engine<EngineItem, int> engine = new Engine<EngineItem, int>(item => item.Id))
            {
                engine.CreateCatalog("SomeInt", item => item.SomeInt);
                engine.CreateCatalog("SomeString", item => item.SomeString);

                IEngineStatistics stats = engine.GenerateStatistics();
                Assert.AreEqual(2, stats.CatalogCount);
                Assert.AreEqual(0, stats.VectorCount);
                Assert.AreEqual(0, stats.WordCount);
                Assert.AreEqual(0, stats.OneBitPackableWordCount);
                Assert.AreEqual(0, stats.TwoBitPackableWordCount);

                EngineItem[] items = EngineItem.CreateItems(id => IsIdZero(id) ? 0 : 1, id => DateTime.Now, id => IsIdZero(id) ? "0" : "1", id => new string[0], 249);
                engine.Add(items.Take(63));

                stats = engine.GenerateStatistics();
                Assert.AreEqual(2, stats.CatalogCount);
                Assert.AreEqual(4, stats.VectorCount);
                Assert.AreEqual(10, stats.WordCount);
                Assert.AreEqual(2, stats.OneBitPackableWordCount);
                Assert.AreEqual(0, stats.TwoBitPackableWordCount);

                engine.Add(items.Skip(63).Take(62));

                stats = engine.GenerateStatistics();
                Assert.AreEqual(2, stats.CatalogCount);
                Assert.AreEqual(4, stats.VectorCount);
                Assert.AreEqual(18, stats.WordCount);
                Assert.AreEqual(4, stats.OneBitPackableWordCount);
                Assert.AreEqual(0, stats.TwoBitPackableWordCount);

                engine.Add(items.Skip(125).Take(62));

                stats = engine.GenerateStatistics();
                Assert.AreEqual(2, stats.CatalogCount);
                Assert.AreEqual(4, stats.VectorCount);
                Assert.AreEqual(26, stats.WordCount);
                Assert.AreEqual(4, stats.OneBitPackableWordCount);
                Assert.AreEqual(2, stats.TwoBitPackableWordCount);

                engine.Add(items.Skip(187).Take(62));

                stats = engine.GenerateStatistics();
                Assert.AreEqual(2, stats.CatalogCount);
                Assert.AreEqual(4, stats.VectorCount);
                Assert.AreEqual(34, stats.WordCount);
                Assert.AreEqual(4, stats.OneBitPackableWordCount);
                Assert.AreEqual(4, stats.TwoBitPackableWordCount);
            }
        }

        private static bool IsIdZero(int id)
        {
            return (id >= 0 && id < 32)
                || id == 62
                || (id >= 63 && id < 94)
                || id == 124
                || (id >= 125 && id < 157)
                || id == 186
                || (id >= 187 && id < 219)
                || id == 248;
        }
    }
}
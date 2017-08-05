namespace Kangarooper.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;

    [TestClass]
    public class EngineTestsStatistics
    {
        [TestMethod]
        public void Full()
        {
            SafetyAndCompression.RunAll(safetyAndCompression =>
            {
                using (var engine = new Engine<EngineItem, int>(safetyAndCompression.AllowUnsafe, item => item.Id))
                {
                    engine.CreateCatalog("SomeInt", safetyAndCompression.Compression, item => item.SomeInt);
                    engine.CreateCatalog("SomeString", safetyAndCompression.Compression, item => item.SomeString);

                    var stats = engine.GenerateStatistics();
                    Assert.AreEqual(2, stats.CatalogCount);
                    Assert.AreEqual(0, stats.VectorCount);
                    Assert.AreEqual(0, stats.WordCount);
                    Assert.AreEqual(0, stats.PackedWordCount);
                    Assert.AreEqual(0, stats.OneBitPackableWordCount);
                    Assert.AreEqual(0, stats.TwoBitPackableWordCount);

                    var items = EngineItem.CreateItems(id => IsIdZero(id) ? 0 : 1, id => DateTime.Now, id => IsIdZero(id) ? "0" : "1", id => new string[0], 249);
                    engine.Add(items.Take(63));

                    stats = engine.GenerateStatistics();
                    Assert.AreEqual(2, stats.CatalogCount);
                    Assert.AreEqual(4, stats.VectorCount);

                    switch (safetyAndCompression.Compression)
                    {
                        case VectorCompression.None:
                            // TODO
                            break;

                        case VectorCompression.Compressed:
                            Assert.AreEqual(10, stats.WordCount);
                            Assert.AreEqual(0, stats.PackedWordCount);
                            Assert.AreEqual(2, stats.OneBitPackableWordCount);
                            Assert.AreEqual(0, stats.TwoBitPackableWordCount);
                            break;

                        case VectorCompression.CompressedWithPackedPosition:
                            Assert.AreEqual(8, stats.WordCount);
                            Assert.AreEqual(2, stats.PackedWordCount);
                            Assert.AreEqual(0, stats.OneBitPackableWordCount);
                            Assert.AreEqual(0, stats.TwoBitPackableWordCount);
                            break;
                    }

                    engine.Add(items.Skip(63).Take(62));

                    stats = engine.GenerateStatistics();
                    Assert.AreEqual(2, stats.CatalogCount);
                    Assert.AreEqual(4, stats.VectorCount);

                    switch (safetyAndCompression.Compression)
                    {
                        case VectorCompression.None:
                            // TODO
                            break;

                        case VectorCompression.Compressed:
                            Assert.AreEqual(18, stats.WordCount);
                            Assert.AreEqual(0, stats.PackedWordCount);
                            Assert.AreEqual(4, stats.OneBitPackableWordCount);
                            Assert.AreEqual(0, stats.TwoBitPackableWordCount);
                            break;

                        case VectorCompression.CompressedWithPackedPosition:
                            Assert.AreEqual(14, stats.WordCount);
                            Assert.AreEqual(4, stats.PackedWordCount);
                            Assert.AreEqual(0, stats.OneBitPackableWordCount);
                            Assert.AreEqual(0, stats.TwoBitPackableWordCount);
                            break;
                    }

                    engine.Add(items.Skip(125).Take(62));

                    stats = engine.GenerateStatistics();
                    Assert.AreEqual(2, stats.CatalogCount);
                    Assert.AreEqual(4, stats.VectorCount);

                    switch (safetyAndCompression.Compression)
                    {
                        case VectorCompression.None:
                            // TODO
                            break;

                        case VectorCompression.Compressed:
                            Assert.AreEqual(26, stats.WordCount);
                            Assert.AreEqual(0, stats.PackedWordCount);
                            Assert.AreEqual(4, stats.OneBitPackableWordCount);
                            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
                            break;

                        case VectorCompression.CompressedWithPackedPosition:
                            Assert.AreEqual(22, stats.WordCount);
                            Assert.AreEqual(4, stats.PackedWordCount);
                            Assert.AreEqual(0, stats.OneBitPackableWordCount);
                            Assert.AreEqual(2, stats.TwoBitPackableWordCount);
                            break;
                    }

                    engine.Add(items.Skip(187).Take(62));

                    stats = engine.GenerateStatistics();
                    Assert.AreEqual(2, stats.CatalogCount);
                    Assert.AreEqual(4, stats.VectorCount);

                    switch (safetyAndCompression.Compression)
                    {
                        case VectorCompression.None:
                            // TODO
                            break;

                        case VectorCompression.Compressed:
                            Assert.AreEqual(34, stats.WordCount);
                            Assert.AreEqual(0, stats.PackedWordCount);
                            Assert.AreEqual(4, stats.OneBitPackableWordCount);
                            Assert.AreEqual(4, stats.TwoBitPackableWordCount);
                            break;

                        case VectorCompression.CompressedWithPackedPosition:
                            Assert.AreEqual(30, stats.WordCount);
                            Assert.AreEqual(4, stats.PackedWordCount);
                            Assert.AreEqual(0, stats.OneBitPackableWordCount);
                            Assert.AreEqual(4, stats.TwoBitPackableWordCount);
                            break;
                    }
                }
            });
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
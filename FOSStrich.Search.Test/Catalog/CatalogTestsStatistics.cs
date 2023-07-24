namespace FOSStrich.Search;

[TestClass]
public class CatalogTestsStatistics
{
    [TestMethod]
    public void Full() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            var catalog = new Catalog<int>("SomeInt", true, safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
            var stats = catalog.GenerateStatistics();
            Assert.AreEqual(0, stats.VectorCount);
            Assert.AreEqual(0, stats.WordCount);
            Assert.AreEqual(0, stats.PackedWordCount);
            Assert.AreEqual(0, stats.OneBitPackableWordCount);
            Assert.AreEqual(0, stats.TwoBitPackableWordCount);

            catalog.Fill(0, Enumerable.Range(0, 32).ToArray(), true);
            catalog.Set(0, 62, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(1, stats.VectorCount);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    Assert.AreEqual(3, stats.WordCount);
                    Assert.AreEqual(0, stats.PackedWordCount);
                    Assert.AreEqual(1, stats.OneBitPackableWordCount);
                    Assert.AreEqual(0, stats.TwoBitPackableWordCount);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    Assert.AreEqual(2, stats.WordCount);
                    Assert.AreEqual(1, stats.PackedWordCount);
                    Assert.AreEqual(0, stats.OneBitPackableWordCount);
                    Assert.AreEqual(0, stats.TwoBitPackableWordCount);
                    break;
            }

            catalog.Fill(0, Enumerable.Range(63, 31).ToArray(), true);
            catalog.Set(0, 124, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(1, stats.VectorCount);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    Assert.AreEqual(5, stats.WordCount);
                    Assert.AreEqual(0, stats.PackedWordCount);
                    Assert.AreEqual(2, stats.OneBitPackableWordCount);
                    Assert.AreEqual(0, stats.TwoBitPackableWordCount);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    Assert.AreEqual(3, stats.WordCount);
                    Assert.AreEqual(2, stats.PackedWordCount);
                    Assert.AreEqual(0, stats.OneBitPackableWordCount);
                    Assert.AreEqual(0, stats.TwoBitPackableWordCount);
                    break;
            }

            catalog.Fill(0, Enumerable.Range(125, 32).ToArray(), true);
            catalog.Set(0, 186, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(1, stats.VectorCount);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    Assert.AreEqual(7, stats.WordCount);
                    Assert.AreEqual(0, stats.PackedWordCount);
                    Assert.AreEqual(2, stats.OneBitPackableWordCount);
                    Assert.AreEqual(1, stats.TwoBitPackableWordCount);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    Assert.AreEqual(5, stats.WordCount);
                    Assert.AreEqual(2, stats.PackedWordCount);
                    Assert.AreEqual(0, stats.OneBitPackableWordCount);
                    Assert.AreEqual(1, stats.TwoBitPackableWordCount);
                    break;
            }

            catalog.Fill(0, Enumerable.Range(187, 32).ToArray(), true);
            catalog.Set(0, 248, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(1, stats.VectorCount);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    Assert.AreEqual(9, stats.WordCount);
                    Assert.AreEqual(0, stats.PackedWordCount);
                    Assert.AreEqual(2, stats.OneBitPackableWordCount);
                    Assert.AreEqual(2, stats.TwoBitPackableWordCount);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    Assert.AreEqual(7, stats.WordCount);
                    Assert.AreEqual(2, stats.PackedWordCount);
                    Assert.AreEqual(0, stats.OneBitPackableWordCount);
                    Assert.AreEqual(2, stats.TwoBitPackableWordCount);
                    break;
            }

            catalog.Fill(1, Enumerable.Range(0, 32).ToArray(), true);
            catalog.Set(1, 62, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(2, stats.VectorCount);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    Assert.AreEqual(12, stats.WordCount);
                    Assert.AreEqual(0, stats.PackedWordCount);
                    Assert.AreEqual(3, stats.OneBitPackableWordCount);
                    Assert.AreEqual(2, stats.TwoBitPackableWordCount);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    Assert.AreEqual(9, stats.WordCount);
                    Assert.AreEqual(3, stats.PackedWordCount);
                    Assert.AreEqual(0, stats.OneBitPackableWordCount);
                    Assert.AreEqual(2, stats.TwoBitPackableWordCount);
                    break;
            }

            catalog.Fill(1, Enumerable.Range(63, 31).ToArray(), true);
            catalog.Set(1, 124, true);

            stats = catalog.GenerateStatistics();
            Assert.AreEqual(2, stats.VectorCount);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    Assert.AreEqual(14, stats.WordCount);
                    Assert.AreEqual(0, stats.PackedWordCount);
                    Assert.AreEqual(4, stats.OneBitPackableWordCount);
                    Assert.AreEqual(2, stats.TwoBitPackableWordCount);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    Assert.AreEqual(10, stats.WordCount);
                    Assert.AreEqual(4, stats.PackedWordCount);
                    Assert.AreEqual(0, stats.OneBitPackableWordCount);
                    Assert.AreEqual(2, stats.TwoBitPackableWordCount);
                    break;
            }
        });
}
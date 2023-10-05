namespace FOSStrich.Search;

public class EngineTestsStatistics
{
    [Fact]
    public void Full() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            using var engine = new Engine<EngineItem, int>(item => item.Id);

            engine.CreateCatalog("SomeInt", safetyAndCompression.Compression, item => item.SomeInt);
            engine.CreateCatalog("SomeString", safetyAndCompression.Compression, item => item.SomeString);

            var stats = engine.GenerateStatistics();
            stats.CatalogCount.Should().Be(2);
            stats.VectorCount.Should().Be(0);
            stats.WordCount.Should().Be(0);
            stats.PackedWordCount.Should().Be(0);
            stats.OneBitPackableWordCount.Should().Be(0);
            stats.TwoBitPackableWordCount.Should().Be(0);

            var items = EngineItem.CreateItems(id => IsIdZero(id) ? 0 : 1, id => DateTime.Now, id => IsIdZero(id) ? "0" : "1", id => Array.Empty<string>(), 249);
            engine.Add(items.Take(63));

            stats = engine.GenerateStatistics();
            stats.CatalogCount.Should().Be(2);
            stats.VectorCount.Should().Be(4);

            switch (safetyAndCompression.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(10);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(2);
                    stats.TwoBitPackableWordCount.Should().Be(0);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(8);
                    stats.PackedWordCount.Should().Be(2);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(0);
                    break;
            }

            engine.Add(items.Skip(63).Take(62));

            stats = engine.GenerateStatistics();
            stats.CatalogCount.Should().Be(2);
            stats.VectorCount.Should().Be(4);

            switch (safetyAndCompression.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(18);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(4);
                    stats.TwoBitPackableWordCount.Should().Be(0);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(14);
                    stats.PackedWordCount.Should().Be(4);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(0);
                    break;
            }

            engine.Add(items.Skip(125).Take(62));

            stats = engine.GenerateStatistics();
            stats.CatalogCount.Should().Be(2);
            stats.VectorCount.Should().Be(4);

            switch (safetyAndCompression.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(26);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(4);
                    stats.TwoBitPackableWordCount.Should().Be(2);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(22);
                    stats.PackedWordCount.Should().Be(4);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(2);
                    break;
            }

            engine.Add(items.Skip(187).Take(62));

            stats = engine.GenerateStatistics();
            stats.CatalogCount.Should().Be(2);
            stats.VectorCount.Should().Be(4);

            switch (safetyAndCompression.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(34);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(4);
                    stats.TwoBitPackableWordCount.Should().Be(4);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(30);
                    stats.PackedWordCount.Should().Be(4);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(4);
                    break;
            }
        });

    private static bool IsIdZero(int id) =>
        (id >= 0 && id < 32)
            || id == 62
            || (id >= 63 && id < 94)
            || id == 124
            || (id >= 125 && id < 157)
            || id == 186
            || (id >= 187 && id < 219)
            || id == 248;
}
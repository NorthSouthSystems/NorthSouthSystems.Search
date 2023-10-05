namespace FOSStrich.Search;

public class CatalogTestsStatistics
{
    [Fact]
    public void Full() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            var catalog = new Catalog<int>("SomeInt", true, safetyAndCompression.Compression);
            var stats = catalog.GenerateStatistics();
            stats.VectorCount.Should().Be(0);
            stats.WordCount.Should().Be(0);
            stats.PackedWordCount.Should().Be(0);
            stats.OneBitPackableWordCount.Should().Be(0);
            stats.TwoBitPackableWordCount.Should().Be(0);

            catalog.Fill(0, Enumerable.Range(0, 32).ToArray(), true);
            catalog.Set(0, 62, true);

            stats = catalog.GenerateStatistics();
            stats.VectorCount.Should().Be(1);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(3);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(1);
                    stats.TwoBitPackableWordCount.Should().Be(0);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(2);
                    stats.PackedWordCount.Should().Be(1);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(0);
                    break;
            }

            catalog.Fill(0, Enumerable.Range(63, 31).ToArray(), true);
            catalog.Set(0, 124, true);

            stats = catalog.GenerateStatistics();
            stats.VectorCount.Should().Be(1);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(5);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(2);
                    stats.TwoBitPackableWordCount.Should().Be(0);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(3);
                    stats.PackedWordCount.Should().Be(2);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(0);
                    break;
            }

            catalog.Fill(0, Enumerable.Range(125, 32).ToArray(), true);
            catalog.Set(0, 186, true);

            stats = catalog.GenerateStatistics();
            stats.VectorCount.Should().Be(1);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(7);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(2);
                    stats.TwoBitPackableWordCount.Should().Be(1);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(5);
                    stats.PackedWordCount.Should().Be(2);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(1);
                    break;
            }

            catalog.Fill(0, Enumerable.Range(187, 32).ToArray(), true);
            catalog.Set(0, 248, true);

            stats = catalog.GenerateStatistics();
            stats.VectorCount.Should().Be(1);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(9);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(2);
                    stats.TwoBitPackableWordCount.Should().Be(2);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(7);
                    stats.PackedWordCount.Should().Be(2);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(2);
                    break;
            }

            catalog.Fill(1, Enumerable.Range(0, 32).ToArray(), true);
            catalog.Set(1, 62, true);

            stats = catalog.GenerateStatistics();
            stats.VectorCount.Should().Be(2);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(12);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(3);
                    stats.TwoBitPackableWordCount.Should().Be(2);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(9);
                    stats.PackedWordCount.Should().Be(3);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(2);
                    break;
            }

            catalog.Fill(1, Enumerable.Range(63, 31).ToArray(), true);
            catalog.Set(1, 124, true);

            stats = catalog.GenerateStatistics();
            stats.VectorCount.Should().Be(2);

            switch (catalog.Compression)
            {
                case VectorCompression.None:
                    // TODO
                    break;

                case VectorCompression.Compressed:
                    stats.WordCount.Should().Be(14);
                    stats.PackedWordCount.Should().Be(0);
                    stats.OneBitPackableWordCount.Should().Be(4);
                    stats.TwoBitPackableWordCount.Should().Be(2);
                    break;

                case VectorCompression.CompressedWithPackedPosition:
                    stats.WordCount.Should().Be(10);
                    stats.PackedWordCount.Should().Be(4);
                    stats.OneBitPackableWordCount.Should().Be(0);
                    stats.TwoBitPackableWordCount.Should().Be(2);
                    break;
            }
        });
}
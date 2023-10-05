namespace FOSStrich.Search;

public class VectorTestsStatistics
{
    [Fact]
    public void FullCompressed()
    {
        var vector = new Vector(VectorCompression.Compressed);

        var stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(1);
        stats.PackedWordCount.Should().Be(0);
        stats.OneBitPackableWordCount.Should().Be(0);
        stats.TwoBitPackableWordCount.Should().Be(0);

        vector.SetBits(Enumerable.Range(0, 32).ToArray(), true);
        vector[62] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(3);
        stats.PackedWordCount.Should().Be(0);
        stats.OneBitPackableWordCount.Should().Be(1);
        stats.TwoBitPackableWordCount.Should().Be(0);

        vector.SetBits(Enumerable.Range(63, 31).ToArray(), true);
        vector[124] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(5);
        stats.PackedWordCount.Should().Be(0);
        stats.OneBitPackableWordCount.Should().Be(2);
        stats.TwoBitPackableWordCount.Should().Be(0);

        vector.SetBits(Enumerable.Range(125, 32).ToArray(), true);
        vector[186] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(7);
        stats.PackedWordCount.Should().Be(0);
        stats.OneBitPackableWordCount.Should().Be(2);
        stats.TwoBitPackableWordCount.Should().Be(1);

        vector.SetBits(Enumerable.Range(187, 32).ToArray(), true);
        vector[248] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(9);
        stats.PackedWordCount.Should().Be(0);
        stats.OneBitPackableWordCount.Should().Be(2);
        stats.TwoBitPackableWordCount.Should().Be(2);
    }

    [Fact]
    public void FullCompressedWithPackedPosition()
    {
        var vector = new Vector(VectorCompression.CompressedWithPackedPosition);

        var stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(1);
        stats.PackedWordCount.Should().Be(0);
        stats.OneBitPackableWordCount.Should().Be(0);
        stats.TwoBitPackableWordCount.Should().Be(0);

        vector.SetBits(Enumerable.Range(0, 32).ToArray(), true);
        vector[62] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(2);
        stats.PackedWordCount.Should().Be(1);
        stats.OneBitPackableWordCount.Should().Be(0);
        stats.TwoBitPackableWordCount.Should().Be(0);

        vector.SetBits(Enumerable.Range(63, 31).ToArray(), true);
        vector[124] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(3);
        stats.PackedWordCount.Should().Be(2);
        stats.OneBitPackableWordCount.Should().Be(0);
        stats.TwoBitPackableWordCount.Should().Be(0);

        vector.SetBits(Enumerable.Range(125, 32).ToArray(), true);
        vector[186] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(5);
        stats.PackedWordCount.Should().Be(2);
        stats.OneBitPackableWordCount.Should().Be(0);
        stats.TwoBitPackableWordCount.Should().Be(1);

        vector.SetBits(Enumerable.Range(187, 32).ToArray(), true);
        vector[248] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(7);
        stats.PackedWordCount.Should().Be(2);
        stats.OneBitPackableWordCount.Should().Be(0);
        stats.TwoBitPackableWordCount.Should().Be(2);
    }

    [Fact]
    public void PackedPositionOneBitPackable()
    {
        var vector = new Vector(VectorCompression.CompressedWithPackedPosition);

        var stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(1);
        stats.PackedWordCount.Should().Be(0);
        stats.OneBitPackableWordCount.Should().Be(0);
        stats.TwoBitPackableWordCount.Should().Be(0);

        vector.SetBits(Enumerable.Range(0, 32).ToArray(), true);
        vector[62] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(2);
        stats.PackedWordCount.Should().Be(1);
        stats.OneBitPackableWordCount.Should().Be(0);
        stats.TwoBitPackableWordCount.Should().Be(0);

        vector[93] = true;

        stats = vector.GenerateStatistics();
        stats.WordCount.Should().Be(3);
        stats.PackedWordCount.Should().Be(1);
        stats.OneBitPackableWordCount.Should().Be(1);
        stats.TwoBitPackableWordCount.Should().Be(0);
    }
}
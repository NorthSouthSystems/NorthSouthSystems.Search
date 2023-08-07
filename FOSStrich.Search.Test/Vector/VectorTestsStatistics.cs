namespace FOSStrich.Search;

[TestClass]
public class VectorTestsStatistics
{
    [TestMethod]
    public void FullCompressed()
    {
        var vector = new Vector(false, VectorCompression.Compressed);

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

    [TestMethod]
    public void FullCompressedWithPackedPosition()
    {
        var vector = new Vector(false, VectorCompression.CompressedWithPackedPosition);

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

    [TestMethod]
    public void PackedPositionOneBitPackable()
    {
        var vector = new Vector(false, VectorCompression.CompressedWithPackedPosition);

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
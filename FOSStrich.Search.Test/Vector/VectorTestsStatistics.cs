namespace FOSStrich.Search;

[TestClass]
public class VectorTestsStatistics
{
    [TestMethod]
    public void FullCompressed()
    {
        var vector = new Vector(false, VectorCompression.Compressed);

        var stats = vector.GenerateStatistics();
        Assert.AreEqual(1, stats.WordCount);
        Assert.AreEqual(0, stats.PackedWordCount);
        Assert.AreEqual(0, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(0, 32).ToArray(), true);
        vector[62] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(3, stats.WordCount);
        Assert.AreEqual(0, stats.PackedWordCount);
        Assert.AreEqual(1, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(63, 31).ToArray(), true);
        vector[124] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(5, stats.WordCount);
        Assert.AreEqual(0, stats.PackedWordCount);
        Assert.AreEqual(2, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(125, 32).ToArray(), true);
        vector[186] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(7, stats.WordCount);
        Assert.AreEqual(0, stats.PackedWordCount);
        Assert.AreEqual(2, stats.OneBitPackableWordCount);
        Assert.AreEqual(1, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(187, 32).ToArray(), true);
        vector[248] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(9, stats.WordCount);
        Assert.AreEqual(0, stats.PackedWordCount);
        Assert.AreEqual(2, stats.OneBitPackableWordCount);
        Assert.AreEqual(2, stats.TwoBitPackableWordCount);
    }

    [TestMethod]
    public void FullCompressedWithPackedPosition()
    {
        var vector = new Vector(false, VectorCompression.CompressedWithPackedPosition);

        var stats = vector.GenerateStatistics();
        Assert.AreEqual(1, stats.WordCount);
        Assert.AreEqual(0, stats.PackedWordCount);
        Assert.AreEqual(0, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(0, 32).ToArray(), true);
        vector[62] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(2, stats.WordCount);
        Assert.AreEqual(1, stats.PackedWordCount);
        Assert.AreEqual(0, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(63, 31).ToArray(), true);
        vector[124] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(3, stats.WordCount);
        Assert.AreEqual(2, stats.PackedWordCount);
        Assert.AreEqual(0, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(125, 32).ToArray(), true);
        vector[186] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(5, stats.WordCount);
        Assert.AreEqual(2, stats.PackedWordCount);
        Assert.AreEqual(0, stats.OneBitPackableWordCount);
        Assert.AreEqual(1, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(187, 32).ToArray(), true);
        vector[248] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(7, stats.WordCount);
        Assert.AreEqual(2, stats.PackedWordCount);
        Assert.AreEqual(0, stats.OneBitPackableWordCount);
        Assert.AreEqual(2, stats.TwoBitPackableWordCount);
    }

    [TestMethod]
    public void PackedPositionOneBitPackable()
    {
        var vector = new Vector(false, VectorCompression.CompressedWithPackedPosition);

        var stats = vector.GenerateStatistics();
        Assert.AreEqual(1, stats.WordCount);
        Assert.AreEqual(0, stats.PackedWordCount);
        Assert.AreEqual(0, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);

        vector.SetBits(Enumerable.Range(0, 32).ToArray(), true);
        vector[62] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(2, stats.WordCount);
        Assert.AreEqual(1, stats.PackedWordCount);
        Assert.AreEqual(0, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);

        vector[93] = true;

        stats = vector.GenerateStatistics();
        Assert.AreEqual(3, stats.WordCount);
        Assert.AreEqual(1, stats.PackedWordCount);
        Assert.AreEqual(1, stats.OneBitPackableWordCount);
        Assert.AreEqual(0, stats.TwoBitPackableWordCount);
    }
}
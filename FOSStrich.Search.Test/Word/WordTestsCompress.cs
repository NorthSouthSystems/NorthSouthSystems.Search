namespace FOSStrich.Search;

[TestClass]
public class WordTestsCompress
{
    [TestMethod]
    public void Compressible()
    {
        Word word = new Word(0);
        word.IsCompressible.Should().BeTrue();
        word.Compress();
        word.Raw.Should().Be(0x80000001);
        word.IsCompressed.Should().BeTrue();
        word.FillBit.Should().BeFalse();
        word.FillCount.Should().Be(1);

        word = new Word(Word.COMPRESSIBLEMASK);
        word.IsCompressible.Should().BeTrue();
        word.Compress();
        word.Raw.Should().Be(0xC0000001);
        word.IsCompressed.Should().BeTrue();
        word.FillBit.Should().BeTrue();
        word.FillCount.Should().Be(1);
    }

    [TestMethod]
    public void NotCompressible()
    {
        foreach (uint wordValue in new uint[] { 0x00000001u, 0x40000000u, 0x7FFFFFFEu, 0x3FFFFFFFu, 0x12345678u, 0x7FEDCBA9u })
        {
            Word word = new Word(wordValue);
            word.Compress();
            word.Raw.Should().Be(wordValue);
            word.IsCompressible.Should().BeFalse();
            word.IsCompressed.Should().BeFalse();
        }
    }

    [TestMethod]
    public void NotCompressibleFullCoverage()
    {
        for (uint i = 1; i < 0x7FFFFFFF; i += WordExtensions.LARGEPRIME)
        {
            Word word = new Word(i);
            word.Compress();
            word.Raw.Should().Be(i);
            word.IsCompressible.Should().BeFalse();
            word.IsCompressed.Should().BeFalse();
        }
    }

    [TestMethod]
    public void CompressedFullCoverage()
    {
        for (uint i = 0x80000000; i > 0x80000000 && i <= 0xFFFFFFFF; i += WordExtensions.LARGEPRIME)
        {
            Word word = new Word(i);
            word.Compress();
            word.Raw.Should().Be(i, because: word.ToString());
            word.IsCompressed.Should().BeTrue(because: word.ToString());
        }
    }

    [TestMethod]
    public void Pack()
    {
        Word word = new Word(true, 1);

        Assert.IsTrue(word.IsCompressed);
        Assert.IsTrue(word.FillBit);
        word.FillCount.Should().Be(1);
        Assert.IsFalse(word.HasPackedWord);

        word.Pack(new Word(1));

        Assert.IsTrue(word.IsCompressed);
        Assert.IsTrue(word.FillBit);
        word.FillCount.Should().Be(1);
        Assert.IsTrue(word.HasPackedWord);
        word.PackedPosition.Should().Be(30);
        word.PackedWord.Raw.Should().Be((uint)1);

        word = new Word(true, 1);

        Assert.IsTrue(word.IsCompressed);
        Assert.IsTrue(word.FillBit);
        word.FillCount.Should().Be(1);
        Assert.IsFalse(word.HasPackedWord);

        word.Pack(new Word(1 << 30));

        Assert.IsTrue(word.IsCompressed);
        Assert.IsTrue(word.FillBit);
        word.FillCount.Should().Be(1);
        Assert.IsTrue(word.HasPackedWord);
        word.PackedPosition.Should().Be(0);
        word.PackedWord.Raw.Should().Be((uint)1 << 30);
    }

    #region Exceptions

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PackedPositionNotSupported()
    {
        Word word = new Word(0);
        int packedPositions = word.PackedPosition;
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PackedWordNotSupported()
    {
        Word word = new Word(0);
        Word packedWord = word.PackedWord;
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PackNotSupported1()
    {
        Word word = new Word(0);
        word.Pack(new Word(1));
    }

    [TestMethod]
    public void PackNotSupported2OK()
    {
        Word word = new Word(true, 1);
        word.Pack(new Word(1));
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PackNotSupported2()
    {
        Word word = new Word(true, 1);
        word.Pack(new Word(1));
        word.Pack(new Word(1));
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PackNotSupported3()
    {
        Word word = new Word(true, 1);
        word.Pack(new Word(true, 1));
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PackNotSupported4_1()
    {
        Word word = new Word(true, 1);
        word.Pack(new Word(0));
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PackNotSupported4_2()
    {
        Word word = new Word(true, 1);
        word.Pack(new Word(3));
    }

    #endregion
}
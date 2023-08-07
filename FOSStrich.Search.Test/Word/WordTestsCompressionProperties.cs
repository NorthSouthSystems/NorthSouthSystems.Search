namespace FOSStrich.Search;

[TestClass]
public class WordTestsCompressionProperties
{
    [TestMethod]
    public void IsCompressibleTrue()
    {
        var word = new Word(0x00000000u);
        word.IsCompressible.Should().BeTrue();
        word.CompressibleFillBit.Should().BeFalse();
        word.IsCompressed.Should().BeFalse();

        word = new Word(Word.COMPRESSIBLEMASK);
        word.IsCompressible.Should().BeTrue();
        word.CompressibleFillBit.Should().BeTrue();
        word.IsCompressed.Should().BeFalse();
    }

    [TestMethod]
    public void IsCompressibleFalse()
    {
        foreach (uint u in new uint[] { 0x00000001u, 0x40000000u, 0x7FFFFFFEu, 0x3FFFFFFFu, 0x12345678u, 0x7FEDCBA9u })
        {
            var word = new Word(u);
            word.IsCompressible.Should().BeFalse();
            word.IsCompressed.Should().BeFalse();
        }
    }

    [TestMethod]
    public void IsCompressibleFalseFullCoverage()
    {
        for (uint i = 1; i < 0x7FFFFFFFu; i += WordExtensions.LARGEPRIME)
        {
            var word = new Word(i);
            word.IsCompressible.Should().BeFalse();
            word.IsCompressed.Should().BeFalse();
        }
    }

    [TestMethod]
    public void CompressedFillBitFalseNoFill() => CompressedBase(false, 0x00000000, 0x80000000u);

    [TestMethod]
    public void CompressedFillBitFalse1Fill() => CompressedBase(false, 0x00000001, 0x80000001u);

    [TestMethod]
    public void CompressedFillBitFalseMaxFill() => CompressedBase(false, 0x01FFFFFF, 0x81FFFFFFu);

    [TestMethod]
    public void CompressedFillBitTrueNoFill() => CompressedBase(true, 0x00000000, 0xC0000000u);

    [TestMethod]
    public void CompressedFillBitTrue1Fill() => CompressedBase(true, 0x00000001, 0xC0000001u);

    [TestMethod]
    public void CompressedFillBitTrueMaxFill() => CompressedBase(true, 0x01FFFFFF, 0xC1FFFFFFu);

    private void CompressedBase(bool fillBit, int fillCount, uint wordValue)
    {
        var word = new Word(fillBit, fillCount);
        word.Raw.Should().Be(wordValue);
        word.IsCompressed.Should().BeTrue();
        word.FillBit.Should().Be(fillBit);
        word.FillCount.Should().Be(fillCount);
    }

    [TestMethod]
    public void CompressedFullCoverage()
    {
        foreach (bool fillBit in new bool[] { false, true })
        {
            for (int i = 0; i < 0x02000000; i += WordExtensions.LARGEPRIME)
            {
                var word = new Word(fillBit, i);
                word.Raw.Should().Be((fillBit ? 0xC0000000 : 0x80000000) + (uint)i);
                word.IsCompressed.Should().BeTrue(because: word.ToString());
                word.FillBit.Should().Be(fillBit, because: word.ToString());
                word.FillCount.Should().Be(i, because: word.ToString());
            }
        }
    }
}
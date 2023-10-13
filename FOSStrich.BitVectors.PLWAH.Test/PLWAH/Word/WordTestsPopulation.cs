namespace FOSStrich.BitVectors.PLWAH;

public class WordTestsPopulation
{
    [Fact]
    public void NotCompressed()
    {
        new Word(0x00000000u).Population.Should().Be(0);
        new Word(0x7FFFFFFFu).Population.Should().Be(31);
        new Word(0x00000001u).Population.Should().Be(1);
        new Word(0x40000000u).Population.Should().Be(1);
        new Word(0x7FFFFFFEu).Population.Should().Be(30);
        new Word(0x3FFFFFFFu).Population.Should().Be(30);
        new Word(0x12345678u).Population.Should().Be(13);
        new Word(0x7FEDCBA9u).Population.Should().Be(22);

        for (int i = 0; i < Word.SIZE - 1; i++)
            new Word(1u << i).Population.Should().Be(1);
    }

    [Fact]
    public void CompressedFillBitFalseNoFill() => CompressedBase(false, 0x00000000);

    [Fact]
    public void CompressedFillBitFalse1Fill() => CompressedBase(false, 0x00000001);

    [Fact]
    public void CompressedFillBitFalseMaxFill() => CompressedBase(false, 0x01FFFFFF);

    [Fact]
    public void CompressedFillBitTrueNoFill() => CompressedBase(true, 0x00000000);

    [Fact]
    public void CompressedFillBitTrue1Fill() => CompressedBase(true, 0x00000001);

    [Fact]
    public void CompressedFillBitTrueMaxFill() => CompressedBase(true, 0x01FFFFFF);

    private void CompressedBase(bool fillBit, int fillCount)
    {
        var word = new Word(fillBit, fillCount);
        word.Population.Should().Be(fillBit ? (31 * fillCount) : 0);
    }

    [Fact]
    public void CompressedFullCoverage()
    {
        foreach (bool fillBit in new bool[] { false, true })
        {
            for (int i = 0; i < 0x02000000; i += WordExtensions.LARGEPRIME)
            {
                var word = new Word(fillBit, i);
                word.Population.Should().Be(fillBit ? (31 * i) : 0, because: word.ToString());
            }
        }
    }

    [Fact]
    public void Packed()
    {
        var word = new Word(false, 1);
        word.Population.Should().Be(0);
        word.Pack(new Word(1));
        word.Population.Should().Be(1);

        word = new Word(true, 1);
        word.Population.Should().Be(Word.SIZE - 1);
        word.Pack(new Word(1));
        word.Population.Should().Be(Word.SIZE);
    }
}
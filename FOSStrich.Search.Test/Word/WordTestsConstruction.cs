namespace FOSStrich.Search;

[TestClass]
public class WordTestsConstruction
{
    [TestMethod]
    public void Bounds()
    {
        Word word = new Word(0);
        word = new Word(Word.COMPRESSEDMASK - 1);
    }

    [TestMethod]
    public void Compressed()
    {
        Word word = new Word(false, 0);
        word.FillBit.Should().Be(false);
        word.FillCount.Should().Be(0);

        word = new Word(false, 1);
        word.FillBit.Should().Be(false);
        word.FillCount.Should().Be(1);

        word = new Word(false, 22);
        word.FillBit.Should().Be(false);
        word.FillCount.Should().Be(22);

        word = new Word(true, 0);
        word.FillBit.Should().Be(true);
        word.FillCount.Should().Be(0);

        word = new Word(true, 1);
        word.FillBit.Should().Be(true);
        word.FillCount.Should().Be(1);

        word = new Word(true, 22);
        word.FillBit.Should().Be(true);
        word.FillCount.Should().Be(22);
    }

    #region Exceptions

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void RawArgumentOutOfRange()
    {
        Word word = new Word(Word.COMPRESSEDMASK);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FillCountOutOfRange1()
    {
        Word word = new Word(true, -1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FillCountOutOfRange2()
    {
        Word word = new Word(true, 0x02000000);
    }

    #endregion
}
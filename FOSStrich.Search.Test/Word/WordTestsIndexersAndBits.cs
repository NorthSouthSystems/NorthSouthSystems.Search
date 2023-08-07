namespace FOSStrich.Search;

[TestClass]
public class WordTestsIndexersAndBits
{
    [TestMethod]
    public void GetSetPositionsSimple()
    {
        var word = new Word();

        for (int i = 0; i < Word.SIZE - 1; i++)
        {
            word[i] = true;

            for (int j = 0; j < Word.SIZE - 1; j++)
                word[j].Should().Be(i == j);

            word[i] = false;

            for (int j = 0; j < Word.SIZE - 1; j++)
                word[j].Should().BeFalse();
        }
    }

    [TestMethod]
    public void BitsSimple()
    {
        int[] bitPositions = new int[] { 0, 3, 8, 12, 19, 24, 30 };
        BitsBase(bitPositions, true);
        BitsBase(bitPositions, false);
    }

    private void BitsBase(int[] bitPositions, bool value)
    {
        var word = new Word();

        for (int i = 0; i < Word.SIZE - 1; i++)
            word[i] = bitPositions.Contains(i) ? value : !value;

        bool[] bits = word.Bits;

        bits.Count(bit => value ? bit : !bit).Should().Be(bitPositions.Length);

        for (int i = 0; i < Word.SIZE - 1; i++)
            (value ? bits[i] : !bits[i]).Should().Be(bitPositions.Contains(i));
    }

    [TestMethod]
    public void GetBitPositionsSimple()
    {
        int[] bitPositions = new int[] { 0, 3, 8, 12, 19, 24, 30 };
        GetBitPositionsBase(bitPositions, true);
        GetBitPositionsBase(bitPositions, false);
    }

    private void GetBitPositionsBase(int[] bitPositions, bool value)
    {
        var word = new Word();

        for (int i = 0; i < Word.SIZE - 1; i++)
            word[i] = bitPositions.Contains(i) ? value : !value;

        int[] getBitPositions = word.GetBitPositions(value);

        getBitPositions.Length.Should().Be(bitPositions.Length);

        for (int i = 0; i < bitPositions.Length; i++)
            getBitPositions[i].Should().Be(bitPositions[i]);
    }

    #region Exceptions

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void ComputeIndexerMaskNotSupported()
    {
        var word = new Word(true, 1);
        bool bit = word[0];
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ComputeIndexerMaskArgumentOutOfRange1()
    {
        var word = new Word(0);
        bool bit = word[-1];
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ComputeIndexerMaskArgumentOutOfRange2()
    {
        var word = new Word(0);
        bool bit = word[Word.SIZE - 1];
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void BitsNotSupported()
    {
        var word = new Word(true, 1);
        bool[] bits = word.Bits;
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void GetBitPositionsNotSupported()
    {
        var word = new Word(true, 1);
        int[] bitPositions = word.GetBitPositions(true);
    }

    #endregion
}
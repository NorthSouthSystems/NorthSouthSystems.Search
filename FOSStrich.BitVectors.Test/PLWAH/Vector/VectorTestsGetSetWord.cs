namespace FOSStrich.BitVectors.PLWAH;

public class VectorTestsGetSetWord
{
    [Fact]
    public void Uncompressed()
    {
        var vector = new Vector(false);

        vector.AssertWordLogicalValues(0, 0, 0);
        vector.AssertWordCounts(1, 1);

        // Ignore 0 Sets When ZeroFilling required
        vector.SetWord(1, new Word(0));
        vector.AssertWordLogicalValues(0, 0, 0);
        vector.AssertWordCounts(1, 1);

        vector.SetWord(1, new Word(1));
        vector.AssertWordLogicalValues(0, 1, 0);
        vector.AssertWordCounts(2, 2);

        vector.SetWord(1, new Word(0));
        vector.AssertWordLogicalValues(0, 0, 0);
        vector.AssertWordCounts(2, 2);

        vector.SetWord(2, new Word(1));
        vector.AssertWordLogicalValues(0, 0, 1);
        vector.AssertWordCounts(3, 3);

        vector.SetWord(0, new Word(1));
        vector.AssertWordLogicalValues(1, 0, 1);
        vector.AssertWordCounts(3, 3);

        vector.SetWord(1, new Word(1));
        vector.AssertWordLogicalValues(1, 1, 1);
        vector.AssertWordCounts(3, 3);

        vector.SetWord(0, new Word(0));
        vector.AssertWordLogicalValues(0, 1, 1);
        vector.AssertWordCounts(3, 3);

        vector.SetWord(1, new Word(0));
        vector.AssertWordLogicalValues(0, 0, 1);
        vector.AssertWordCounts(3, 3);

        vector.SetWord(2, new Word(0));
        vector.AssertWordLogicalValues(0, 0, 0);
        vector.AssertWordCounts(3, 3);
    }

    [Fact]
    public void CompressedZeroFillCodeCoverage()
    {
        var vector = new Vector(true);

        vector.AssertWordLogicalValues(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        vector.AssertWordCounts(1, 1);

        // Ignore 0 Sets When ZeroFilling required
        vector.SetWord(1, new Word(0));
        vector.AssertWordLogicalValues(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        vector.AssertWordCounts(1, 1);

        // Force Compression of Word[0]
        vector.SetWord(1, new Word(1));
        vector.AssertWordLogicalValues(0, 1, 0, 0, 0, 0, 0, 0, 0, 0);
        vector.AssertWordCounts(2, 2);

        vector.SetWord(1, new Word(0));
        vector.AssertWordLogicalValues(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        vector.AssertWordCounts(2, 2);

        // Increment Compression on Word[0]
        vector.SetWord(2, new Word(1));
        vector.AssertWordLogicalValues(0, 0, 1, 0, 0, 0, 0, 0, 0, 0);
        vector.AssertWordCounts(2, 3);

        vector.SetWord(2, new Word(0));
        vector.AssertWordLogicalValues(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        vector.AssertWordCounts(2, 3);

        // End the 0's and have the tail compressed
        vector.SetWord(3, new Word(1));
        vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        vector.AssertWordCounts(2, 4);

        // Add a 1 and pack the tail
        vector.SetWord(4, new Word(1));
        vector.AssertWordLogicalValues(0, 0, 0, 1, 1, 0, 0, 0, 0, 0);
        // TODO : vector.AssertWordCounts(vector.IsPackedPositionEnabled ? 2 : 3, 5);
        vector.AssertWordCounts(2, 5);

        // Add a 0 Word
        vector.SetWord(4, new Word(0));
        vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        // TODO : vector.AssertWordCounts(vector.IsPackedPositionEnabled ? 2 : 3, 5);
        vector.AssertWordCounts(2, 5);

        // Add a 1 Word far away, forcing a compression
        vector.SetWord(7, new Word(1));
        vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 1, 0, 0);
        // TODO : vector.AssertWordCounts(vector.IsPackedPositionEnabled ? 3 : 4, 8);
        vector.AssertWordCounts(3, 8);

        // Add a 1 Word two spaces away, forcing a pack and 2xZeroFill with overwrite
        vector.SetWord(9, new Word(1));
        vector.AssertWordLogicalValues(0, 0, 0, 1, 0, 0, 0, 1, 0, 1);
        // TODO : vector.AssertWordCounts(vector.IsPackedPositionEnabled ? 4 : 6, 10);
        vector.AssertWordCounts(4, 10);
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var vector = new Vector(false);
            vector.GetWordLogical(-1);
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "GetWordLogicalArgumentOutOfRange");

        act = () =>
        {
            var vector = new Vector(false);
            vector.SetWord(-1, new Word(0x11111111));
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "SetWordArgumentOutOfRange");

        act = () =>
        {
            var vector = new Vector(true);
            vector[30] = true;
            vector.SetWord(0, new Word(0x00000001u));
        };
        act.Should().NotThrow(because: "SetWordSupportedForwardOnly");

        act = () =>
        {
            var vector = new Vector(true);
            vector[31] = true;
            vector.SetWord(0, new Word(0x00000001u));
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "SetWordNotSupportedForwardOnly");
    }
}
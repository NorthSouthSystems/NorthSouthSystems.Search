#if POSITIONLISTENABLED
namespace FOSStrich.BitVectors.PLWAH;
#else
namespace FOSStrich.BitVectors.WAH;
#endif

public class WordTestsIndexersAndBits
{
    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var word = new Word(true, 1);
            bool bit = word[0];
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "ComputeIndexerMaskNotSupported");

        act = () =>
        {
            var word = new Word(0);
            bool bit = word[-1];
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "ComputeIndexerMaskArgumentOutOfRange1");

        act = () =>
        {
            var word = new Word(0);
            bool bit = word[Word.SIZE - 1];
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "ComputeIndexerMaskArgumentOutOfRange2");

        act = () =>
        {
            var word = new Word(true, 1);
            bool[] bits = word.Bits;
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "BitsNotSupported");

        act = () =>
        {
            var word = new Word(true, 1);
            int[] bitPositions = word.GetBitPositions(true);
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "GetBitPositionsNotSupported");
    }
}
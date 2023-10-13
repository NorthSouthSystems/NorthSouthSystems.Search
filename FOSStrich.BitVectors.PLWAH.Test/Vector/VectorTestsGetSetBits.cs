#if POSITIONLISTENABLED
namespace FOSStrich.BitVectors.PLWAH;
#else
namespace FOSStrich.BitVectors.WAH;
#endif

public class VectorTestsGetSetBits
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Full(bool isCompressed)
    {
        var vector = new Vector(isCompressed);
        int[] bitPositions = vector.SetBitsRandom(999, 100, true);
        vector[2000] = false;
        vector.AssertBitPositions(bitPositions);
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var vector = new Vector(false);
            vector[-1] = true;
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "IndexArgumentOutOfRange1");

        act = () =>
        {
            var vector = new Vector(false);
            bool value = vector[-1];
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "IndexArgumentOutOfRange2");

        act = () =>
        {
            var vector = new Vector(true);
            vector[30] = true;
            vector[61] = true;
        };
        act.Should().NotThrow(because: "SetBitSupportedForwardOnly");

        act = () =>
        {
            var vector = new Vector(true);
            vector[30] = true;
            vector[31] = true;
            vector[30] = true;
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "SetBitNotSupportedForwardOnly");

        act = () =>
        {
            var vector = new Vector(true);
            vector.Bits.ToArray();
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "GetBitsCompressedNotSupported");
    }
}
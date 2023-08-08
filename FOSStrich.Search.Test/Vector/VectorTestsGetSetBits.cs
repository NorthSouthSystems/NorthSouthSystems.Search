namespace FOSStrich.Search;

[TestClass]
public class VectorTestsGetSetBits
{
    [TestMethod]
    public void Full() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            var vector = new Vector(safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
            int[] bitPositions = vector.SetBitsRandom(999, 100, true);
            vector[2000] = false;
            vector.AssertBitPositions(bitPositions);
        });

    [TestMethod]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.None);
            vector[-1] = true;
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "IndexArgumentOutOfRange1");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.None);
            bool value = vector[-1];
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "IndexArgumentOutOfRange2");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);
            vector[30] = true;
            vector[61] = true;
        };
        act.Should().NotThrow(because: "SetBitSupportedForwardOnly");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);
            vector[30] = true;
            vector[31] = true;
            vector[30] = true;
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "SetBitNotSupportedForwardOnly");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);
            vector.Bits.ToArray();
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "GetBitsCompressedNotSupported");
    }
}
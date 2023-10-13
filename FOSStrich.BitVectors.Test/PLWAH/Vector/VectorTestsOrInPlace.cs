namespace FOSStrich.BitVectors.PLWAH;

public class VectorTestsOrInPlace
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OrInPlaceWithCompressibleTrueInput(bool isCompressed)
    {
        var vector = new Vector(false);
        vector[100] = true;

        var compressibleTrue = new Vector(isCompressed);
        compressibleTrue.SetBits(Enumerable.Range(0, 32).ToArray(), true);

        vector.OrInPlace(compressibleTrue);

        vector.AssertBitPositions(Enumerable.Range(0, 32), new[] { 100 });
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var vector = new Vector(false);
            vector.OrInPlace(null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "OrInPlaceArgumentNull");

        act = () =>
        {
            var vector = new Vector(true);
            var input = new Vector(false);
            vector.OrInPlace(input);
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "OrInPlaceNotSupported");
    }
}
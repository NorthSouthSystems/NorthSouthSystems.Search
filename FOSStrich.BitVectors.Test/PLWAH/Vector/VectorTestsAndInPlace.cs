namespace FOSStrich.BitVectors.PLWAH;

public class VectorTestsAndInPlace
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AndInPlaceRandom(bool isCompressed) =>
        VectorTestsRandom.LogicInPlaceBase(22, (Word.SIZE - 1) * 10 + 1, isCompressed, (left, right) => left.AndInPlace(right), Enumerable.Intersect);

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var vector = new Vector(false);
            vector.AndInPlace(null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "AndInPlaceArgumentNull");

        act = () =>
        {
            var vector = new Vector(true);
            var input = new Vector(false);
            vector.AndInPlace(input);
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "AndInPlaceNotSupported");
    }
}
namespace FOSStrich.BitVectors.PLWAH;

public class VectorTestsAndInPlace
{
    [Fact]
    public void AndInPlaceRandom() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
            VectorTestsRandom.LogicInPlaceBase(22, (Word.SIZE - 1) * 10 + 1, safetyAndCompression, (left, right) => left.AndInPlace(right), Enumerable.Intersect));

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var vector = new Vector(VectorCompression.None);
            vector.AndInPlace(null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "AndInPlaceArgumentNull");

        act = () =>
        {
            var vector = new Vector(VectorCompression.Compressed);
            var input = new Vector(VectorCompression.None);
            vector.AndInPlace(input);
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "AndInPlaceNotSupported");
    }
}
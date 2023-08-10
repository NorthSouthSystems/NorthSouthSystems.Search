namespace FOSStrich.Search;

public class VectorTestsOrInPlace
{
    [Fact]
    public void OrInPlaceCompressedWithCompressedTrueInput() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            var vector = new Vector(safetyAndCompression.AllowUnsafe, VectorCompression.None);
            vector[100] = true;

            Vector compressedTrue = new Vector(safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
            compressedTrue.SetBits(Enumerable.Range(0, 32).ToArray(), true);

            vector.OrInPlace(compressedTrue);

            vector.AssertBitPositions(Enumerable.Range(0, 32), new[] { 100 });
        });

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.None);
            vector.OrInPlace(null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "OrInPlaceArgumentNull");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);
            var input = new Vector(false, VectorCompression.None);
            vector.OrInPlace(input);
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "OrInPlaceNotSupported");
    }
}
namespace FOSStrich.Search;

[TestClass]
public class VectorTestsOrOutOfPlace
{
    [TestMethod]
    public void OrOutOfPlace() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            int[] bitPositionsA = new int[] { 0, 12, 16, 22, 34, 55, 110 };
            int[] bitPositionsB = new int[] { 0, 11, 16, 23, 34, 54, 110, 120 };
            int[] bitPositionsC = new int[] { 5, 10, 15, 20 };

            var vectorA = new Vector(safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
            vectorA.SetBits(bitPositionsA, true);

            var vectorB = new Vector(safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
            vectorB.SetBits(bitPositionsB, true);

            var vectorC = new Vector(safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
            vectorC.SetBits(bitPositionsC, true);

            Vector.OrOutOfPlace(vectorA, vectorB).AssertBitPositions(bitPositionsA, bitPositionsB);
            Vector.OrOutOfPlace(vectorA, vectorB, vectorC).AssertBitPositions(bitPositionsA, bitPositionsB, bitPositionsC);
        });

    [TestMethod]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            Vector[] vectors = null;
            Vector.OrOutOfPlace(vectors);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "OrOutOfPlaceArgumentNull1");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.None);
            Vector.OrOutOfPlace(vector, null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "OrOutOfPlaceArgumentNull2");

        act = () =>
        {
            var vector = Vector.OrOutOfPlace();
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "OrOutOfPlaceArgumentOutOfRange1");

        act = () =>
        {
            var vector1 = new Vector(false, VectorCompression.None);
            var vector = Vector.OrOutOfPlace(vector1);
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "OrOutOfPlaceArgumentOutOfRange2");
    }
}
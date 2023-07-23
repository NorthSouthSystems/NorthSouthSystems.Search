namespace FOSStrich.Search;

[TestClass]
public class VectorTestsAndInPlace
{
    [TestMethod]
    [TestProperty("Duration", "Long")]
    public void AndInPlaceRandom()
    {
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            VectorTestsRandom.LogicInPlaceBase(22, (Word.SIZE - 1) * 10 + 1, safetyAndCompression, (left, right) => left.AndInPlace(right), Enumerable.Intersect);
        });
    }

    #region Exceptions

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AndInPlaceArgumentNull()
    {
        var vector = new Vector(false, VectorCompression.None);
        vector.AndInPlace(null);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void AndInPlaceNotSupported()
    {
        var vector = new Vector(false, VectorCompression.Compressed);
        var input = new Vector(false, VectorCompression.None);
        vector.AndInPlace(input);
    }

    #endregion
}
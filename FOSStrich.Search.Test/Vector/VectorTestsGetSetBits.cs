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

    #region Exceptions

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexArgumentOutOfRange1()
    {
        var vector = new Vector(false, VectorCompression.None);
        vector[-1] = true;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexArgumentOutOfRange2()
    {
        var vector = new Vector(false, VectorCompression.None);
        bool value = vector[-1];
    }

    [TestMethod]
    public void SetBitSupportedForwardOnly()
    {
        var vector = new Vector(false, VectorCompression.Compressed);
        vector[30] = true;
        vector[61] = true;
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void SetBitNotSupportedForwardOnly()
    {
        var vector = new Vector(false, VectorCompression.Compressed);
        vector[30] = true;
        vector[31] = true;
        vector[30] = true;
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void GetBitsCompressedNotSupported()
    {
        var vector = new Vector(false, VectorCompression.Compressed);
        vector.Bits.ToArray();
    }

    #endregion
}
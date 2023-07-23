namespace FOSStrich.Search;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class VectorTestsOrInPlace
{
    [TestMethod]
    public void OrInPlaceCompressedWithCompressedTrueInput()
    {
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            Vector vector = new Vector(safetyAndCompression.AllowUnsafe, VectorCompression.None);
            vector[100] = true;

            Vector compressedTrue = new Vector(safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
            compressedTrue.SetBits(Enumerable.Range(0, 32).ToArray(), true);

            vector.OrInPlace(compressedTrue);

            vector.AssertBitPositions(Enumerable.Range(0, 32), new[] { 100 });
        });
    }

    #region Exceptions

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OrInPlaceArgumentNull()
    {
        var vector = new Vector(false, VectorCompression.None);
        vector.OrInPlace(null);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void OrInPlaceNotSupported()
    {
        var vector = new Vector(false, VectorCompression.Compressed);
        var input = new Vector(false, VectorCompression.None);
        vector.OrInPlace(input);
    }

    #endregion
}
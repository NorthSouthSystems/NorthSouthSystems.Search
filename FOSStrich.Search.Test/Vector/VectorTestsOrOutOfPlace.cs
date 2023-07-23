namespace FOSStrich.Search;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class VectorTestsOrOutOfPlace
{
    [TestMethod]
    public void OrOutOfPlace()
    {
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
    }

    #region Exceptions

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OrOutOfPlaceArgumentNull1()
    {
        Vector[] vectors = null;
        Vector.OrOutOfPlace(vectors);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OrOutOfPlaceArgumentNull2()
    {
        var vector = new Vector(false, VectorCompression.None);
        Vector.OrOutOfPlace(vector, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OrOutOfPlaceArgumentOutOfRange1()
    {
        var vector = Vector.OrOutOfPlace();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OrOutOfPlaceArgumentOutOfRange2()
    {
        var vector1 = new Vector(false, VectorCompression.None);
        var vector = Vector.OrOutOfPlace(vector1);
    }

    #endregion
}
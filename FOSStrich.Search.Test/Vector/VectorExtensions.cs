namespace FOSStrich.Search;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

internal static class VectorExtensions
{
    #region Assert

    internal static void AssertWordCounts(this Vector vector, int expectedWordCountPhysical, int expectedWordCountLogical)
    {
        Assert.AreEqual(expectedWordCountPhysical, (int)_wordCountPhysicalField.GetValue(vector));
        Assert.AreEqual(expectedWordCountLogical, (int)_wordCountLogicalField.GetValue(vector));
    }

    private static readonly FieldInfo _wordCountPhysicalField = typeof(Vector).GetField("_wordCountPhysical", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _wordCountLogicalField = typeof(Vector).GetField("_wordCountLogical", BindingFlags.Instance | BindingFlags.NonPublic);

    internal static void AssertWordLogicalValues(this Vector vector, params uint[] expectedWordLogicalValues)
    {
        for (int i = 0; i < expectedWordLogicalValues.Length; i++)
            Assert.AreEqual(expectedWordLogicalValues[i], vector.GetWordLogical(i).Raw, "i=" + i.ToString());
    }

    internal static void AssertBitPositions(this Vector vector, params IEnumerable<int>[] expectedBitPositionses)
    {
        int[] expectedBitPositions = expectedBitPositionses.SelectMany(bps => bps)
            .Distinct()
            .OrderBy(bitPosition => bitPosition)
            .ToArray();

        bool[] expectedBits = expectedBitPositions.Length == 0 ? new bool[0] : new bool[expectedBitPositions.Max() + 1];

        foreach (int expectedBitPosition in expectedBitPositions)
            expectedBits[expectedBitPosition] = true;

        CollectionAssert.AreEqual(expectedBitPositions, vector.GetBitPositions(true).ToArray());

        if (!vector.IsCompressed)
            CollectionAssert.AreEqual(expectedBits, vector.Bits.Reverse().SkipWhile(bit => !bit).Reverse().ToArray());

        foreach (int expectedBitPosition in expectedBitPositions)
            Assert.IsTrue(vector[expectedBitPosition]);

        Assert.AreEqual(expectedBitPositions.Length, vector.Population);
        Assert.AreEqual(expectedBitPositions.Length > 0, vector.PopulationAny);
    }

    #endregion

    #region Setting

    internal static void SetBits(this Vector vector, int[] bitPositions, bool value)
    {
        foreach (int bitPosition in bitPositions)
            vector[bitPosition] = value;
    }

    internal static int[] SetBitsRandom(this Vector vector, int maxBitPosition, int count, bool value)
    {
        int[] bitPositions = Enumerable.Range(0, maxBitPosition + 1)
            .Select(bitPosition => new { BitPosition = bitPosition, RandomDouble = _random.NextDouble() })
            .OrderBy(bitPosition => bitPosition.RandomDouble)
            .Take(count)
            .Select(bitPosition => bitPosition.BitPosition)
            .OrderBy(bitPosition => bitPosition)
            .ToArray();

        vector.SetBits(bitPositions, value);
        return bitPositions;
    }

    private static Random _random = new Random(4438);

    #endregion
}
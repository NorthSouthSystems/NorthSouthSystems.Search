namespace FOSStrich.Search;

using System;
using System.Collections.Generic;
using System.Linq;

internal static class VectorTestsRandom
{
    internal static void LogicInPlaceBase(int randomSeed, int maxBitPosition,
        SafetyAndCompression safetyAndCompression,
        Action<Vector, Vector> logic, Func<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>> expectedBitPositionCalculator)
    {
        var templates = GenerateRandomVectorsFavorCompression(randomSeed, maxBitPosition);

        foreach (var templateLeft in templates)
        {
            foreach (var templateRight in templates)
            {
                var resultLeft = new Vector(safetyAndCompression.AllowUnsafe, VectorCompression.None, templateLeft.Item1);
                var resultRight = new Vector(safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression, templateRight.Item1);

                logic(resultLeft, resultRight);

                resultLeft.AssertBitPositions(expectedBitPositionCalculator(templateLeft.Item2, templateRight.Item2));
            }
        }
    }

    internal static void LogicOutOfPlaceBase(int randomSeed, int maxBitPosition,
        bool allowUnsafe, VectorCompression leftVectorCompression, VectorCompression rightVectorCompression,
        Func<Vector, Vector, Vector> logic, Func<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>> expectedBitPositionCalculator)
    {
        var templates = GenerateRandomVectorsFavorCompression(randomSeed, maxBitPosition);

        foreach (var templateLeft in templates)
        {
            foreach (var templateRight in templates)
            {
                var resultLeft = new Vector(allowUnsafe, leftVectorCompression, templateLeft.Item1);
                var resultRight = new Vector(allowUnsafe, rightVectorCompression, templateRight.Item1);

                var result = logic(resultLeft, resultRight);

                result.AssertBitPositions(expectedBitPositionCalculator(templateLeft.Item2, templateRight.Item2));
            }
        }
    }

    private static Tuple<Vector, int[]>[] GenerateRandomVectorsFavorCompression(int randomSeed, int maxBitPosition)
    {
        var random = new Random(randomSeed);

        return Enumerable.Range(1, maxBitPosition - 1)
            .Where(count =>
            {
                double fillFactor = (double)count / maxBitPosition;
                double favorability = Math.Abs(fillFactor - .5) / .5;

                return random.NextDouble() <= favorability;
            })
            .Select(count =>
            {
                var vector = new Vector(false, VectorCompression.None);
                int[] bitPositions = vector.SetBitsRandom(maxBitPosition, count, true);

                return Tuple.Create(vector, bitPositions);
            })
            .ToArray();
    }
}
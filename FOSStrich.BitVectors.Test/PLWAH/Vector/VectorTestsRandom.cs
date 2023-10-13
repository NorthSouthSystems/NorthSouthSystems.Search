namespace FOSStrich.BitVectors.PLWAH;

internal static class VectorTestsRandom
{
    internal static void LogicInPlaceBase(int randomSeed, int maxBitPosition,
        bool isCompressed,
        Action<Vector, Vector> logic, Func<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>> expectedBitPositionCalculator)
    {
        var templates = GenerateRandomVectorsFavorCompression(randomSeed, maxBitPosition);

        foreach (var templateLeft in templates)
        {
            foreach (var templateRight in templates)
            {
                var resultLeft = new Vector(false, templateLeft.Item1);
                var resultRight = new Vector(isCompressed, templateRight.Item1);

                logic(resultLeft, resultRight);

                resultLeft.AssertBitPositions(expectedBitPositionCalculator(templateLeft.Item2, templateRight.Item2));
            }
        }
    }

    internal static void LogicOutOfPlaceBase(int randomSeed, int maxBitPosition,
        bool leftIsCompressed, bool rightIsCompressed,
        Func<Vector, Vector, Vector> logic, Func<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>> expectedBitPositionCalculator)
    {
        var templates = GenerateRandomVectorsFavorCompression(randomSeed, maxBitPosition);

        foreach (var templateLeft in templates)
        {
            foreach (var templateRight in templates)
            {
                var resultLeft = new Vector(leftIsCompressed, templateLeft.Item1);
                var resultRight = new Vector(rightIsCompressed, templateRight.Item1);

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
                var vector = new Vector(false);
                int[] bitPositions = vector.SetBitsRandom(maxBitPosition, count, true);

                return Tuple.Create(vector, bitPositions);
            })
            .ToArray();
    }
}
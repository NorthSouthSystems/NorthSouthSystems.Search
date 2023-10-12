﻿namespace FOSStrich.BitVectors.PLWAH;

using System.Reflection;

public class VectorTestsConstruction
{
    [Fact]
    public void ConstructCopy()
    {
        int[] fillMaxBitPositions = new int[] { 99, 499 };
        int[] fillCounts = new int[] { 0, 1, 2, 5, 10, 20, 30, 40, 50, 100, 200, 300, 400, 450, 460, 470, 480, 490, 495, 498, 499, 500 };

        var instructions =
             from sourceCompression in (int[])Enum.GetValues(typeof(VectorCompression))
             from resultCompression in (int[])Enum.GetValues(typeof(VectorCompression))
             from fillMaxBitPosition in fillMaxBitPositions
             from fillCount in fillCounts
             where fillCount <= fillMaxBitPosition + 1
             select new
             {
                 SourceCompression = (VectorCompression)sourceCompression,
                 ResultCompression = (VectorCompression)resultCompression,
                 FillMaxBitPosition = fillMaxBitPosition,
                 FillCount = fillCount
             };

        foreach (var instruction in instructions)
        {
            var source = new Vector(instruction.SourceCompression);
            source.Compression.Should().Be(instruction.SourceCompression);

            int[] bitPositions = source.SetBitsRandom(instruction.FillMaxBitPosition, instruction.FillCount, true);

            var result = new Vector(instruction.ResultCompression, source);
            result.Compression.Should().Be(instruction.ResultCompression);

            result.AssertBitPositions(bitPositions);

            var resultWords = (Word[])typeof(Vector).GetField("_words", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result);

            if (instruction.ResultCompression == VectorCompression.None)
                resultWords.Any(word => word.IsCompressed).Should().BeFalse();
            else if (instruction.ResultCompression == VectorCompression.Compressed)
                resultWords.Any(word => word.HasPackedWord).Should().BeFalse();

            source.WordsClear();
            source.Population.Should().Be(0);
            source.AssertBitPositions();

            result.WordsClear();
            result.Population.Should().Be(0);
            result.AssertBitPositions();
        }
    }
}
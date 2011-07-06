using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    public static class VectorTestExtensions
    {
        public static void AssertWordCounts(this Vector vector, int expectedWordCountPhysical, int expectedWordCountLogical)
        {
            PrivateObject poVector = new PrivateObject(vector);
            Assert.AreEqual(expectedWordCountPhysical, (int)poVector.GetField("_wordCountPhysical"));
            Assert.AreEqual(expectedWordCountLogical, (int)poVector.GetField("_wordCountLogical"));
        }

        public static void AssertWordLogicalValues(this Vector vector, params uint[] expectedWordLogicalValues)
        {
            for (int i = 0; i < expectedWordLogicalValues.Length; i++)
                Assert.AreEqual(expectedWordLogicalValues[i], vector.GetWordLogical(i).Raw, "i=" + i.ToString());
        }

        public static void AssertBitPositions(this Vector vector, params IEnumerable<int>[] bitPositionEnumerations)
        {
            int[] bitPositions = bitPositionEnumerations.SelectMany(enumeration => enumeration)
                .Distinct()
                .OrderBy(i => i)
                .ToArray();

            bool[] bits = bitPositions.Length == 0 ? new bool[0] : new bool[bitPositions.Max() + 1];

            foreach (int bitPosition in bitPositions)
                bits[bitPosition] = true;

            CollectionAssert.AreEqual(bitPositions, vector.GetBitPositions(true).ToArray());

            if (!vector.IsCompressed)
                CollectionAssert.AreEqual(bits, vector.GetBits().Reverse().SkipWhile(bit => !bit).Reverse().ToArray());

            foreach (int bitPosition in bitPositions)
                Assert.AreEqual(true, vector[bitPosition]);

            Assert.AreEqual(bitPositions.Length, vector.Population);
        }

        public static void Fill(this Vector vector, int[] bitPositions, bool value)
        {
            foreach (int bitPosition in bitPositions)
                vector[bitPosition] = value;
        }

        public static int[] RandomFill(this Vector vector, int maxBitPosition, int count)
        {
            HashSet<int> bitPositionSet = new HashSet<int>();

            while (bitPositionSet.Count < count)
            {
                int bitPosition = _random.Next(maxBitPosition);

                while (bitPositionSet.Contains(bitPosition))
                {
                    if (bitPosition == maxBitPosition)
                        bitPosition = 0;
                    else
                        bitPosition++;
                }

                bitPositionSet.Add(bitPosition);
            }

            int[] bitPositions = bitPositionSet.OrderBy(i => i).ToArray();
            vector.Fill(bitPositions, true);
            return bitPositions;
        }

        private static Random _random = new Random(4438);
    }
}
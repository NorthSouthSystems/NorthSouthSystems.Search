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
    }
}
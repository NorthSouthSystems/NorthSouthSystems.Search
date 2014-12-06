namespace SoftwareBotany.Sunlight
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VectorTestsAndOutOfPlace
    {
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void AndOutOfPlaceRandom()
        {
            const int randomSeed = 22;

            SafetyAndCompression.RunAllSafeties(allowUnsafe =>
                SafetyAndCompression.RunAllCompressions(leftCompression =>
                    SafetyAndCompression.RunAllCompressions(rightCompression =>
                        SafetyAndCompression.RunAllCompressions(resultCompression =>
                        {
                            VectorTestsRandom.LogicOutOfPlaceBase(randomSeed, (Word.SIZE - 1) * 10 + 1,
                                allowUnsafe, leftCompression, rightCompression,
                                (left, right) => left.AndOutOfPlace(right, resultCompression), Enumerable.Intersect);
                        }))));
        }
    }
}
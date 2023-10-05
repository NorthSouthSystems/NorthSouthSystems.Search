namespace FOSStrich.Search;

public class VectorTestsAndOutOfPlace
{
    [Fact]
    public void AndOutOfPlaceRandom()
    {
        const int randomSeed = 22;

        SafetyAndCompression.RunAllCompressions(leftCompression =>
            SafetyAndCompression.RunAllCompressions(rightCompression =>
                SafetyAndCompression.RunAllCompressions(resultCompression =>
                {
                    VectorTestsRandom.LogicOutOfPlaceBase(randomSeed, (Word.SIZE - 1) * 10 + 1,
                        leftCompression, rightCompression,
                        (left, right) => left.AndOutOfPlace(right, resultCompression), Enumerable.Intersect);
                })));
    }
}
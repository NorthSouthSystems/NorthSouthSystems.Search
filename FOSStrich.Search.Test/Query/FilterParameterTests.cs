namespace FOSStrich.Search;

[TestClass]
public class FilterParameterTests
{
    [TestMethod]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            var someStringParameter = FilterParameter.Create(someStringCatalog, null, "A");
            someStringParameter = FilterParameter.Create(someStringCatalog, "A", null);
        };
        act.Should().NotThrow(because: "FilterParameterRangeArgumentNullOK");

        act = () =>
        {
            var engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            var someStringParameter = FilterParameter.Create(someStringCatalog, null, null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FilterParameterRangeArgumentNull");

        act = () =>
        {
            var engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            var someStringParameter = FilterParameter.Create(someStringCatalog, "B", "A");
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "FilterParameterRangeArgumentOutOfRange");
    }
}
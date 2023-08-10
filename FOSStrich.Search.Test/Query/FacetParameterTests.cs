namespace FOSStrich.Search;

public class FacetParameterTests
{
    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someIntCatalog = engine.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

            var someIntFacet = FacetParameter.Create(someIntCatalog);

            var facet = someIntFacet.Facet;
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "FacetQueryNotExecuted");
    }
}
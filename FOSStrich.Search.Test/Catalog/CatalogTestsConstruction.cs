namespace FOSStrich.Search;

public class CatalogTestsConstruction
{
    [Fact]
    public void Public()
    {
        var catalog = new Catalog<int>("SomeInt", true, VectorCompression.None);
        catalog.Name.Should().Be("SomeInt");
        catalog.Compression.Should().Be(VectorCompression.None);
    }
}
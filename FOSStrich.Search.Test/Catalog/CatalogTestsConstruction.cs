namespace FOSStrich.Search;

public class CatalogTestsConstruction
{
    [Fact]
    public void Public()
    {
        var catalog = new Catalog<int>("SomeInt", true, false, VectorCompression.None);
        catalog.Name.Should().Be("SomeInt");
        catalog.AllowUnsafe.Should().BeFalse();
        catalog.Compression.Should().Be(VectorCompression.None);
    }
}
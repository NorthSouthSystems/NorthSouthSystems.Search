namespace FOSStrich.Search;

[TestClass]
public class CatalogTestsConstruction
{
    [TestMethod]
    public void Public()
    {
        var catalog = new Catalog<int>("SomeInt", true, false, VectorCompression.None);
        catalog.Name.Should().Be("SomeInt");
        catalog.AllowUnsafe.Should().Be(false);
        catalog.Compression.Should().Be(VectorCompression.None);
    }
}
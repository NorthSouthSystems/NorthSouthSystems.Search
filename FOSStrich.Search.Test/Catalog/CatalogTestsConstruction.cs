namespace FOSStrich.Search;

[TestClass]
public class CatalogTestsConstruction
{
    [TestMethod]
    public void Public()
    {
        var catalog = new Catalog<int>("SomeInt", true, false, VectorCompression.None);
        Assert.AreEqual("SomeInt", catalog.Name);
        Assert.AreEqual(false, catalog.AllowUnsafe);
        Assert.AreEqual(VectorCompression.None, catalog.Compression);
    }
}
namespace FOSStrich.Search;

using FOSStrich.BitVectors;

public class CatalogTestsConstruction
{
    [Theory]
    [ClassData(typeof(BitVectorFactories))]
    public void Public<TBitVector>(IBitVectorFactory<TBitVector> bitVectorFactory)
        where TBitVector : IBitVector<TBitVector>
    {
        var catalog = new Catalog<TBitVector, int>(bitVectorFactory, "SomeInt", true);
        catalog.Name.Should().Be("SomeInt");
    }
}
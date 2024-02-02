namespace NorthSouthSystems.Search;

using NorthSouthSystems.BitVectors;

public class FacetParameterTests
{
    [Theory]
    [ClassData(typeof(BitVectorFactories))]
    public void Exceptions<TBitVector>(IBitVectorFactory<TBitVector> bitVectorFactory)
        where TBitVector : IBitVector<TBitVector>
    {
        Action act;

        act = () =>
        {
            var engine = new Engine<TBitVector, EngineItem, int>(bitVectorFactory, item => item.Id);
            var someIntCatalog = engine.CreateCatalog("SomeInt", item => item.SomeInt);

            var someIntFacet = FacetParameter.Create(someIntCatalog);

            var facet = someIntFacet.Facet;
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "FacetQueryNotExecuted");
    }
}
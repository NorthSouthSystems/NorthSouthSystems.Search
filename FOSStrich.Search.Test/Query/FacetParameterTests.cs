namespace FOSStrich.Search;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class FacetParameterTests
{
    #region Exceptions

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void FacetQueryNotExecuted()
    {
        var engine = new Engine<EngineItem, int>(false, item => item.Id);
        var someIntCatalog = engine.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

        var someIntFacet = FacetParameter.Create(someIntCatalog);

        var facet = someIntFacet.Facet;
    }

    #endregion
}
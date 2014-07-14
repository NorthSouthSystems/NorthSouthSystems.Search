namespace SoftwareBotany.Sunlight
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FacetParameterTests
    {
        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void FacetQueryNotExecuted()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someIntCatalog = engine.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

            FacetParameter<int> someIntFacet = engine.CreateQuery()
                .AddFacetParameter(someIntCatalog);

            var facet = someIntFacet.Facet;
        }

        #endregion
    }
}
using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class FacetParameterTests
    {
        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void FacetSearchNotExecuted()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someIntFactory = engine.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

            FacetParameter<int> someIntFacet = engine.CreateSearch()
                .AddFacetParameter(someIntFactory);

            var facet = someIntFacet.Facet;
        }

        #endregion
    }
}
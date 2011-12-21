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
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(item => item.Id);
            var someIntFactory = engine.CreateCatalog("SomeInt", item => item.SomeInt);

            FacetParameter<int> someIntFacet;

            var search = engine.CreateSearch()
                .AddFacetParameter(someIntFactory, out someIntFacet);

            var facets = someIntFacet.Facets;
        }

        #endregion
    }
}
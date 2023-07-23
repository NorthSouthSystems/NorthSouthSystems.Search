namespace FreeAndWithBeer.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class FilterParameterTests
    {
        #region Exceptions

        [TestMethod]
        public void FilterParameterRangeArgumentNullOK()
        {
            var engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            var someStringParameter = FilterParameter.Create(someStringCatalog, null, "A");
            someStringParameter = FilterParameter.Create(someStringCatalog, "A", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterParameterRangeArgumentNull()
        {
            var engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            var someStringParameter = FilterParameter.Create(someStringCatalog, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FilterParameterRangeArgumentOutOfRange()
        {
            var engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            var someStringParameter = FilterParameter.Create(someStringCatalog, "B", "A");
        }

        #endregion
    }
}
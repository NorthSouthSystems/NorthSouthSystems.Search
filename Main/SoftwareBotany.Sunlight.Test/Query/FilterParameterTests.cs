namespace SoftwareBotany.Sunlight
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FilterParameterTests
    {
        #region Exceptions

        [TestMethod]
        public void FilterParameterRangeArgumentNullOK()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            FilterParameter<string> someStringParameter = engine.CreateQuery()
                .AddFilterRangeParameter(someStringCatalog, null, "A");

            someStringParameter = engine.CreateQuery()
                .AddFilterRangeParameter(someStringCatalog, "A", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterParameterRangeArgumentNull()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            FilterParameter<string> someStringParameter = engine.CreateQuery()
                .AddFilterRangeParameter(someStringCatalog, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FilterParameterRangeArgumentOutOfRange()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringCatalog = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            FilterParameter<string> someStringParameter = engine.CreateQuery()
                .AddFilterRangeParameter(someStringCatalog, "B", "A");
        }

        #endregion
    }
}
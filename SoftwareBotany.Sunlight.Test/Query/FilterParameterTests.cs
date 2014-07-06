using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class FilterParameterTests
    {
        #region Exceptions

        [TestMethod]
        public void FilterParameterRangeArgumentNullOK()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringFactory = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            FilterParameter<string> someStringParameter = engine.CreateQuery()
                .AddFilterRangeParameter(someStringFactory, null, "A");

            someStringParameter = engine.CreateQuery()
                .AddFilterRangeParameter(someStringFactory, "A", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterParameterRangeArgumentNull()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringFactory = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            FilterParameter<string> someStringParameter = engine.CreateQuery()
                .AddFilterRangeParameter(someStringFactory, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FilterParameterRangeArgumentOutOfRange()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringFactory = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            FilterParameter<string> someStringParameter = engine.CreateQuery()
                .AddFilterRangeParameter(someStringFactory, "B", "A");
        }

        #endregion
    }
}
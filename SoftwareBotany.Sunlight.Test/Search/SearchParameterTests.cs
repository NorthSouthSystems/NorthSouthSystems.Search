using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class SearchParameterTests
    {
        #region Exceptions

        [TestMethod]
        public void SearchParameterRangeArgumentNullOK()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringFactory = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            SearchParameter<string> someStringParameter = engine.CreateSearch()
                .AddSearchRangeParameter(someStringFactory, null, "A");

            someStringParameter = engine.CreateSearch()
                .AddSearchRangeParameter(someStringFactory, "A", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SearchParameterRangeArgumentNull()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringFactory = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            SearchParameter<string> someStringParameter = engine.CreateSearch()
                .AddSearchRangeParameter(someStringFactory, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SearchParameterRangeArgumentOutOfRange()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(false, item => item.Id);
            var someStringFactory = engine.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

            SearchParameter<string> someStringParameter = engine.CreateSearch()
                .AddSearchRangeParameter(someStringFactory, "B", "A");
        }

        #endregion
    }
}
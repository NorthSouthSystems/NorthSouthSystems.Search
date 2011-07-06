using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class SearchTests
    {
        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void EngineMismatch()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            Engine<EngineItem, int> engine2 = new Engine<EngineItem, int>(item => item.Id);
            var factory2 = engine2.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.CreateSearch().AddSearchExactParameter(factory2, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void DuplicateSearch()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.CreateSearch().AddSearchExactParameter(factory1, 1).AddSearchExactParameter(factory1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void PrimaryKeySortExists()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.CreateSearch()
                .AddSearchExactParameter(factory1, 1)
                .AddSortPrimaryKey(true)
                .AddSortDirectionalParameter(factory1, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void DuplicateSort()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.CreateSearch()
                .AddSearchExactParameter(factory1, 1)
                .AddSortDirectionalParameter(factory1, true)
                .AddSortDirectionalParameter(factory1, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void DuplicateProjection()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);
            var factory2 = engine1.CreateCatalog("SomeString", item => item.SomeString, true);

            engine1.CreateSearch()
                .AddSearchExactParameter(factory1, 1)
                .AddProjectionParameter(factory2)
                .AddProjectionParameter(factory2);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void SearchAlreadyExecuted()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            int totalCount;

            var search = engine1.CreateSearch().AddSearchExactParameter(factory1, 1);
            search.Execute(0, 1, out totalCount);
            search.Execute(0, 1, out totalCount);
        }

        [TestMethod]
        public void SanityNoExceptions()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);
            var factory2 = engine1.CreateCatalog("SomeString", item => item.SomeString, true);

            int totalCount;

            engine1.CreateSearch()
                .AddSearchExactParameter(factory1, 1)
                .AddSortDirectionalParameter(factory2, true)
                .AddSortPrimaryKey(true)
                .AddProjectionParameter(factory2)
                .Execute(0, 1, out totalCount);
        }

        #endregion
    }
}
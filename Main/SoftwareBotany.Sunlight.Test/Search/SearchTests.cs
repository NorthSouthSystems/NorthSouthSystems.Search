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
        [ExpectedException(typeof(ArgumentException))]
        public void EngineMismatch()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            using (Engine<EngineItem, int> engine2 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);
                var factory2 = engine2.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch().AddSearchExactParameter(factory2, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddAmongstNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch().AddAmongstPrimaryKeys((int[])null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSearchExactNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch().AddSearchExactParameter((ParameterFactory<int>)null, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSearchEnumerableNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch().AddSearchEnumerableParameter((ParameterFactory<int>)null, new[] { 1, 2 });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSearchRangeNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch().AddSearchRangeParameter((ParameterFactory<int>)null, 1, 3);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateSearch()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch().AddSearchExactParameter(factory1, 1).AddSearchExactParameter(factory1, 2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSortNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch().AddSortDirectionalParameter((ParameterFactory<int>)null, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PrimaryKeySortExists()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch()
                    .AddSearchExactParameter(factory1, 1)
                    .AddSortPrimaryKey(true)
                    .AddSortDirectionalParameter(factory1, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateSort()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch()
                    .AddSearchExactParameter(factory1, 1)
                    .AddSortDirectionalParameter(factory1, true)
                    .AddSortDirectionalParameter(factory1, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddProjectionNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                engine1.CreateSearch().AddProjectionParameter((ParameterFactory<int>)null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateProjection()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);
                var factory2 = engine1.CreateCatalog("SomeString", item => item.SomeString);

                engine1.CreateSearch()
                    .AddSearchExactParameter(factory1, 1)
                    .AddProjectionParameter(factory2)
                    .AddProjectionParameter(factory2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SearchAlreadyExecuted()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);

                int totalCount;

                var search = engine1.CreateSearch().AddSearchExactParameter(factory1, 1);
                search.Execute(0, 1, out totalCount);
                search.Execute(0, 1, out totalCount);
            }
        }

        [TestMethod]
        public void SanityNoExceptions()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt);
                var factory2 = engine1.CreateCatalog("SomeString", item => item.SomeString);

                int totalCount;

                engine1.CreateSearch()
                    .AddSearchExactParameter(factory1, 1)
                    .AddSortDirectionalParameter(factory2, true)
                    .AddSortPrimaryKey(true)
                    .AddProjectionParameter(factory2)
                    .Execute(0, 1, out totalCount);
            }
        }

        #endregion
    }
}
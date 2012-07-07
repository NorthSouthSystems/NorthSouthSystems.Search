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
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            using (Engine<EngineItem, int> engine2 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var factory2 = engine2.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateSearch().AddSearchExactParameter(factory2, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddAmongstNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateSearch().AddAmongstPrimaryKeys((int[])null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSearchExactNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateSearch().AddSearchExactParameter((ParameterFactory<int>)null, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSearchEnumerableNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateSearch().AddSearchEnumerableParameter((ParameterFactory<int>)null, new[] { 1, 2 });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSearchRangeNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateSearch().AddSearchRangeParameter((ParameterFactory<int>)null, 1, 3);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateSearch()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var search = engine1.CreateSearch();
                search.AddSearchExactParameter(factory1, 1);
                search.AddSearchExactParameter(factory1, 2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSortNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateSearch().AddSortDirectionalParameter((ParameterFactory<int>)null, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PrimaryKeySortExists()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var search = engine1.CreateSearch();
                search.AddSearchExactParameter(factory1, 1);
                search.SortPrimaryKeyAscending = true;
                search.AddSortDirectionalParameter(factory1, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateSort()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var search = engine1.CreateSearch();
                search.AddSearchExactParameter(factory1, 1);
                search.AddSortDirectionalParameter(factory1, true);
                search.AddSortDirectionalParameter(factory1, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFacetNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateSearch().AddFacetParameter((ParameterFactory<int>)null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateFacet()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var factory2 = engine1.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

                var search = engine1.CreateSearch();
                search.AddSearchExactParameter(factory1, 1);
                search.AddFacetParameter(factory2);
                search.AddFacetParameter(factory2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SearchAlreadyExecuted()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                int totalCount;

                var search = engine1.CreateSearch();
                search.AddSearchExactParameter(factory1, 1);
                search.Execute(0, 1, out totalCount);
                search.Execute(0, 1, out totalCount);
            }
        }

        [TestMethod]
        public void SanityNoExceptions()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var factory2 = engine1.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

                int totalCount;

                var search = engine1.CreateSearch();
                search.AddSearchExactParameter(factory1, 1);
                search.AddSortDirectionalParameter(factory2, true);
                search.SortPrimaryKeyAscending = true;
                search.AddFacetParameter(factory2);
                search.Execute(0, 1, out totalCount);
            }
        }

        #endregion
    }
}
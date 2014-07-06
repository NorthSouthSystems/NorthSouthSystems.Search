using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class QueryTests
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

                engine1.CreateQuery().AddFilterExactParameter(factory2, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddAmongstNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddAmongstPrimaryKeys((int[])null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterExactNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFilterExactParameter((ParameterFactory<int>)null, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterEnumerableNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFilterEnumerableParameter((ParameterFactory<int>)null, new[] { 1, 2 });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterRangeNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFilterRangeParameter((ParameterFactory<int>)null, 1, 3);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateFilter()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(factory1, 1);
                query.AddFilterExactParameter(factory1, 2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSortNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddSortParameter((ParameterFactory<int>)null, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PrimaryKeySortExists()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(factory1, 1);
                query.SortPrimaryKeyAscending = true;
                query.AddSortParameter(factory1, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateSort()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(factory1, 1);
                query.AddSortParameter(factory1, true);
                query.AddSortParameter(factory1, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFacetNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFacetParameter((ParameterFactory<int>)null);
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

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(factory1, 1);
                query.AddFacetParameter(factory2);
                query.AddFacetParameter(factory2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void QueryAlreadyExecuted()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var factory1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                int totalCount;

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(factory1, 1);
                query.Execute(0, 1, out totalCount);
                query.Execute(0, 1, out totalCount);
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

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(factory1, 1);
                query.AddSortParameter(factory2, true);
                query.SortPrimaryKeyAscending = true;
                query.AddFacetParameter(factory2);
                query.Execute(0, 1, out totalCount);
            }
        }

        #endregion
    }
}
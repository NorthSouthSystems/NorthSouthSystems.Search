namespace SoftwareBotany.Sunlight
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var catalog2 = engine2.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFilterExactParameter(catalog2, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddAmongstNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddAmongstPrimaryKeys((int[])null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterExactNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFilterExactParameter((ICatalogHandle<int>)null, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterEnumerableNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFilterEnumerableParameter((ICatalogHandle<int>)null, new[] { 1, 2 });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterRangeNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFilterRangeParameter((ICatalogHandle<int>)null, 1, 3);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateFilter()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(catalog1, 1);
                query.AddFilterExactParameter(catalog1, 2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSortNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddSortParameter((ICatalogHandle<int>)null, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PrimaryKeySortExists()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(catalog1, 1);
                query.SortPrimaryKeyAscending = true;
                query.AddSortParameter(catalog1, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateSort()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(catalog1, 1);
                query.AddSortParameter(catalog1, true);
                query.AddSortParameter(catalog1, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFacetNull()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().AddFacetParameter((ICatalogHandle<int>)null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateFacet()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var catalog2 = engine1.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(catalog1, 1);
                query.AddFacetParameter(catalog2);
                query.AddFacetParameter(catalog2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void QueryAlreadyExecuted()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                int totalCount;

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(catalog1, 1);
                query.Execute(0, 1, out totalCount);
                query.Execute(0, 1, out totalCount);
            }
        }

        [TestMethod]
        public void SanityNoExceptions()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var catalog2 = engine1.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

                int totalCount;

                var query = engine1.CreateQuery();
                query.AddFilterExactParameter(catalog1, 1);
                query.AddSortParameter(catalog2, true);
                query.SortPrimaryKeyAscending = true;
                query.AddFacetParameter(catalog2);
                query.Execute(0, 1, out totalCount);
            }
        }

        #endregion
    }
}
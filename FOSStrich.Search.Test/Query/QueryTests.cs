namespace FOSStrich.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class QueryTests
    {
        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EngineMismatch()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            using (var engine2 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var catalog2 = engine2.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().Filter(FilterParameter.Create(catalog2, 1));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterExactNull()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().Filter(FilterParameter.Create((ICatalogHandle<int>)null, 1));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterEnumerableNull()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().Filter(FilterParameter.Create((ICatalogHandle<int>)null, new[] { 1, 2 }));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFilterRangeNull()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().Filter(FilterParameter.Create((ICatalogHandle<int>)null, 1, 3));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddSortNull()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().Sort(SortParameter.Create((ICatalogHandle<int>)null, true));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PrimaryKeySortExists()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery()
                    .Filter(FilterParameter.Create(catalog1, 1))
                    .SortPrimaryKey(true)
                    .Sort(SortParameter.Create(catalog1, true));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateSort()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery()
                    .Filter(FilterParameter.Create(catalog1, 1))
                    .Sort(SortParameter.Create(catalog1, true),
                        SortParameter.Create(catalog1, true));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFacetNull()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.CreateQuery().Facet(FacetParameter.Create((ICatalogHandle<int>)null));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DuplicateFacet()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var catalog2 = engine1.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

                var query = engine1.CreateQuery()
                    .Filter(FilterParameter.Create(catalog1, 1))
                    .Facet(FacetParameter.Create(catalog2),
                        FacetParameter.Create(catalog2));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void QueryAlreadyExecuted()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                var query = engine1.CreateQuery()
                    .Filter(FilterParameter.Create(catalog1, 1))
                    .Execute(0, 1)
                    .Execute(0, 1);
            }
        }

        [TestMethod]
        public void SanityNoExceptions()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                var catalog2 = engine1.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);

                var query = engine1.CreateQuery()
                    .Filter(FilterParameter.Create(catalog1, 1))
                    .Sort(SortParameter.Create(catalog2, true))
                    .SortPrimaryKey(true)
                    .Facet(FacetParameter.Create(catalog2))
                    .Execute(0, 1);
            }
        }

        #endregion
    }
}
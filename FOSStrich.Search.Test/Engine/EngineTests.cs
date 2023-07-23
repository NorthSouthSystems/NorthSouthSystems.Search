namespace FOSStrich.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;

    [TestClass]
    public class EngineTests
    {
        [TestMethod]
        public void Construction()
        {
            using (var engine = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                Assert.AreEqual(false, engine.AllowUnsafe);
            }
        }

        private class SimpleItem
        {
            public int Id;
            public int SomeInt;
        }

        [TestMethod]
        public void AmongstPrimaryKeyOutOfRange()
        {
            SafetyAndCompression.RunAll(safetyAndCompression =>
            {
                using (var engine1 = new Engine<SimpleItem, int>(safetyAndCompression.AllowUnsafe, item => item.Id))
                {
                    var catalog1 = engine1.CreateCatalog("SomeInt", safetyAndCompression.Compression, item => item.SomeInt);

                    engine1.Add(new SimpleItem { Id = 43, SomeInt = 0 });

                    var query = engine1.CreateQuery();
                    query.Amongst(new[] { 43, 44 });

                    query.Execute(0, 10);

                    Assert.AreEqual(1, query.ResultTotalCount);
                    Assert.AreEqual(1, query.ResultPrimaryKeys.Length);
                    Assert.AreEqual(43, query.ResultPrimaryKeys[0]);
                }
            });
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreateCatalogNotInitializing()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.Add(EngineItem.CreateItems(id => id, id => DateTime.Now, id => id.ToString(), id => new string[0], 1).Single());

                var catalog2 = engine1.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCatalogDuplicateName()
        {
            using (var engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("Name", VectorCompression.None, item => item.SomeInt);
                var catalog2 = engine1.CreateCatalog("Name", VectorCompression.None, item => item.SomeString);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDuplicatePrimaryKey()
        {
            using (var engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
                engine1.Add(new SimpleItem { Id = 0, SomeInt = 1 });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRangeNull()
        {
            using (Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                engine1.Add((SimpleItem[])null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateNoPrimaryKey()
        {
            using (var engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
                engine1.Update(new SimpleItem { Id = 1, SomeInt = 1 });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateRangeNull()
        {
            using (var engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                engine1.Update((SimpleItem[])null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveNoPrimaryKey()
        {
            using (var engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
                engine1.Remove(new SimpleItem { Id = 1, SomeInt = 1 });
            }
        }

        [TestMethod]
        public void RemoveReAddPrimaryKey()
        {
            using (var engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
                engine1.Remove(new SimpleItem { Id = 0, SomeInt = 0 });
                engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveRangeNull()
        {
            using (var engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                engine1.Remove((SimpleItem[])null);
            }
        }

        #endregion
    }
}
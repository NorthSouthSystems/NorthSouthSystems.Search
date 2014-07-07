using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class EngineTests
    {
        [TestMethod]
        public void Construction()
        {
            using (Engine<SimpleItem, int> engine = new Engine<SimpleItem, int>(false, item => item.Id))
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
        public void AmongstPrimaryKeyOutOfRange() { SafetyVectorCompressionTuple.RunAll(AmongstPrimaryKeyOutOfRangeBase); }

        private static void AmongstPrimaryKeyOutOfRangeBase(SafetyVectorCompressionTuple safetyVectorCompression)
        {
            using (Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(safetyVectorCompression.AllowUnsafe, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", safetyVectorCompression.Compression, item => item.SomeInt);

                engine1.Add(new SimpleItem { Id = 43, SomeInt = 0 });

                int totalCount;
                
                var query = engine1.CreateQuery();
                query.AddAmongstPrimaryKeys(new[] { 43, 44 });

                int[] primaryKeys = query.Execute(0, 10, out totalCount);

                Assert.AreEqual(1, totalCount);
                Assert.AreEqual(1, primaryKeys.Length);
                Assert.AreEqual(43, primaryKeys[0]);
            }
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreateCatalogNotInitializing()
        {
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
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
            using (Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("Name", VectorCompression.None, item => item.SomeInt);
                var catalog2 = engine1.CreateCatalog("Name", VectorCompression.None, item => item.SomeString);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDuplicatePrimaryKey()
        {
            using (Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
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
            using (Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
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
            using (Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                engine1.Update((SimpleItem[])null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveNoPrimaryKey()
        {
            using (Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

                engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
                engine1.Remove(new SimpleItem { Id = 1, SomeInt = 1 });
            }
        }

        [TestMethod]
        public void RemoveReAddPrimaryKey()
        {
            using (Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
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
            using (Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(false, item => item.Id))
            {
                var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
                engine1.Remove((SimpleItem[])null);
            }
        }

        #endregion
    }
}
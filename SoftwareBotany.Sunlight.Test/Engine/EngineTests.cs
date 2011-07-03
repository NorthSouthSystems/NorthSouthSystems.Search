using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class EngineTests
    {
        private class SimpleItem
        {
            public int Id;
            public int SomeInt;
        }

        [TestMethod]
        public void AmongstPrimaryKeyOutOfRange()
        {
            Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.Add(new SimpleItem { Id = 43, SomeInt = 0 });
            
            int totalCount;
            int[] primaryKeys = engine1.CreateSearch().AddAmongstPrimaryKeys(new[] { 43, 44 }).Execute(0, 10, out totalCount);

            Assert.AreEqual(1, totalCount);
            Assert.AreEqual(1, primaryKeys.Length);
            Assert.AreEqual(43, primaryKeys[0]);
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CreateCatalogNotInitializing()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.Add(EngineItem.CreateItems(id => id, id => DateTime.Now, id => id.ToString(), id => new string[0], 1).Single());

            var factory2 = engine1.CreateCatalog("SomeString", item => item.SomeString, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCatalogDuplicateName()
        {
            Engine<EngineItem, int> engine1 = new Engine<EngineItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("Name", item => item.SomeInt, true);
            var factory2 = engine1.CreateCatalog("Name", item => item.SomeString, true);           
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDuplicatePrimaryKey()
        {
            Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
            engine1.Add(new SimpleItem { Id = 0, SomeInt = 1 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateNoPrimaryKey()
        {
            Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
            engine1.Update(new SimpleItem { Id = 1, SomeInt = 1 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveNoPrimaryKey()
        {
            Engine<SimpleItem, int> engine1 = new Engine<SimpleItem, int>(item => item.Id);
            var factory1 = engine1.CreateCatalog("SomeInt", item => item.SomeInt, true);

            engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
            engine1.Remove(new SimpleItem { Id = 1, SomeInt = 1 });
        }

        #endregion
    }
}
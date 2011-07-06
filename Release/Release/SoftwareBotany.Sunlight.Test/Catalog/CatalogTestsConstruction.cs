using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class CatalogTestsConstruction
    {
        [TestMethod]
        public void Public()
        {
            Catalog<int> catalog = new Catalog<int>("SomeInt", true);
            Assert.AreEqual("SomeInt", catalog.Name);
            Assert.AreEqual(true, catalog.IsCompressed);

            catalog = new Catalog<int>("SomeInt", false);
            Assert.AreEqual("SomeInt", catalog.Name);
            Assert.AreEqual(false, catalog.IsCompressed);
        }
    }
}
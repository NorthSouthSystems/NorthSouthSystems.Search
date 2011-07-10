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
            Catalog<int> catalog = new Catalog<int>("SomeInt");
            Assert.AreEqual("SomeInt", catalog.Name);
        }
    }
}
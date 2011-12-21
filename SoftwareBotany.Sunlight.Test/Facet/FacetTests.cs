using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class FacetTests
    {
        [TestMethod]
        public void EqualityAndHashing()
        {
            Facet<int> facet1 = new Facet<int>(1, 1);
            Facet<int> facet2 = new Facet<int>(1, 1);

            Assert.AreEqual(true, facet1.Equals(facet2));
            Assert.AreEqual(true, facet1.Equals((object)facet2));
            Assert.AreEqual(false, facet1.Equals(null));
            Assert.AreEqual(false, facet1.Equals("test"));
            Assert.AreEqual(true, facet1 == facet2);
            Assert.AreEqual(false, facet1 != facet2);
            Assert.AreEqual(facet1.GetHashCode(), facet2.GetHashCode());

            facet2 = new Facet<int>(2, 1);

            Assert.AreEqual(false, facet1.Equals(facet2));
            Assert.AreEqual(false, facet1.Equals((object)facet2));
            Assert.AreEqual(false, facet1 == facet2);
            Assert.AreEqual(true, facet1 != facet2);

            facet2 = new Facet<int>(1, 2);

            Assert.AreEqual(false, facet1.Equals(facet2));
            Assert.AreEqual(false, facet1.Equals((object)facet2));
            Assert.AreEqual(false, facet1 == facet2);
            Assert.AreEqual(true, facet1 != facet2);
        }
    }
}
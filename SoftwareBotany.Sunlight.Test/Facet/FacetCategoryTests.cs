using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class FacetCategoryTests
    {
        [TestMethod]
        public void EqualityAndHashing()
        {
            var facet1 = new FacetCategory<int>(1, 1);
            var facet2 = new FacetCategory<int>(1, 1);

            Assert.AreEqual(true, facet1.Equals(facet2));
            Assert.AreEqual(true, facet1.Equals((object)facet2));
            Assert.AreEqual(false, facet1.Equals(null));
            Assert.AreEqual(false, facet1.Equals("test"));
            Assert.AreEqual(true, facet1 == facet2);
            Assert.AreEqual(false, facet1 != facet2);
            Assert.AreEqual(facet1.GetHashCode(), facet2.GetHashCode());

            facet2 = new FacetCategory<int>(2, 1);

            Assert.AreEqual(false, facet1.Equals(facet2));
            Assert.AreEqual(false, facet1.Equals((object)facet2));
            Assert.AreEqual(false, facet1 == facet2);
            Assert.AreEqual(true, facet1 != facet2);

            facet2 = new FacetCategory<int>(1, 2);

            Assert.AreEqual(false, facet1.Equals(facet2));
            Assert.AreEqual(false, facet1.Equals((object)facet2));
            Assert.AreEqual(false, facet1 == facet2);
            Assert.AreEqual(true, facet1 != facet2);
        }
    }
}
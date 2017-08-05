namespace Kangarooper.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FacetCategoryTests
    {
        [TestMethod]
        public void EqualityAndHashing()
        {
            var category1 = new FacetCategory<int>(1, 1);
            var category2 = new FacetCategory<int>(1, 1);

            Assert.AreEqual(true, category1.Equals(category2));
            Assert.AreEqual(true, category1.Equals((object)category2));
            Assert.AreEqual(false, category1.Equals(null));
            Assert.AreEqual(false, category1.Equals("test"));
            Assert.AreEqual(true, category1 == category2);
            Assert.AreEqual(false, category1 != category2);
            Assert.AreEqual(category1.GetHashCode(), category2.GetHashCode());

            category2 = new FacetCategory<int>(2, 1);

            Assert.AreEqual(false, category1.Equals(category2));
            Assert.AreEqual(false, category1.Equals((object)category2));
            Assert.AreEqual(false, category1 == category2);
            Assert.AreEqual(true, category1 != category2);

            category2 = new FacetCategory<int>(1, 2);

            Assert.AreEqual(false, category1.Equals(category2));
            Assert.AreEqual(false, category1.Equals((object)category2));
            Assert.AreEqual(false, category1 == category2);
            Assert.AreEqual(true, category1 != category2);
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class ProjectionTests
    {
        [TestMethod]
        public void EqualityAndHashing()
        {
            Projection<int> proj1 = new Projection<int>(1, 1);
            Projection<int> proj2 = new Projection<int>(1, 1);

            Assert.AreEqual(true, proj1.Equals(proj2));
            Assert.AreEqual(true, proj1.Equals((object)proj2));
            Assert.AreEqual(false, proj1.Equals(null));
            Assert.AreEqual(false, proj1.Equals("test"));
            Assert.AreEqual(true, proj1 == proj2);
            Assert.AreEqual(false, proj1 != proj2);
            Assert.AreEqual(proj1.GetHashCode(), proj2.GetHashCode());

            proj2 = new Projection<int>(2, 1);

            Assert.AreEqual(false, proj1.Equals(proj2));
            Assert.AreEqual(false, proj1.Equals((object)proj2));
            Assert.AreEqual(false, proj1 == proj2);
            Assert.AreEqual(true, proj1 != proj2);

            proj2 = new Projection<int>(1, 2);

            Assert.AreEqual(false, proj1.Equals(proj2));
            Assert.AreEqual(false, proj1.Equals((object)proj2));
            Assert.AreEqual(false, proj1 == proj2);
            Assert.AreEqual(true, proj1 != proj2);
        }
    }
}
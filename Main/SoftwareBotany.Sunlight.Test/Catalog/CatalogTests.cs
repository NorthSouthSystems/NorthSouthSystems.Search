using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class CatalogTests
    {
        [TestMethod]
        public void SortBitPositions()
        {
            Catalog<int> catalog = new Catalog<int>("SomeInt");
            catalog.Set(0, 5, true);
            catalog.Set(1, 6, true);
            catalog.Set(2, 7, true);
            catalog.Set(3, 8, true);
            catalog.Set(4, 9, true);

            Vector vector = new Vector(false);
            vector[4] = true;
            vector[5] = true;
            vector[6] = true;
            vector[7] = true;
            vector[8] = true;
            vector[9] = true;
            vector[10] = true;

            CatalogSortResult<int> result = catalog.SortBitPositions(vector, true, false);
            int[] bitPositions = result.SelectMany(partial => partial).ToArray();
            CollectionAssert.AreEqual(new[] { 9, 8, 7, 6, 5 }, bitPositions);

            CatalogPartialSortResult<int>[] partialResults = result.ToArray();
            Assert.AreEqual(5, partialResults.Length);

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(4 - i, partialResults[i].Key);
                Assert.AreEqual(9 - i, partialResults[i].Single());
            }
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetRangeNull()
        {
            Catalog<int> catalog = new Catalog<int>("SomeInt");
            catalog.Set((int[])null, 777, true);
        }

        #endregion
    }
}
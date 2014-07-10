using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class CatalogTests
    {
        [TestMethod]
        public void SortBitPositions() { SafetyVectorCompressionTuple.RunAll(SortBitPositionsBase); }

        private static void SortBitPositionsBase(SafetyVectorCompressionTuple safetyVectorCompression)
        {
            Catalog<int> catalog = new Catalog<int>("SomeInt", true, safetyVectorCompression.AllowUnsafe, safetyVectorCompression.Compression);
            catalog.Set(0, 5, true);
            catalog.Set(1, 6, true);
            catalog.Set(2, 7, true);
            catalog.Set(3, 8, true);
            catalog.Set(4, 9, true);

            Vector vector = new Vector(safetyVectorCompression.AllowUnsafe, VectorCompression.None);
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
        public void SetNull()
        {
            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Set((string)null, 777, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetEnumerableNull()
        {
            Catalog<int> catalog = new Catalog<int>("SomeInt", true, false, VectorCompression.None);
            catalog.Set((int[])null, 777, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterExactVectorNull()
        {
            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterExactKeyNull()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);

            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterEnumerableVectorNull()
        {
            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, new[] { "A", "B" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterEnumerableKeysNull()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);

            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string[])null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterEnumerableKeysKeyNull()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);

            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, new[] { "A", null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterRangeVectorNull()
        {
            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, "A", "B");
        }

        [TestMethod]
        public void FilterRangeKeyMinMaxOK()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);

            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null, "A");
            catalog.Filter(vector, "A", (string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterRangeKeyMinMaxNull()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);

            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null, (string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FilterRangeKeyMinMaxOutOfRange()
        {
            Vector vector = new Vector(false, VectorCompression.Compressed);

            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, "B", "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FacetVectorNull()
        {
            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Facet(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SortBitPositionsVectorNull()
        {
            Catalog<string> catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.SortBitPositions(null, true, true);
        }

        #endregion
    }
}
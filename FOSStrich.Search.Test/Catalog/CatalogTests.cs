namespace FreeAndWithBeer.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;

    [TestClass]
    public class CatalogTests
    {
        [TestMethod]
        public void SortBitPositions()
        {
            SafetyAndCompression.RunAll(safetyAndCompression =>
            {
                var catalog = new Catalog<int>("SomeInt", true, safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
                catalog.Set(0, 5, true);
                catalog.Set(1, 6, true);
                catalog.Set(2, 7, true);
                catalog.Set(3, 8, true);
                catalog.Set(4, 9, true);

                var vector = new Vector(safetyAndCompression.AllowUnsafe, VectorCompression.None);
                vector[4] = true;
                vector[5] = true;
                vector[6] = true;
                vector[7] = true;
                vector[8] = true;
                vector[9] = true;
                vector[10] = true;

                foreach (bool disableParallel in new[] { false, true })
                {
                    var result = catalog.Sort(vector, true, false, disableParallel);
                    int[] bitPositions = result.PartialSorts.SelectMany(partial => partial.GetBitPositions(true)).ToArray();
                    CollectionAssert.AreEqual(new[] { 9, 8, 7, 6, 5 }, bitPositions);

                    var partialSorts = result.PartialSorts.ToArray();
                    Assert.AreEqual(5, partialSorts.Length);

                    for (int i = 0; i < 5; i++)
                        Assert.AreEqual(9 - i, partialSorts[i].GetBitPositions(true).Single());
                }
            });
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNull()
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Set((string)null, 777, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetEnumerableNull()
        {
            var catalog = new Catalog<int>("SomeInt", true, false, VectorCompression.None);
            catalog.Set((int[])null, 777, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterExactVectorNull()
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterExactKeyNull()
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterEnumerableVectorNull()
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, new[] { "A", "B" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterEnumerableKeysNull()
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string[])null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterEnumerableKeysKeyNull()
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, new[] { "A", null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterRangeVectorNull()
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, "A", "B");
        }

        [TestMethod]
        public void FilterRangeKeyMinMaxOK()
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null, "A");
            catalog.Filter(vector, "A", (string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterRangeKeyMinMaxNull()
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null, (string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FilterRangeKeyMinMaxOutOfRange()
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, "B", "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FacetVectorNull()
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Facet(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SortBitPositionsVectorNull()
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Sort(null, true, true, false);
        }

        #endregion
    }
}
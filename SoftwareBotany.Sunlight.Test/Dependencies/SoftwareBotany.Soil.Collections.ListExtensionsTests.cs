using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class ListExtensionsTests
    {
        [TestMethod]
        public void BinarySearchIntEmpty()
        {
            // Must cast to IList in order to use the Extension method and not List<T>.BinarySearch.
            IList<int> ints = new List<int>();

            int index = ints.BinarySearch(-1);
            Assert.AreEqual(0, ~index);

            index = ints.BinarySearch(0);
            Assert.AreEqual(0, ~index);

            index = ints.BinarySearch(1);
            Assert.AreEqual(0, ~index);
        }

        [TestMethod]
        public void BinarySearchInt()
        {
            foreach (int startValue in Enumerable.Range(-2, 5))
                foreach (int length in Enumerable.Range(1, 5))
                    foreach (int step in Enumerable.Range(1, 5))
                        BinarySearchIntBase(startValue, length, step);
        }

        [TestMethod]
        public void BinarySearchIntLengthy()
        {
            foreach (int startValue in Enumerable.Range(-2, 5))
                foreach (int step in Enumerable.Range(1, 5))
                    BinarySearchIntBase(startValue, 100, step);
        }

        private void BinarySearchIntBase(int startValue, int length, int step)
        {
            // Must cast to IList in order to use the Extension method and not List<T>.BinarySearch.
            IList<int> ints = new List<int>();
            int value;

            for (value = startValue; value < startValue + (length * step); value += step)
                ints.Add(value);

            value = startValue - 1;
            int index = ints.BinarySearch(value);
            Assert.AreEqual(0, ~index);

            for (value = startValue; value < startValue + (length * step); value++)
            {
                bool found = ((value - startValue) % step) == 0;
                int expectedIndex = ((value - startValue) / step) + (found ? 0 : 1);

                index = ints.BinarySearch(value);
                Assert.AreEqual(expectedIndex, found ? index : ~index);
            }

            index = ints.BinarySearch(value);
            Assert.AreEqual(length, ~index);
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BinarySearchArgumentNull()
        {
            IList<int> ints = null;
            ints.BinarySearch(0);
        }

        #endregion
    }
}
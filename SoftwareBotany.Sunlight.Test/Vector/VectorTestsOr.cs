using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsOr
    {
        [TestMethod]
        public void OrCompressedWithCompressedTrueInput()
        {
            Vector vector = new Vector(false);
            vector[100] = true;

            Vector compressedTrue = new Vector(true);
            compressedTrue.Fill(Enumerable.Range(0, 32).ToArray(), true);

            vector.Or(compressedTrue);

            vector.AssertBitPositions(Enumerable.Range(0, 32), new[] { 100 });
        }

        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrArgumentNull()
        {
            Vector vector = new Vector(false);
            vector.Or(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void OrNotSupported()
        {
            Vector vector = new Vector(true);
            Vector input = new Vector(false);
            vector.Or(input);
        }

        #endregion
    }
}
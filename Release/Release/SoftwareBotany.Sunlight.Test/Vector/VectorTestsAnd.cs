using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class VectorTestsAnd
    {
        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndArgumentNull()
        {
            Vector vector = new Vector(false);
            vector.And(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndNotSupported()
        {
            Vector vector = new Vector(true);
            Vector input = new Vector(false);
            vector.And(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndPopulationArgumentNull()
        {
            Vector vector = new Vector(false);
            vector.AndPopulation(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndPopulationNotSupported()
        {
            Vector vector = new Vector(true);
            Vector input = new Vector(false);
            vector.AndPopulation(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndFilterArgumentNull()
        {
            Vector vector = new Vector(false);
            vector.AndFilterBitPositions(null, true).ToArray();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AndFilterNotSupported()
        {
            Vector vector = new Vector(true);
            Vector input = new Vector(false);
            vector.AndFilterBitPositions(input, true).ToArray();
        }

        #endregion
    }
}
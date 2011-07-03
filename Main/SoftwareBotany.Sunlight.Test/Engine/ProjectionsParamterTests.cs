using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class ProjectionParameterTests
    {
        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ProjectionSearchNotExecuted()
        {
            Engine<EngineItem, int> engine = new Engine<EngineItem, int>(item => item.Id);
            var someIntFactory = engine.CreateCatalog("SomeInt", item => item.SomeInt, true);

            ProjectionParameter<int> someIntProjection;

            var search = engine.CreateSearch()
                .AddProjectionParameter(someIntFactory, out someIntProjection);

            var projections = someIntProjection.Projections;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class EngineTestsBroad
    {
        [TestMethod]
        public void Small() { Base(50); }

        [TestMethod]
        public void Medium() { Base(500); }

        [TestMethod]
        public void Large() { Base(5000); }

        private void Base(int size)
        {
            using (var engine = new Engine<EngineItem, int>(item => item.Id))
            {
                var someIntFactory = engine.CreateCatalog("SomeInt", item => item.SomeInt);
                var someDateTimeFactory = engine.CreateCatalog("SomeDateTime", item => item.SomeDateTime);
                var someStringFactory = engine.CreateCatalog("SomeString", item => item.SomeString);
                var someTagsFactory = engine.CreateCatalog<string>("SomeTags", item => item.SomeTags);

                EngineItem[] items = EngineItem.CreateItems(id => id / 5, id => new DateTime(2011, 1, 1).AddDays(id / 7), id => id.ToString(), id => new[] { id, id / 2, id / 3, id / 5 }.Distinct().Select(i => i.ToString()).ToArray(), size);

                foreach (EngineItem item in items)
                    engine.Add(item);

                for (int i = 0; i < 3; i++)
                {
                    Search<int> search = engine.CreateSearch()
                        .AddSearchExactParameter(someIntFactory, 0)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 10);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someIntFactory, 0)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 1, 3);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someIntFactory, 1)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someIntFactory, 1)
                        .AddSortDirectionalParameter(someIntFactory, true)
                        .AddSortDirectionalParameter(someStringFactory, true)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someIntFactory, 1)
                        .AddSortDirectionalParameter(someIntFactory, true)
                        .AddSortPrimaryKey(true)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someIntFactory, 1)
                        .AddSortPrimaryKey(false)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someIntFactory, 1)
                        .AddSortPrimaryKey(true)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someIntFactory, 1)
                        .AddSortDirectionalParameter(someStringFactory, false)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 1, 3);

                    search = engine.CreateSearch()
                        .AddSearchEnumerableParameter(someStringFactory, new[] { "2", "4" })
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 2);

                    search = engine.CreateSearch()
                        .AddSearchRangeParameter(someStringFactory, "2", "3")
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 2);

                    search = engine.CreateSearch()
                        .AddSearchEnumerableParameter(someStringFactory, new[] { "0", "5", "10" })
                        .AddSortDirectionalParameter(someIntFactory, false)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someTagsFactory, "2")
                        .AddSortDirectionalParameter(someIntFactory, false)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someStringFactory, "2")
                        .AddSortDirectionalParameter(someTagsFactory, true)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someStringFactory, "2")
                        .AddSortDirectionalParameter(someTagsFactory, false)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    search = engine.CreateSearch()
                        .AddSearchExactParameter(someTagsFactory, "2")
                        .AddSearchExactParameter(someTagsFactory, "3")
                        .AddSortDirectionalParameter(someIntFactory, false)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someTagsFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    foreach (EngineItem item in items)
                        engine.Update(item);

                    engine.Rebuild();
                }
            }
        }
    }
}
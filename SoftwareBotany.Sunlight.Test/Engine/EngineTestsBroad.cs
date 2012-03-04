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
                    Search<int> search = engine.CreateSearch();
                    search.AddSearchExactParameter(someIntFactory, 0);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 10);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someIntFactory, 0);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 1, 3);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someIntFactory, 1);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someIntFactory, 1);
                    search.AddSortDirectionalParameter(someIntFactory, true);
                    search.AddSortDirectionalParameter(someStringFactory, true);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someIntFactory, 1);
                    search.AddSortDirectionalParameter(someIntFactory, true);
                    search.SortPrimaryKeyAscending = true;
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someIntFactory, 1);
                    search.SortPrimaryKeyAscending = false;
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someIntFactory, 1);
                    search.SortPrimaryKeyAscending = true;
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 4);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someIntFactory, 1);
                    search.AddSortDirectionalParameter(someStringFactory, false);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someStringFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 1, 3);

                    search = engine.CreateSearch();
                    search.AddSearchEnumerableParameter(someStringFactory, new[] { "2", "4" });
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 2);

                    search = engine.CreateSearch();
                    search.AddSearchRangeParameter(someStringFactory, "2", "3");
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 2);

                    search = engine.CreateSearch();
                    search.AddSearchEnumerableParameter(someStringFactory, new[] { "0", "5", "10" });
                    search.AddSortDirectionalParameter(someIntFactory, false);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someTagsFactory, "2");
                    search.AddSortDirectionalParameter(someIntFactory, false);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someStringFactory, "2");
                    search.AddSortDirectionalParameter(someTagsFactory, true);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someStringFactory, "2");
                    search.AddSortDirectionalParameter(someTagsFactory, false);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someIntFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    search = engine.CreateSearch();
                    search.AddSearchExactParameter(someTagsFactory, "2");
                    search.AddSearchExactParameter(someTagsFactory, "3");
                    search.AddSortDirectionalParameter(someIntFactory, false);
                    search.AddFacetParameter(someDateTimeFactory);
                    search.AddFacetParameter(someIntFactory);
                    search.AddFacetParameter(someTagsFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, 5);

                    foreach (EngineItem item in items)
                        engine.Update(item);

                    engine.Optimize();
                }
            }
        }
    }
}
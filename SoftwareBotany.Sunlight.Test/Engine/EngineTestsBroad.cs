using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class EngineTestsBroad
    {
        [TestMethod]
        public void Small() { SafetyVectorCompressionTuple.RunAll(svct => Base(svct, 50)); }

        [TestMethod]
        public void Medium() { SafetyVectorCompressionTuple.RunAll(svct => Base(svct, 500)); }

        [TestMethod]
        public void Large() { SafetyVectorCompressionTuple.RunAll(svct => Base(svct, 5000)); }

        private void Base(SafetyVectorCompressionTuple safetyVectorCompression, int size)
        {
            using (var engine = new Engine<EngineItem, int>(safetyVectorCompression.AllowUnsafe, item => item.Id))
            {
                var someIntCatalog = engine.CreateCatalog("SomeInt", safetyVectorCompression.Compression, item => item.SomeInt);
                var someDateTimeCatalog = engine.CreateCatalog("SomeDateTime", safetyVectorCompression.Compression, item => item.SomeDateTime);
                var someStringCatalog = engine.CreateCatalog("SomeString", safetyVectorCompression.Compression, item => item.SomeString);
                var someTagsCatalog = engine.CreateCatalog<string>("SomeTags", safetyVectorCompression.Compression, item => item.SomeTags);

                EngineItem[] items = EngineItem.CreateItems(id => id / 5, id => new DateTime(2011, 1, 1).AddDays(id / 7), id => id.ToString(), id => new[] { id, id / 2, id / 3, id / 5 }.Distinct().Select(i => i.ToString()).ToArray(), size);

                foreach (EngineItem item in items)
                    engine.Add(item);

                for (int i = 0; i < 10; i++)
                {
                    Query<int> query = engine.CreateQuery();
                    query.AddFilterExactParameter(someIntCatalog, 0);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someStringCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 10);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someIntCatalog, 0);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someStringCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 1, 3);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someIntCatalog, 1);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someStringCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 4);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someIntCatalog, 1);
                    query.AddSortParameter(someIntCatalog, true);
                    query.AddSortParameter(someStringCatalog, true);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someStringCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 4);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someIntCatalog, 1);
                    query.AddSortParameter(someIntCatalog, true);
                    query.SortPrimaryKeyAscending = true;
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someStringCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 4);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someIntCatalog, 1);
                    query.SortPrimaryKeyAscending = false;
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someStringCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 4);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someIntCatalog, 1);
                    query.SortPrimaryKeyAscending = true;
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someStringCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 4);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someIntCatalog, 1);
                    query.AddSortParameter(someStringCatalog, false);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someStringCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 1, 3);

                    query = engine.CreateQuery();
                    query.AddFilterEnumerableParameter(someStringCatalog, new[] { "2", "4" });
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someIntCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 2);

                    query = engine.CreateQuery();
                    query.AddFilterRangeParameter(someStringCatalog, "2", "3");
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someIntCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 2);

                    query = engine.CreateQuery();
                    query.AddFilterEnumerableParameter(someStringCatalog, new[] { "0", "5", "10" });
                    query.AddSortParameter(someIntCatalog, false);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someIntCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 5);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someTagsCatalog, "2");
                    query.AddSortParameter(someIntCatalog, false);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someIntCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 5);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someStringCatalog, "2");
                    query.AddSortParameter(someTagsCatalog, true);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someIntCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 5);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someStringCatalog, "2");
                    query.AddSortParameter(someTagsCatalog, false);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someIntCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 5);

                    query = engine.CreateQuery();
                    query.AddFilterExactParameter(someTagsCatalog, "2");
                    query.AddFilterExactParameter(someTagsCatalog, "3");
                    query.AddSortParameter(someIntCatalog, false);
                    query.AddFacetParameter(someDateTimeCatalog);
                    query.AddFacetParameter(someIntCatalog);
                    query.AddFacetParameter(someTagsCatalog);

                    EngineAssert.ExecuteAndAssert(items, query, 0, 5);

                    items = Update(engine, items, i);
                    items = Remove(engine, items);
                    items = RemoveReAdd(engine, items);

                    if (i % 2 == 0)
                        engine.Optimize();
                }
            }
        }

        private static EngineItem[] Update(Engine<EngineItem, int> engine, EngineItem[] items, int i)
        {
            EngineItem[] updateItems = items.Where(item => item.Id % 10 == i)
                .ToArray();

            engine.Update(updateItems.Take(updateItems.Length / 2));

            foreach (EngineItem item in updateItems.Skip(updateItems.Length / 2))
                engine.Update(item);

            return items.Except(updateItems).Concat(updateItems).ToArray();
        }

        private static EngineItem[] Remove(Engine<EngineItem, int> engine, EngineItem[] items)
        {
            EngineItem[] removeItems = items.Take(items.Length / 10)
                .ToArray();

            engine.Remove(removeItems.Take(removeItems.Length / 2));

            foreach (EngineItem item in removeItems.Skip(removeItems.Length / 2))
                engine.Remove(item);

            return items.Except(removeItems).ToArray();
        }

        private static EngineItem[] RemoveReAdd(Engine<EngineItem, int> engine, EngineItem[] items)
        {
            EngineItem[] removeReAddItems = items.Take(items.Length / 10)
                .ToArray();

            engine.Remove(removeReAddItems.Take(removeReAddItems.Length / 2));
            engine.Add(removeReAddItems.Take(removeReAddItems.Length / 2));

            foreach (EngineItem item in removeReAddItems.Skip(removeReAddItems.Length / 2))
            {
                engine.Remove(item);
                engine.Add(item);
            }

            return items.Except(removeReAddItems).Concat(removeReAddItems).ToArray();
        }
    }
}
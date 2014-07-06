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
                var someIntFactory = engine.CreateCatalog("SomeInt", safetyVectorCompression.Compression, item => item.SomeInt);
                var someDateTimeFactory = engine.CreateCatalog("SomeDateTime", safetyVectorCompression.Compression, item => item.SomeDateTime);
                var someStringFactory = engine.CreateCatalog("SomeString", safetyVectorCompression.Compression, item => item.SomeString);
                var someTagsFactory = engine.CreateCatalog<string>("SomeTags", safetyVectorCompression.Compression, item => item.SomeTags);

                EngineItem[] items = EngineItem.CreateItems(id => id / 5, id => new DateTime(2011, 1, 1).AddDays(id / 7), id => id.ToString(), id => new[] { id, id / 2, id / 3, id / 5 }.Distinct().Select(i => i.ToString()).ToArray(), size);

                foreach (EngineItem item in items)
                    engine.Add(item);

                for (int i = 0; i < 10; i++)
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
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class EngineTestsRandom
    {
        private const int XXSMALLSEED = 885632;
        private const int XXSMALLSIZE = 10;
        private const int XXSMALLRUNS = 500;

        private const int XSMALLSEED = -711236;
        private const int XSMALLSIZE = 25;
        private const int XSMALLRUNS = 200;

        private const int SMALLSEED = 1245596;
        private const int SMALLSIZE = 50;
        private const int SMALLRUNS = 100;

        private const int MEDIUMSEED = -885321;
        private const int MEDIUMSIZE = 100;
        private const int MEDIUMRUNS = 50;

        private const int LARGESEED = 368110;
        private const int LARGESIZE = 1000;
        private const int LARGERUNS = 25;

        private const int XLARGESEED = -345621;
        private const int XLARGESIZE = 2000;
        private const int XLARGERUNS = 10;

        private const int XXLARGESEED = 2239982;
        private const int XXLARGESIZE = 5000;
        private const int XXLARGERUNS = 2;

        [TestMethod]
        public void XXSmallSimple() { Base(XXSMALLSEED, XXSMALLSIZE, true, XXSMALLRUNS); }
        [TestMethod]
        public void XXSmallComplex() { Base(XXSMALLSEED, XXSMALLSIZE, false, XXSMALLRUNS); }

        [TestMethod]
        public void XSmallSimple() { Base(XSMALLSEED, XSMALLSIZE, true, XSMALLRUNS); }
        [TestMethod]
        public void XSmallComplex() { Base(XSMALLSEED, XSMALLSIZE, false, XSMALLRUNS); }

        [TestMethod]
        public void SmallSimple() { Base(SMALLSEED, SMALLSIZE, true, SMALLRUNS); }
        [TestMethod]
        public void SmallComplex() { Base(SMALLSEED, SMALLSIZE, false, SMALLRUNS); }

        [TestMethod]
        public void MediumSimple() { Base(MEDIUMSEED, MEDIUMSIZE, true, MEDIUMRUNS); }
        [TestMethod]
        public void MediumComplex() { Base(MEDIUMSEED, MEDIUMSIZE, false, MEDIUMRUNS); }

        [TestMethod]
        public void LargeSimple() { Base(LARGESEED, LARGESIZE, true, LARGERUNS); }
        [TestMethod]
        public void LargeComplex() { Base(LARGESEED, LARGESIZE, false, LARGERUNS); }

        [TestMethod]
        public void XLargeSimple() { Base(XLARGESEED, XLARGESIZE, true, XLARGERUNS); }
        [TestMethod]
        public void XLargeComplex() { Base(XLARGESEED, XLARGESIZE, false, XLARGERUNS); }

        [TestMethod]
        public void XXLargeSimple() { Base(XXLARGESEED, XXLARGESIZE, true, XXLARGERUNS); }
        [TestMethod]
        public void XXLargeComplex() { Base(XXLARGESEED, XXLARGESIZE, false, XXLARGERUNS); }

        private static void Base(int randomSeed, int size, bool simple, int runs)
        {
            Random random = new Random(randomSeed);

            foreach(int run in Enumerable.Range(0, runs))
                BaseImpl(random.Next(), size, simple);
        }

        private static void BaseImpl(int randomSeed, int size, bool simple)
        {
            Random random = new Random(randomSeed);

            size = Convert.ToInt32(Math.Round((random.NextDouble() + .5) * size));

            using (var engine = new Engine<EngineItem, int>(item => item.Id))
            {
                var someIntFactory = engine.CreateCatalog("SomeInt", item => item.SomeInt);
                var someDateTimeFactory = engine.CreateCatalog("SomeDateTime", item => item.SomeDateTime);
                var someStringFactory = engine.CreateCatalog("SomeString", item => item.SomeString);
                var someTagsFactory = engine.CreateCatalog<string>("SomeTags", item => item.SomeTags);

                double fillFactor = Math.Max(random.NextDouble() * 10, 1);
                int someIntMax, someDateTimeMax, someStringMax, someTagsMax, someTagsMaxCount;
                EngineItem[] items = EngineItem.CreateItems(random, size, out someIntMax, out someDateTimeMax, out someStringMax, out someTagsMax, out someTagsMaxCount);

                int enumerableAddCount = random.Next(items.Length);

                engine.Add(items.Take(enumerableAddCount));

                foreach (EngineItem item in items.Skip(enumerableAddCount))
                    engine.Add(item);

                for (int i = 0; i < 3; i++)
                {
                    Search<int> search = null;

                    if (simple)
                    {
                        search = engine.CreateSearch()
                            .AddRandomSearchExactParameter(someIntFactory, random.Next(), someIntMax)
                            .AddSortDirectionalParameter(someStringFactory, random.Next() % 2 == 0)
                            .AddFacetParameter(someIntFactory)
                            .AddFacetParameter(someDateTimeFactory)
                            .AddFacetParameter(someStringFactory)
                            .AddFacetParameter(someTagsFactory);

                        EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                        search = engine.CreateSearch()
                            .AddRandomSearchEnumerableParameter(someIntFactory, random.Next(), someIntMax)
                            .AddSortDirectionalParameter(someIntFactory, random.Next() % 2 == 0)
                            .AddFacetParameter(someIntFactory)
                            .AddFacetParameter(someDateTimeFactory)
                            .AddFacetParameter(someStringFactory)
                            .AddFacetParameter(someTagsFactory);

                        EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                        search = engine.CreateSearch()
                            .AddRandomSearchRangeParameter(someIntFactory, random.Next(), someIntMax)
                            .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                            .AddFacetParameter(someIntFactory)
                            .AddFacetParameter(someDateTimeFactory)
                            .AddFacetParameter(someStringFactory)
                            .AddFacetParameter(someTagsFactory);

                        EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                        search = engine.CreateSearch()
                            .AddRandomSearchRangeParameter(someTagsFactory, random.Next(), someTagsMax)
                            .AddSortDirectionalParameter(someTagsFactory, random.Next() % 2 == 0)
                            .AddFacetParameter(someIntFactory)
                            .AddFacetParameter(someDateTimeFactory)
                            .AddFacetParameter(someStringFactory);

                        EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));
                    }
                    else
                    {
                        search = engine.CreateSearch()
                            .AddRandomSearchExactParameter(someStringFactory, random.Next(), someStringMax)
                            .AddRandomSearchRangeParameter(someIntFactory, random.Next(), someIntMax)
                            .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                            .AddSortDirectionalParameter(someIntFactory, random.Next() % 2 == 0)
                            .AddSortPrimaryKey(random.Next() % 2 == 0)
                            .AddFacetParameter(someIntFactory)
                            .AddFacetParameter(someDateTimeFactory)
                            .AddFacetParameter(someStringFactory)
                            .AddFacetParameter(someTagsFactory);

                        EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                        search = engine.CreateSearch()
                            .AddRandomSearchEnumerableParameter(someStringFactory, random.Next(), someStringMax)
                            .AddRandomSearchRangeParameter(someDateTimeFactory, random.Next(), someDateTimeMax)
                            .AddSortDirectionalParameter(someIntFactory, random.Next() % 2 == 0)
                            .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                            .AddSortPrimaryKey(random.Next() % 2 == 0)
                            .AddFacetParameter(someIntFactory)
                            .AddFacetParameter(someDateTimeFactory)
                            .AddFacetParameter(someStringFactory)
                            .AddFacetParameter(someTagsFactory);

                        EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                        search = engine.CreateSearch()
                            .AddAmongstPrimaryKeys(items.Take((items.Length / 2) + random.Next(items.Length / 2)).Select(item => item.Id))
                            .AddRandomSearchRangeParameter(someDateTimeFactory, random.Next(), someDateTimeMax)
                            .AddRandomSearchRangeParameter(someIntFactory, random.Next(), someIntMax)
                            .AddSortDirectionalParameter(someStringFactory, random.Next() % 2 == 0)
                            .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                            .AddSortPrimaryKey(random.Next() % 2 == 0)
                            .AddFacetParameter(someIntFactory)
                            .AddFacetParameter(someDateTimeFactory)
                            .AddFacetParameter(someStringFactory)
                            .AddFacetParameter(someTagsFactory);

                        EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                        search = engine.CreateSearch()
                            .AddRandomSearchRangeParameter(someIntFactory, random.Next(), someIntMax)
                            .AddRandomSearchRangeParameter(someTagsFactory, random.Next(), someTagsMax)
                            .AddRandomSearchRangeParameter(someTagsFactory, random.Next(), someTagsMax)
                            .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                            .AddSortDirectionalParameter(someTagsFactory, random.Next() % 2 == 0)
                            .AddSortPrimaryKey(random.Next() % 2 == 0)
                            .AddFacetParameter(someIntFactory)
                            .AddFacetParameter(someDateTimeFactory)
                            .AddFacetParameter(someStringFactory)
                            .AddFacetParameter(someTagsFactory);

                        EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));
                    }

                    var itemsOrdered = (i % 2 == 0) ? items.OrderBy(item => item.GetHashCode()) : items.OrderByDescending(item => item.GetHashCode());
                    EngineItem[] updateItems = itemsOrdered.Take(random.Next(items.Length)).ToArray();

                    int enumerableUpdateCount = random.Next(updateItems.Length);

                    engine.Update(updateItems.Take(enumerableUpdateCount));

                    foreach (EngineItem item in updateItems.Skip(enumerableUpdateCount))
                        engine.Update(item);

                    HashSet<EngineItem> updateSet = new HashSet<EngineItem>(updateItems);
                    items = items.Where(item => !updateSet.Contains(item)).Concat(updateItems).ToArray();

                    EngineItem[] removeItems = itemsOrdered.Take(random.Next(items.Length / 10)).ToArray();

                    foreach (EngineItem item in removeItems)
                        engine.Remove(item);

                    HashSet<EngineItem> removeSet = new HashSet<EngineItem>(removeItems);
                    items = items.Where(item => !removeSet.Contains(item)).ToArray();

                    if (i % 2 == 0)
                        engine.Rebuild();
                }
            }
        }
    }
}
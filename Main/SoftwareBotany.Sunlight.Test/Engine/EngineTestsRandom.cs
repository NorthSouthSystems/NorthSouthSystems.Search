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
        public void XXSmallCompressedSimple() { Base(XXSMALLSEED, XXSMALLSIZE, true, true, XXSMALLRUNS); }
        [TestMethod]
        public void XXSmallNotCompressedSimple() { Base(XXSMALLSEED, XXSMALLSIZE, false, true, XXSMALLRUNS); }
        [TestMethod]
        public void XXSmallCompressedComplex() { Base(XXSMALLSEED, XXSMALLSIZE, true, false, XXSMALLRUNS); }
        [TestMethod]
        public void XXSmallNotCompressedComplex() { Base(XXSMALLSEED, XXSMALLSIZE, false, false, XXSMALLRUNS); }

        [TestMethod]
        public void XSmallCompressedSimple() { Base(XSMALLSEED, XSMALLSIZE, true, true, XSMALLRUNS); }
        [TestMethod]
        public void XSmallNotCompressedSimple() { Base(XSMALLSEED, XSMALLSIZE, false, true, XSMALLRUNS); }
        [TestMethod]
        public void XSmallCompressedComplex() { Base(XSMALLSEED, XSMALLSIZE, true, false, XSMALLRUNS); }
        [TestMethod]
        public void XSmallNotCompressedComplex() { Base(XSMALLSEED, XSMALLSIZE, false, false, XSMALLRUNS); }

        [TestMethod]
        public void SmallCompressedSimple() { Base(SMALLSEED, SMALLSIZE, true, true, SMALLRUNS); }
        [TestMethod]
        public void SmallNotCompressedSimple() { Base(SMALLSEED, SMALLSIZE, false, true, SMALLRUNS); }
        [TestMethod]
        public void SmallCompressedComplex() { Base(SMALLSEED, SMALLSIZE, true, false, SMALLRUNS); }
        [TestMethod]
        public void SmallNotCompressedComplex() { Base(SMALLSEED, SMALLSIZE, false, false, SMALLRUNS); }

        [TestMethod]
        public void MediumCompressedSimple() { Base(MEDIUMSEED, MEDIUMSIZE, true, true, MEDIUMRUNS); }
        [TestMethod]
        public void MediumNotCompressedSimple() { Base(MEDIUMSEED, MEDIUMSIZE, false, true, MEDIUMRUNS); }
        [TestMethod]
        public void MediumCompressedComplex() { Base(MEDIUMSEED, MEDIUMSIZE, true, false, MEDIUMRUNS); }
        [TestMethod]
        public void MediumNotCompressedComplex() { Base(MEDIUMSEED, MEDIUMSIZE, false, false, MEDIUMRUNS); }

        [TestMethod]
        public void LargeCompressedSimple() { Base(LARGESEED, LARGESIZE, true, true, LARGERUNS); }
        [TestMethod]
        public void LargeNotCompressedSimple() { Base(LARGESEED, LARGESIZE, false, true, LARGERUNS); }
        [TestMethod]
        public void LargeCompressedComplex() { Base(LARGESEED, LARGESIZE, true, false, LARGERUNS); }
        [TestMethod]
        public void LargeNotCompressedComplex() { Base(LARGESEED, LARGESIZE, false, false, LARGERUNS); }

        [TestMethod]
        public void XLargeCompressedSimple() { Base(XLARGESEED, XLARGESIZE, true, true, XLARGERUNS); }
        [TestMethod]
        public void XLargeNotCompressedSimple() { Base(XLARGESEED, XLARGESIZE, false, true, XLARGERUNS); }
        [TestMethod]
        public void XLargeCompressedComplex() { Base(XLARGESEED, XLARGESIZE, true, false, XLARGERUNS); }
        [TestMethod]
        public void XLargeNotCompressedComplex() { Base(XLARGESEED, XLARGESIZE, false, false, XLARGERUNS); }

        [TestMethod]
        public void XXLargeCompressedSimple() { Base(XXLARGESEED, XXLARGESIZE, true, true, XXLARGERUNS); }
        [TestMethod]
        public void XXLargeNotCompressedSimple() { Base(XXLARGESEED, XXLARGESIZE, false, true, XXLARGERUNS); }
        [TestMethod]
        public void XXLargeCompressedComplex() { Base(XXLARGESEED, XXLARGESIZE, true, false, XXLARGERUNS); }
        [TestMethod]
        public void XXLargeNotCompressedComplex() { Base(XXLARGESEED, XXLARGESIZE, false, false, XXLARGERUNS); }

        private static void Base(int randomSeed, int size, bool isCompressed, bool simple, int runs)
        {
            Random random = new Random(randomSeed);

            foreach(int run in Enumerable.Range(0, runs))
                BaseImpl(random.Next(), size, isCompressed, simple);
        }

        private static void BaseImpl(int randomSeed, int size, bool isCompressed, bool simple)
        {
            Random random = new Random(randomSeed);

            size = Convert.ToInt32(Math.Round((random.NextDouble() + .5) * size));

            var engine = new Engine<EngineItem, int>(item => item.Id);
            var someIntFactory = engine.CreateCatalog("SomeInt", item => item.SomeInt, isCompressed);
            var someDateTimeFactory = engine.CreateCatalog("SomeDateTime", item => item.SomeDateTime, isCompressed);
            var someStringFactory = engine.CreateCatalog("SomeString", item => item.SomeString, isCompressed);
            var someTagsFactory = engine.CreateCatalog<string>("SomeTags", item => item.SomeTags, isCompressed);

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
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory)
                        .AddProjectionParameter(someTagsFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                    search = engine.CreateSearch()
                        .AddRandomSearchEnumerableParameter(someIntFactory, random.Next(), someIntMax)
                        .AddSortDirectionalParameter(someIntFactory, random.Next() % 2 == 0)
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory)
                        .AddProjectionParameter(someTagsFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                    search = engine.CreateSearch()
                        .AddRandomSearchRangeParameter(someIntFactory, random.Next(), someIntMax)
                        .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory)
                        .AddProjectionParameter(someTagsFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                    search = engine.CreateSearch()
                        .AddRandomSearchRangeParameter(someTagsFactory, random.Next(), someTagsMax)
                        .AddSortDirectionalParameter(someTagsFactory, random.Next() % 2 == 0)
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory);

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
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory)
                        .AddProjectionParameter(someTagsFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                    search = engine.CreateSearch()
                        .AddRandomSearchEnumerableParameter(someStringFactory, random.Next(), someStringMax)
                        .AddRandomSearchRangeParameter(someDateTimeFactory, random.Next(), someDateTimeMax)
                        .AddSortDirectionalParameter(someIntFactory, random.Next() % 2 == 0)
                        .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                        .AddSortPrimaryKey(random.Next() % 2 == 0)
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory)
                        .AddProjectionParameter(someTagsFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                    search = engine.CreateSearch()
                        .AddAmongstPrimaryKeys(items.Take((items.Length / 2) + random.Next(items.Length / 2)).Select(item => item.Id))
                        .AddRandomSearchRangeParameter(someDateTimeFactory, random.Next(), someDateTimeMax)
                        .AddRandomSearchRangeParameter(someIntFactory, random.Next(), someIntMax)
                        .AddSortDirectionalParameter(someStringFactory, random.Next() % 2 == 0)
                        .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                        .AddSortPrimaryKey(random.Next() % 2 == 0)
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory)
                        .AddProjectionParameter(someTagsFactory);

                    EngineAssert.ExecuteAndAssert(items, search, 0, random.Next(size));

                    search = engine.CreateSearch()
                        .AddRandomSearchRangeParameter(someIntFactory, random.Next(), someIntMax)
                        .AddRandomSearchRangeParameter(someTagsFactory, random.Next(), someTagsMax)
                        .AddRandomSearchRangeParameter(someTagsFactory, random.Next(), someTagsMax)
                        .AddSortDirectionalParameter(someDateTimeFactory, random.Next() % 2 == 0)
                        .AddSortDirectionalParameter(someTagsFactory, random.Next() % 2 == 0)
                        .AddSortPrimaryKey(random.Next() % 2 == 0)
                        .AddProjectionParameter(someIntFactory)
                        .AddProjectionParameter(someDateTimeFactory)
                        .AddProjectionParameter(someStringFactory)
                        .AddProjectionParameter(someTagsFactory);

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
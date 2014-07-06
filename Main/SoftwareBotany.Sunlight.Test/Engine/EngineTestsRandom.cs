using System;
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
        [TestProperty("Duration", "Long")]
        public void LargeSimple() { Base(LARGESEED, LARGESIZE, true, LARGERUNS); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void LargeComplexNone() { Base(LARGESEED, LARGESIZE, false, LARGERUNS, VectorCompression.None); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void LargeComplexCompressed() { Base(LARGESEED, LARGESIZE, false, LARGERUNS, VectorCompression.Compressed); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void LargeComplexCompressedWithPackedPosition() { Base(LARGESEED, LARGESIZE, false, LARGERUNS, VectorCompression.CompressedWithPackedPosition); }

        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void XLargeSimple() { Base(XLARGESEED, XLARGESIZE, true, XLARGERUNS); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void XLargeComplexNone() { Base(XLARGESEED, XLARGESIZE, false, XLARGERUNS, VectorCompression.None); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void XLargeComplexCompressed() { Base(XLARGESEED, XLARGESIZE, false, XLARGERUNS, VectorCompression.Compressed); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void XLargeComplexCompressedWithPackedPosition() { Base(XLARGESEED, XLARGESIZE, false, XLARGERUNS, VectorCompression.CompressedWithPackedPosition); }

        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void XXLargeSimple() { Base(XXLARGESEED, XXLARGESIZE, true, XXLARGERUNS); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void XXLargeComplexNone() { Base(XXLARGESEED, XXLARGESIZE, false, XXLARGERUNS, VectorCompression.None); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void XXLargeComplexCompressed() { Base(XXLARGESEED, XXLARGESIZE, false, XXLARGERUNS, VectorCompression.Compressed); }
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void XXLargeComplexCompressedWithPackedPosition() { Base(XXLARGESEED, XXLARGESIZE, false, XXLARGERUNS, VectorCompression.CompressedWithPackedPosition); }

        private static void Base(int randomSeed, int size, bool simple, int runs, VectorCompression? compression = null)
        {
            Random random = new Random(randomSeed);

            foreach (int run in Enumerable.Range(0, runs))
            {
                if (compression == null)
                    SafetyVectorCompressionTuple.RunAll(svct => BaseImpl(svct, random.Next(), size, simple));
                else
                    SafetyVectorCompressionTuple.RunAllSafeties(svct => BaseImpl(svct, random.Next(), size, simple), compression.Value);
            }
        }

        private static void BaseImpl(SafetyVectorCompressionTuple safetyVectorCompression, int randomSeed, int size, bool simple)
        {
            Random random = new Random(randomSeed);

            size = Convert.ToInt32(Math.Round((random.NextDouble() + .5) * size));

            using (var engine = new Engine<EngineItem, int>(safetyVectorCompression.AllowUnsafe, item => item.Id))
            {
                var someIntFactory = engine.CreateCatalog("SomeInt", safetyVectorCompression.Compression, item => item.SomeInt);
                var someDateTimeFactory = engine.CreateCatalog("SomeDateTime", safetyVectorCompression.Compression, item => item.SomeDateTime);
                var someStringFactory = engine.CreateCatalog("SomeString", safetyVectorCompression.Compression, item => item.SomeString);
                var someTagsFactory = engine.CreateCatalog<string>("SomeTags", safetyVectorCompression.Compression, item => item.SomeTags);

                double fillFactor = Math.Max(random.NextDouble() * 10, 1);
                int someIntMax, someDateTimeMax, someStringMax, someTagsMax, someTagsMaxCount;
                EngineItem[] items = EngineItem.CreateItems(random, size, out someIntMax, out someDateTimeMax, out someStringMax, out someTagsMax, out someTagsMaxCount);

                int enumerableAddCount = random.Next(items.Length);

                engine.Add(items.Take(enumerableAddCount));

                foreach (EngineItem item in items.Skip(enumerableAddCount))
                    engine.Add(item);

                for (int i = 0; i < 3; i++)
                {
                    Query<int> query = null;

                    if (simple)
                    {
                        query = engine.CreateQuery();
                        query.AddRandomFilterExactParameter(someIntFactory, random.Next(), someIntMax);
                        query.AddSortParameter(someStringFactory, random.Next() % 2 == 0);
                        query.AddFacetParameter(someIntFactory);
                        query.AddFacetParameter(someDateTimeFactory);
                        query.AddFacetParameter(someStringFactory);
                        query.AddFacetParameter(someTagsFactory);
                        query.FacetDisableParallel = random.Next() % 5 == 0;
                        query.FacetShortCircuitCounting = random.Next() % 5 == 0;

                        EngineAssert.ExecuteAndAssert(items, query, 0, random.Next(size));

                        query = engine.CreateQuery();
                        query.AddRandomFilterEnumerableParameter(someIntFactory, random.Next(), someIntMax);
                        query.AddSortParameter(someIntFactory, random.Next() % 2 == 0);
                        query.AddFacetParameter(someIntFactory);
                        query.AddFacetParameter(someDateTimeFactory);
                        query.AddFacetParameter(someStringFactory);
                        query.AddFacetParameter(someTagsFactory);
                        query.FacetDisableParallel = random.Next() % 5 == 0;
                        query.FacetShortCircuitCounting = random.Next() % 5 == 0;

                        EngineAssert.ExecuteAndAssert(items, query, 0, random.Next(size));

                        query = engine.CreateQuery();
                        query.AddRandomFilterRangeParameter(someIntFactory, random.Next(), someIntMax);
                        query.AddSortParameter(someDateTimeFactory, random.Next() % 2 == 0);
                        query.AddFacetParameter(someIntFactory);
                        query.AddFacetParameter(someDateTimeFactory);
                        query.AddFacetParameter(someStringFactory);
                        query.AddFacetParameter(someTagsFactory);
                        query.FacetDisableParallel = random.Next() % 5 == 0;
                        query.FacetShortCircuitCounting = random.Next() % 5 == 0;

                        EngineAssert.ExecuteAndAssert(items, query, 0, random.Next(size));

                        query = engine.CreateQuery();
                        query.AddRandomFilterRangeParameter(someTagsFactory, random.Next(), someTagsMax);
                        query.AddSortParameter(someTagsFactory, random.Next() % 2 == 0);
                        query.AddFacetParameter(someIntFactory);
                        query.AddFacetParameter(someDateTimeFactory);
                        query.AddFacetParameter(someStringFactory);
                        query.AddFacetParameter(someTagsFactory);
                        query.FacetDisableParallel = random.Next() % 5 == 0;
                        query.FacetShortCircuitCounting = random.Next() % 5 == 0;

                        EngineAssert.ExecuteAndAssert(items, query, 0, random.Next(size));
                    }
                    else
                    {
                        query = engine.CreateQuery();
                        query.AddRandomFilterExactParameter(someStringFactory, random.Next(), someStringMax);
                        query.AddRandomFilterRangeParameter(someIntFactory, random.Next(), someIntMax);
                        query.AddSortParameter(someDateTimeFactory, random.Next() % 2 == 0);
                        query.AddSortParameter(someIntFactory, random.Next() % 2 == 0);
                        query.SortPrimaryKeyAscending = (random.Next() % 2 == 0);
                        query.AddFacetParameter(someIntFactory);
                        query.AddFacetParameter(someDateTimeFactory);
                        query.AddFacetParameter(someStringFactory);
                        query.AddFacetParameter(someTagsFactory);
                        query.FacetDisableParallel = random.Next() % 5 == 0;
                        query.FacetShortCircuitCounting = random.Next() % 5 == 0;

                        EngineAssert.ExecuteAndAssert(items, query, 0, random.Next(size));

                        query = engine.CreateQuery();
                        query.AddRandomFilterEnumerableParameter(someStringFactory, random.Next(), someStringMax);
                        query.AddRandomFilterRangeParameter(someDateTimeFactory, random.Next(), someDateTimeMax);
                        query.AddSortParameter(someIntFactory, random.Next() % 2 == 0);
                        query.AddSortParameter(someDateTimeFactory, random.Next() % 2 == 0);
                        query.SortPrimaryKeyAscending = (random.Next() % 2 == 0);
                        query.AddFacetParameter(someIntFactory);
                        query.AddFacetParameter(someDateTimeFactory);
                        query.AddFacetParameter(someStringFactory);
                        query.AddFacetParameter(someTagsFactory);
                        query.FacetDisableParallel = random.Next() % 5 == 0;
                        query.FacetShortCircuitCounting = random.Next() % 5 == 0;

                        EngineAssert.ExecuteAndAssert(items, query, 0, random.Next(size));

                        query = engine.CreateQuery();
                        query.AddAmongstPrimaryKeys(items.Take((items.Length / 2) + random.Next(items.Length / 2)).Select(item => item.Id));
                        query.AddRandomFilterRangeParameter(someDateTimeFactory, random.Next(), someDateTimeMax);
                        query.AddRandomFilterRangeParameter(someIntFactory, random.Next(), someIntMax);
                        query.AddSortParameter(someStringFactory, random.Next() % 2 == 0);
                        query.AddSortParameter(someDateTimeFactory, random.Next() % 2 == 0);
                        query.SortPrimaryKeyAscending = (random.Next() % 2 == 0);
                        query.AddFacetParameter(someIntFactory);
                        query.AddFacetParameter(someDateTimeFactory);
                        query.AddFacetParameter(someStringFactory);
                        query.AddFacetParameter(someTagsFactory);
                        query.FacetDisableParallel = random.Next() % 5 == 0;
                        query.FacetShortCircuitCounting = random.Next() % 5 == 0;

                        EngineAssert.ExecuteAndAssert(items, query, 0, random.Next(size));

                        query = engine.CreateQuery();
                        query.AddRandomFilterRangeParameter(someIntFactory, random.Next(), someIntMax);
                        query.AddRandomFilterRangeParameter(someTagsFactory, random.Next(), someTagsMax);
                        query.AddRandomFilterRangeParameter(someTagsFactory, random.Next(), someTagsMax);
                        query.AddSortParameter(someDateTimeFactory, random.Next() % 2 == 0);
                        query.AddSortParameter(someTagsFactory, random.Next() % 2 == 0);
                        query.SortPrimaryKeyAscending = (random.Next() % 2 == 0);
                        query.AddFacetParameter(someIntFactory);
                        query.AddFacetParameter(someDateTimeFactory);
                        query.AddFacetParameter(someStringFactory);
                        query.AddFacetParameter(someTagsFactory);
                        query.FacetDisableParallel = random.Next() % 5 == 0;
                        query.FacetShortCircuitCounting = random.Next() % 5 == 0;

                        EngineAssert.ExecuteAndAssert(items, query, 0, random.Next(size));
                    }

                    items = Update(engine, items, random);
                    items = Remove(engine, items, random);
                    items = RemoveReAdd(engine, items, random);

                    if (i % 2 == 0)
                        engine.Optimize();
                }
            }
        }

        private static EngineItem[] Update(Engine<EngineItem, int> engine, EngineItem[] items, Random random)
        {
            EngineItem[] updateItems = items.OrderBy(item => item.GetHashCode())
                .Take(random.Next(items.Length))
                .ToArray();

            int rangeUpdateCount = random.Next(updateItems.Length);

            engine.Update(updateItems.Take(rangeUpdateCount));

            foreach (EngineItem item in updateItems.Skip(rangeUpdateCount))
                engine.Update(item);

            return items.Except(updateItems).Concat(updateItems).ToArray();
        }

        private static EngineItem[] Remove(Engine<EngineItem, int> engine, EngineItem[] items, Random random)
        {
            var removeItemsAsc = items.OrderBy(item => item.GetHashCode())
                .Take(random.Next(items.Length / 20));

            var removeItemsDesc = items.OrderByDescending(item => item.GetHashCode())
                .Take(random.Next(items.Length / 20));

            EngineItem[] removeItems = removeItemsAsc.Concat(removeItemsDesc)
                .ToArray();

            int rangeRemoveCount = random.Next(removeItems.Length);

            engine.Remove(removeItems.Take(rangeRemoveCount));

            foreach (EngineItem item in removeItems.Skip(rangeRemoveCount))
                engine.Remove(item);

            return items.Except(removeItems).ToArray();
        }

        private static EngineItem[] RemoveReAdd(Engine<EngineItem, int> engine, EngineItem[] items, Random random)
        {
            var removeReAddItemsAsc = items.OrderBy(item => item.GetHashCode())
                .Take(random.Next(items.Length / 20));

            var removeReAddItemsDesc = items.OrderByDescending(item => item.GetHashCode())
                .Take(random.Next(items.Length / 20));

            EngineItem[] removeReAddItems = removeReAddItemsAsc.Concat(removeReAddItemsDesc)
                .ToArray();

            int rangeRemoveReAddCount = random.Next(removeReAddItems.Length);

            engine.Remove(removeReAddItems.Take(rangeRemoveReAddCount));
            engine.Add(removeReAddItems.Take(rangeRemoveReAddCount));

            foreach (EngineItem item in removeReAddItems.Skip(rangeRemoveReAddCount))
            {
                engine.Remove(item);
                engine.Add(item);
            }

            return items.Except(removeReAddItems).Concat(removeReAddItems).ToArray();
        }
    }
}
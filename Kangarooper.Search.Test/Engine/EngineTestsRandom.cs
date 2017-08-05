namespace Kangarooper.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EngineTestsRandom
    {
        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void EngineTestsRandomSafe() { Base(false); }

        [TestMethod]
        [TestProperty("Duration", "Long")]
        public void EngineTestsRandomUnsafe() { Base(true); }

        private static readonly int[] _randomSeeds = new[] { 18873, -76, 5992, 917773, -6320001 };

        private static void Base(bool allowUnsafe)
        {
            foreach (int randomSeed in _randomSeeds)
            {
                var random = new Random(randomSeed);

                foreach (int size in Enumerable.Range(1, (Word.SIZE - 1) * 10 + 1))
                {
                    using (var engine = new Engine<EngineItem, int>(allowUnsafe, item => item.Id))
                    {
                        var someIntCatalog = engine.CreateCatalog("SomeInt", SafetyAndCompression.RandomCompression(random), item => item.SomeInt);
                        var someDateTimeCatalog = engine.CreateCatalog("SomeDateTime", SafetyAndCompression.RandomCompression(random), item => item.SomeDateTime);
                        var someStringCatalog = engine.CreateCatalog("SomeString", SafetyAndCompression.RandomCompression(random), item => item.SomeString);
                        var someTagsCatalog = engine.CreateCatalog<string>("SomeTags", SafetyAndCompression.RandomCompression(random), item => item.SomeTags);

                        int someIntMax, someDateTimeMax, someStringMax, someTagsMax, someTagsMaxCount;

                        var items = EngineItem.CreateItems(random, size, out someIntMax, out someDateTimeMax, out someStringMax, out someTagsMax, out someTagsMaxCount);

                        // OrderBy the lowest potential cardinality (max represents the potential cardinality) so that we increase our chances of having
                        // compressed 1's in a Catalog.
                        int minCardinality = new[] { someIntMax, someDateTimeMax, someStringMax, someTagsMax }.Min();

                        if (someIntMax == minCardinality)
                            items = items.OrderBy(item => item.SomeInt).ToArray();
                        else if (someDateTimeMax == minCardinality)
                            items = items.OrderBy(item => item.SomeDateTime).ToArray();
                        else if (someStringMax == minCardinality)
                            items = items.OrderBy(item => item.SomeString).ToArray();
                        else if (someTagsMax == minCardinality)
                            items = items.OrderBy(item => item.SomeTags.Min()).ToArray();

                        int enumerableAddCount = random.Next(items.Length);

                        engine.Add(items.Take(enumerableAddCount));

                        foreach (var item in items.Skip(enumerableAddCount))
                            engine.Add(item);

                        foreach (int iteration in Enumerable.Range(0, 2))
                        {
                            ExecuteAndAssert(random, items, engine.CreateQuery());

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Amongst(items.Take((items.Length / 2) + random.Next(items.Length / 2)).Select(item => item.Id)));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterExactParameter(someIntCatalog, random, someIntMax)));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Sort(SortParameter.Create(someIntCatalog, random.Next() % 2 == 0)));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .SortPrimaryKey(random.Next() % 2 == 0));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterExactParameter(someIntCatalog, random, someIntMax))
                                .Sort(SortParameter.Create(someStringCatalog, random.Next() % 2 == 0)));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterExactParameter(someIntCatalog, random, someIntMax))
                                .SortPrimaryKey(random.Next() % 2 == 0));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterEnumerableParameter(someIntCatalog, random, someIntMax))
                                .Sort(SortParameter.Create(engine, "SomeInt", random.Next() % 2 == 0)));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterRangeParameter(someIntCatalog, random, someIntMax))
                                .Sort(SortParameter.Create(someDateTimeCatalog, random.Next() % 2 == 0)));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterRangeParameter(someTagsCatalog, random, someTagsMax))
                                .Sort(SortParameter.Create(engine, "SomeTags", random.Next() % 2 == 0)));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterExactParameter(someStringCatalog, random, someStringMax)
                                    && engine.CreateRandomFilterRangeParameter(someIntCatalog, random, someIntMax))
                                .Sort(SortParameter.Create(someDateTimeCatalog, random.Next() % 2 == 0),
                                    SortParameter.Create(someIntCatalog, random.Next() % 2 == 0))
                                .SortPrimaryKey(random.Next() % 2 == 0));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterEnumerableParameter(someStringCatalog, random, someStringMax)
                                    && engine.CreateRandomFilterRangeParameter(someDateTimeCatalog, random, someDateTimeMax))
                                .Sort(SortParameter.Create(engine, "SomeInt", random.Next() % 2 == 0),
                                    SortParameter.Create(engine, "SomeDateTime", random.Next() % 2 == 0))
                                .SortPrimaryKey(random.Next() % 2 == 0));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Amongst(items.Take((items.Length / 2) + random.Next(items.Length / 2)).Select(item => item.Id))
                                .Filter(engine.CreateRandomFilterRangeParameter(someDateTimeCatalog, random, someDateTimeMax)
                                    && engine.CreateRandomFilterRangeParameter(someIntCatalog, random, someIntMax))
                                .Sort(SortParameter.Create(someStringCatalog, random.Next() % 2 == 0),
                                    SortParameter.Create(someDateTimeCatalog, random.Next() % 2 == 0))
                                .SortPrimaryKey(random.Next() % 2 == 0));

                            ExecuteAndAssert(random, items, engine.CreateQuery()
                                .Filter(engine.CreateRandomFilterRangeParameter(someIntCatalog, random, someIntMax)
                                    && engine.CreateRandomFilterRangeParameter(someTagsCatalog, random, someTagsMax)
                                    && engine.CreateRandomFilterRangeParameter(someTagsCatalog, random, someTagsMax))
                                .Sort(SortParameter.Create(engine, "SomeDateTime", random.Next() % 2 == 0),
                                    SortParameter.Create(engine, "SomeTags", random.Next() % 2 == 0))
                                .SortPrimaryKey(random.Next() % 2 == 0));

                            if (random.Next() % 2 == 0)
                                items = Update(engine, items, random);

                            if (random.Next() % 2 == 0)
                                items = RemoveReAdd(engine, items, random);

                            if (random.Next() % 2 == 0)
                                engine.Optimize();
                        }
                    }
                }
            }
        }

        private static void ExecuteAndAssert(Random random, EngineItem[] items, Query<EngineItem, int> query)
        {
            query = query
                .FacetAll()
                .WithFacetOptions(random.Next() % 2 == 0, random.Next() % 2 == 0);

            EngineAssert.ExecuteAndAssert(items, query, 0, items.Length);
        }

        private static EngineItem[] Update(Engine<EngineItem, int> engine, EngineItem[] items, Random random)
        {
            var updateItems = items.OrderBy(item => item.GetHashCode())
                .Take(random.Next(items.Length))
                .ToArray();

            int rangeUpdateCount = random.Next(updateItems.Length);

            engine.Update(updateItems.Take(rangeUpdateCount));

            foreach (var item in updateItems.Skip(rangeUpdateCount))
                engine.Update(item);

            return items.Except(updateItems)
                .Concat(updateItems)
                .ToArray();
        }

        private static EngineItem[] RemoveReAdd(Engine<EngineItem, int> engine, EngineItem[] items, Random random)
        {
            var removeItemsAsc = items.OrderBy(item => item.GetHashCode())
                .Take(random.Next(items.Length / 2));

            var removeItemsDesc = items.OrderByDescending(item => item.GetHashCode())
                .Take(random.Next(items.Length / 2));

            var removeItems = removeItemsAsc.Concat(removeItemsDesc)
                .Distinct()
                .ToArray();

            var reAddItems = new HashSet<EngineItem>(removeItems.Where(_ => random.Next() % 2 == 0));

            int rangeRemoveCount = random.Next(removeItems.Length);

            engine.Remove(removeItems.Take(rangeRemoveCount));
            engine.Add(removeItems.Take(rangeRemoveCount).Where(item => reAddItems.Contains(item)));

            foreach (var item in removeItems.Skip(rangeRemoveCount))
            {
                engine.Remove(item);

                if (reAddItems.Contains(item))
                    engine.Add(item);
            }

            return items.Except(removeItems)
                .Concat(removeItems.Where(item => reAddItems.Contains(item)))
                .ToArray();
        }
    }
}
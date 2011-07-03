using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    internal class EngineItem
    {
        public static EngineItem[] CreateItems(
            Func<int, int> someIntGenerator,
            Func<int, DateTime> someDateTimeGenerator,
            Func<int, string> someStringGenerator,
            Func<int, string[]> someTagGenerator,
            int count)
        {
            return Enumerable.Range(0, count)
                .Select(id =>
                {
                    EngineItem item = new EngineItem();
                    item.Id = id;
                    item.SomeInt = someIntGenerator(id);
                    item.SomeDateTime = someDateTimeGenerator(id);
                    item.SomeString = someStringGenerator(id);
                    item.SomeTags = someTagGenerator(id);
                    return item;
                })
                .ToArray();
        }

        public static EngineItem[] CreateItems(Random random, int count, out int someIntMax, out int someDateTimeMax, out int someStringMax, out int someTagsMax, out int someTagsMaxCount)
        {
            int someIntCardinality = someIntMax = GetCardinality(random, count);
            int someDateTimeCardinality = someDateTimeMax = GetCardinality(random, count);
            int someStringCardinality = someStringMax = GetCardinality(random, count);
            int someTagsCardinality = someTagsMax = GetCardinality(random, count);
            someTagsMaxCount = Math.Max(random.Next(someTagsCardinality), 1);

            int closableSomeTagsMaxCount = someTagsMaxCount;

            return CreateItems(
                id => random.Next(someIntCardinality),
                id => new DateTime(2011, 1, 1).AddDays(random.Next(someDateTimeCardinality)),
                id => random.Next(someStringCardinality).ToString(),
                id => Enumerable.Range(0, random.Next(closableSomeTagsMaxCount)).Select(i => random.Next(someTagsCardinality)).Distinct().Select(tag => tag.ToString()).ToArray(),
                count);
        }

        private static int GetCardinality(Random random, int count)
        {
            double cardinality = random.Next(count);
            cardinality = Math.Round(cardinality);
            cardinality = Math.Max(cardinality, 1);
            return Convert.ToInt32(cardinality);
        }

        public int Id { get; private set; }
        public int SomeInt { get; private set; }
        public DateTime SomeDateTime { get; private set; }
        public string SomeString { get; private set; }
        public string[] SomeTags { get; private set; }

        public override string ToString() { return Id.ToString(); }
    }
}
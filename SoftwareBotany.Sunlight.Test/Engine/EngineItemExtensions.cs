using System;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    internal static class EngineItemExtensions
    {
        public static FilterParameter<int> AddRandomFilterExactParameter(this Query<int> query, CatalogHandle<int> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);
            return query.AddFilterExactParameter(catalog, random.Next(max));
        }

        public static FilterParameter<int> AddRandomFilterEnumerableParameter(this Query<int> query, CatalogHandle<int> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .ToArray();

            return query.AddFilterEnumerableParameter(catalog, enumerable);
        }

        public static FilterParameter<int> AddRandomFilterRangeParameter(this Query<int> query, CatalogHandle<int> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int val1 = random.Next(max);
            int val2 = random.Next(max);

            if (val1 == val2)
                val2++;

            return query.AddFilterRangeParameter(catalog, Math.Min(val1, val2), Math.Max(val1, val2));
        }

        public static FilterParameter<DateTime> AddRandomFilterExactParameter(this Query<int> query, CatalogHandle<DateTime> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);
            return query.AddFilterExactParameter(catalog, new DateTime(2011, 1, 1).AddDays(random.Next(max)));
        }

        public static FilterParameter<DateTime> AddRandomFilterEnumerableParameter(this Query<int> query, CatalogHandle<DateTime> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            DateTime[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .Select(i => new DateTime(2011, 1, 1).AddDays(i))
                .ToArray();

            return query.AddFilterEnumerableParameter(catalog, enumerable);
        }

        public static FilterParameter<DateTime> AddRandomFilterRangeParameter(this Query<int> query, CatalogHandle<DateTime> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int val1 = random.Next(max);
            int val2 = random.Next(max);

            if (val1 == val2)
                val2++;

            return query.AddFilterRangeParameter(catalog, new DateTime(2011, 1, 1).AddDays(Math.Min(val1, val2)), new DateTime(2011, 1, 1).AddDays(Math.Max(val1, val2)));
        }

        public static FilterParameter<string> AddRandomFilterExactParameter(this Query<int> query, CatalogHandle<string> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);
            return query.AddFilterExactParameter(catalog, random.Next(max).ToString());
        }

        public static FilterParameter<string> AddRandomFilterEnumerableParameter(this Query<int> query, CatalogHandle<string> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            string[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .Select(i => i.ToString())
                .ToArray();

            return query.AddFilterEnumerableParameter(catalog, enumerable);
        }

        public static FilterParameter<string> AddRandomFilterRangeParameter(this Query<int> query, CatalogHandle<string> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int val1 = random.Next(max);
            int val2 = random.Next(max);

            if (val1 == val2)
                val2++;

            string rangeMin = val1.ToString();
            string rangeMax = val2.ToString();

            if (rangeMin.CompareTo(rangeMax) > 0)
            {
                string s = rangeMin;
                rangeMin = rangeMax;
                rangeMax = s;
            }

            return query.AddFilterRangeParameter(catalog, rangeMin, rangeMax);
        }
    }
}
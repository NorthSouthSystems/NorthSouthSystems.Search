namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Linq;

    internal static class EngineItemExtensions
    {
        public static FilterParameter<int> AddRandomFilterExactParameter(this Query<int> query, ICatalogHandle<int> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int exact = random.Next(max);

            return random.Next() % 2 == 0
                ? query.AddFilterExactParameter(catalog, exact)
                : (FilterParameter<int>)query.AddFilterExactParameter(catalog.Name, exact);
        }

        public static FilterParameter<int> AddRandomFilterEnumerableParameter(this Query<int> query, ICatalogHandle<int> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .ToArray();

            return random.Next() % 2 == 0
                ? query.AddFilterEnumerableParameter(catalog, enumerable)
                : (FilterParameter<int>)query.AddFilterEnumerableParameter(catalog.Name, enumerable);
        }

        public static FilterParameter<int> AddRandomFilterRangeParameter(this Query<int> query, ICatalogHandle<int> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int val1 = random.Next(max);
            int val2 = random.Next(max);

            if (val1 == val2)
                val2++;

            int rangeMin = Math.Min(val1, val2);
            int rangeMax = Math.Max(val1, val2);

            return random.Next() % 2 == 0
                ? query.AddFilterRangeParameter(catalog, rangeMin, rangeMax)
                : (FilterParameter<int>)query.AddFilterRangeParameter(catalog.Name, rangeMin, rangeMax);
        }

        public static FilterParameter<DateTime> AddRandomFilterExactParameter(this Query<int> query, ICatalogHandle<DateTime> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            DateTime exact = new DateTime(2011, 1, 1).AddDays(random.Next(max));

            return random.Next() % 2 == 0
                ? query.AddFilterExactParameter(catalog, exact)
                : (FilterParameter<DateTime>)query.AddFilterExactParameter(catalog.Name, exact);
        }

        public static FilterParameter<DateTime> AddRandomFilterEnumerableParameter(this Query<int> query, ICatalogHandle<DateTime> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            DateTime[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .Select(i => new DateTime(2011, 1, 1).AddDays(i))
                .ToArray();

            return random.Next() % 2 == 0
                ? query.AddFilterEnumerableParameter(catalog, enumerable)
                : (FilterParameter<DateTime>)query.AddFilterEnumerableParameter(catalog.Name, enumerable);
        }

        public static FilterParameter<DateTime> AddRandomFilterRangeParameter(this Query<int> query, ICatalogHandle<DateTime> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int val1 = random.Next(max);
            int val2 = random.Next(max);

            if (val1 == val2)
                val2++;

            DateTime rangeMin = new DateTime(2011, 1, 1).AddDays(Math.Min(val1, val2));
            DateTime rangeMax = new DateTime(2011, 1, 1).AddDays(Math.Max(val1, val2));

            return random.Next() % 2 == 0
                ? query.AddFilterRangeParameter(catalog, rangeMin, rangeMax)
                : (FilterParameter<DateTime>)query.AddFilterRangeParameter(catalog.Name, rangeMin, rangeMax);
        }

        public static FilterParameter<string> AddRandomFilterExactParameter(this Query<int> query, ICatalogHandle<string> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            string exact = random.Next(max).ToString();

            return random.Next() % 2 == 0
                ? query.AddFilterExactParameter(catalog, exact)
                : (FilterParameter<string>)query.AddFilterExactParameter(catalog.Name, exact);
        }

        public static FilterParameter<string> AddRandomFilterEnumerableParameter(this Query<int> query, ICatalogHandle<string> catalog, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            string[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .Select(i => i.ToString())
                .ToArray();

            return random.Next() % 2 == 0
                ? query.AddFilterEnumerableParameter(catalog, enumerable)
                : (FilterParameter<string>)query.AddFilterEnumerableParameter(catalog.Name, enumerable);
        }

        public static FilterParameter<string> AddRandomFilterRangeParameter(this Query<int> query, ICatalogHandle<string> catalog, int randomSeed, int max)
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

            return random.Next() % 2 == 0
                ? query.AddFilterRangeParameter(catalog, rangeMin, rangeMax)
                : (FilterParameter<string>)query.AddFilterRangeParameter(catalog.Name, rangeMin, rangeMax);
        }
    }
}
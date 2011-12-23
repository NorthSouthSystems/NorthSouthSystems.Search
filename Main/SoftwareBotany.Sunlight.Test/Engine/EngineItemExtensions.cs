using System;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    internal static class EngineItemExtensions
    {
        public static SearchParameter<int> AddRandomSearchExactParameter(this Search<int> search, ParameterFactory<int> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);
            return search.AddSearchExactParameter(factory, random.Next(max));
        }

        public static SearchParameter<int> AddRandomSearchEnumerableParameter(this Search<int> search, ParameterFactory<int> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .ToArray();

            return search.AddSearchEnumerableParameter(factory, enumerable);
        }

        public static SearchParameter<int> AddRandomSearchRangeParameter(this Search<int> search, ParameterFactory<int> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);
            
            int val1 = random.Next(max);
            int val2 = random.Next(max);

            if (val1 == val2)
                val2++;

            return search.AddSearchRangeParameter(factory, Math.Min(val1, val2), Math.Max(val1, val2));
        }

        public static SearchParameter<DateTime> AddRandomSearchExactParameter(this Search<int> search, ParameterFactory<DateTime> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);
            return search.AddSearchExactParameter(factory, new DateTime(2011, 1, 1).AddDays(random.Next(max)));
        }

        public static SearchParameter<DateTime> AddRandomSearchEnumerableParameter(this Search<int> search, ParameterFactory<DateTime> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            DateTime[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .Select(i => new DateTime(2011, 1, 1).AddDays(i))
                .ToArray();

            return search.AddSearchEnumerableParameter(factory, enumerable);
        }

        public static SearchParameter<DateTime> AddRandomSearchRangeParameter(this Search<int> search, ParameterFactory<DateTime> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int val1 = random.Next(max);
            int val2 = random.Next(max);

            if (val1 == val2)
                val2++;

            return search.AddSearchRangeParameter(factory, new DateTime(2011, 1, 1).AddDays(Math.Min(val1, val2)), new DateTime(2011, 1, 1).AddDays(Math.Max(val1, val2)));
        }

        public static SearchParameter<string> AddRandomSearchExactParameter(this Search<int> search, ParameterFactory<string> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);
            return search.AddSearchExactParameter(factory, random.Next(max).ToString());
        }

        public static SearchParameter<string> AddRandomSearchEnumerableParameter(this Search<int> search, ParameterFactory<string> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            string[] enumerable = Enumerable.Range(0, random.Next(max))
                .Select(i => random.Next(max))
                .Distinct()
                .Select(i => i.ToString())
                .ToArray();

            return search.AddSearchEnumerableParameter(factory, enumerable);
        }

        public static SearchParameter<string> AddRandomSearchRangeParameter(this Search<int> search, ParameterFactory<string> factory, int randomSeed, int max)
        {
            Random random = new Random(randomSeed);

            int val1 = random.Next(max);
            int val2 = random.Next(max);

            if (val1 == val2)
                val2++;

            return search.AddSearchRangeParameter(factory, Math.Min(val1, val2).ToString(), Math.Max(val1, val2).ToString());
        }
    }
}
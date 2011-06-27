using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    internal static class EngineAssert
    {
        public static void ExecuteAndAssert(IEnumerable<EngineItem> source, Search<int> search, int skip, int take)
        {
            int totalCount;
            int[] primaryKeys = search.Execute(skip, take, out totalCount);

            HashSet<int> amongstPrimaryKeys = new HashSet<int>(search.AmongstPrimaryKeys);

            if (amongstPrimaryKeys.Count > 0)
                source = source.Where(item => amongstPrimaryKeys.Contains(item.Id));

            source = SourceSearch(source, search);

            if (search.SortParameters.Any() || search.SortPrimaryKeyAscending.HasValue)
                source = SourceSort(source, search);

            EngineItem[] sourceResults = source.ToArray();

            Assert.AreEqual(sourceResults.Length, totalCount);
            CollectionAssert.AreEqual(sourceResults.Skip(skip).Take(take).Select(item => item.Id).ToArray(), primaryKeys);

            foreach (IProjectionParameter param in search.ProjectionParameters)
                AssertProjection(sourceResults, param);
        }

        #region Search

        private static IEnumerable<EngineItem> SourceSearch(IEnumerable<EngineItem> source, Search<int> search)
        {
            foreach (ISearchParameter param in search.SearchParameters)
            {
                ISearchParameter closedParam = param;

                switch (param.Catalog.Name)
                {
                    case "SomeInt":
                        source = source.Where(item => SourceSearch(item.SomeInt, closedParam));
                        break;
                    case "SomeDateTime":
                        source = source.Where(item => SourceSearch(item.SomeDateTime, closedParam));
                        break;
                    case "SomeString":
                        source = source.Where(item => SourceSearch(item.SomeString, closedParam));
                        break;
                    default:
                        throw new NotImplementedException(param.Catalog.Name);
                }
            }

            return source;
        }

        private static bool SourceSearch(dynamic column, ISearchParameter param)
        {
            switch (param.ParameterType)
            {
                case SearchParameterType.Exact:
                    return column == param.DynamicExact;
                case SearchParameterType.Enumerable:
                    return Enumerable.Contains(param.DynamicEnumerable, column);
                case SearchParameterType.Range:
                    if (param.Catalog.Name == "SomeString")
                        return column.CompareTo(param.DynamicRangeMin) >= 0 && column.CompareTo(param.DynamicRangeMax) <= 0;
                    else
                        return column >= param.DynamicRangeMin && column <= param.DynamicRangeMax;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Sort

        private static IOrderedEnumerable<EngineItem> SourceSort(IEnumerable<EngineItem> source, Search<int> search)
        {
            IOrderedEnumerable<EngineItem> sortedSource;

            if (search.SortParameters.Any())
            {
                sortedSource = SourceSort(source, search.SortParameters.First());

                for (int i = 1; i < search.SortParameters.Count(); i++)
                    sortedSource = SourceSort(sortedSource, search.SortParameters.ToArray()[i]);

                if (search.SortPrimaryKeyAscending.HasValue)
                    return search.SortPrimaryKeyAscending.Value ? sortedSource.ThenBy(item => item.Id) : sortedSource.ThenByDescending(item => item.Id);
                else
                    return sortedSource;
            }
            else
                return search.SortPrimaryKeyAscending.Value ? source.OrderBy(item => item.Id) : source.OrderByDescending(item => item.Id);
        }

        private static IOrderedEnumerable<EngineItem> SourceSort(IEnumerable<EngineItem> source, ISortParameter param)
        {
            switch (param.ParameterType)
            {
                case SortParameterType.Directional:
                    switch (param.Catalog.Name)
                    {
                        case "SomeInt":
                            return param.Ascending ? source.OrderBy(item => item.SomeInt) : source.OrderByDescending(item => item.SomeInt);
                        case "SomeDateTime":
                            return param.Ascending ? source.OrderBy(item => item.SomeDateTime) : source.OrderByDescending(item => item.SomeDateTime);
                        case "SomeString":
                            return param.Ascending ? source.OrderBy(item => item.SomeString) : source.OrderByDescending(item => item.SomeString);
                        default:
                            throw new NotImplementedException(param.Catalog.Name);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private static IOrderedEnumerable<EngineItem> SourceSort(IOrderedEnumerable<EngineItem> source, ISortParameter param)
        {
            switch (param.ParameterType)
            {
                case SortParameterType.Directional:
                    switch (param.Catalog.Name)
                    {
                        case "SomeInt":
                            return param.Ascending ? source.ThenBy(item => item.SomeInt) : source.ThenByDescending(item => item.SomeInt);
                        case "SomeDateTime":
                            return param.Ascending ? source.ThenBy(item => item.SomeDateTime) : source.ThenByDescending(item => item.SomeDateTime);
                        case "SomeString":
                            return param.Ascending ? source.ThenBy(item => item.SomeString) : source.ThenByDescending(item => item.SomeString);
                        default:
                            throw new NotImplementedException(param.Catalog.Name);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Projection

        private static void AssertProjection(EngineItem[] sourceResults, IProjectionParameter param)
        {
            switch (param.Catalog.Name)
            {
                case "SomeInt":
                    Projection<int>[] sourceSomeIntProjection = sourceResults.GroupBy(item => item.SomeInt)
                        .Select(group => new Projection<int>(group.Key, group.Count()))
                        .OrderBy(projection => projection.Key)
                        .ToArray();

                    Projection<int>[] paramSomeIntProjection = ((IEnumerable<Projection<int>>)param.DynamicProjections)
                        .OrderBy(projection => projection.Key)
                        .ToArray();

                    CollectionAssert.AreEqual(sourceSomeIntProjection, paramSomeIntProjection);
                    break;
                case "SomeDateTime":
                    Projection<DateTime>[] sourceSomeDateTimeProjection = sourceResults.GroupBy(item => item.SomeDateTime)
                        .Select(group => new Projection<DateTime>(group.Key, group.Count()))
                        .OrderBy(projection => projection.Key)
                        .ToArray();

                    Projection<DateTime>[] paramSomeDateTimeProjection = ((IEnumerable<Projection<DateTime>>)param.DynamicProjections)
                        .OrderBy(projection => projection.Key)
                        .ToArray();

                    CollectionAssert.AreEqual(sourceSomeDateTimeProjection, paramSomeDateTimeProjection);
                    break;
                case "SomeString":
                    Projection<string>[] sourceSomeStringProjection = sourceResults.GroupBy(item => item.SomeString)
                        .Select(group => new Projection<string>(group.Key, group.Count()))
                        .OrderBy(projection => projection.Key)
                        .ToArray();

                    Projection<string>[] paramSomeStringProjection = ((IEnumerable<Projection<string>>)param.DynamicProjections)
                        .OrderBy(projection => projection.Key)
                        .ToArray();

                    CollectionAssert.AreEqual(sourceSomeStringProjection, paramSomeStringProjection);
                    break;
                default:
                    throw new NotImplementedException(param.Catalog.Name);
            }
        }

        #endregion
    }
}
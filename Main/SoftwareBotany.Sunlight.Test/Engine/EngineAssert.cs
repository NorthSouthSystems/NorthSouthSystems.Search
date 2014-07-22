namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    internal static class EngineAssert
    {
        public static void ExecuteAndAssert(IEnumerable<EngineItem> source, Query<EngineItem, int> query, int skip, int take)
        {
            query.Execute(skip, take);

            HashSet<int> amongstPrimaryKeys = new HashSet<int>(query.AmongstPrimaryKeys);

            if (amongstPrimaryKeys.Count > 0)
                source = source.Where(item => amongstPrimaryKeys.Contains(item.Id));

            source = SourceFilter(source, query);

            if (query.SortParameters.Any() || query.SortPrimaryKeyAscending.HasValue)
                source = SourceSort(source, query);

            EngineItem[] sourceResults = source.ToArray();

            Assert.AreEqual(sourceResults.Length, query.ResultTotalCount);
            CollectionAssert.AreEqual(sourceResults.Skip(skip).Take(take).Select(item => item.Id).ToArray(), query.ResultPrimaryKeys);

            foreach (var facetParam in query.FacetParametersInternal)
                AssertFacet(sourceResults, facetParam, query.FacetShortCircuitCounting);
        }

        #region Filter

        private static IEnumerable<EngineItem> SourceFilter(IEnumerable<EngineItem> source, Query<EngineItem, int> query)
        {
            foreach (IFilterParameter param in query.FilterClause == null ? Enumerable.Empty<IFilterParameter>() : query.FilterClause.SubClauses.Cast<IFilterParameter>())
            {
                IFilterParameter closedParam = param;

                switch (param.Catalog.Name)
                {
                    case "SomeInt":
                        source = source.Where(item => SourceFilter(item.SomeInt, closedParam));
                        break;
                    case "SomeDateTime":
                        source = source.Where(item => SourceFilter(item.SomeDateTime, closedParam));
                        break;
                    case "SomeString":
                        source = source.Where(item => SourceFilter(item.SomeString, closedParam));
                        break;
                    case "SomeTags":
                        source = source.Where(item => item.SomeTags.Any(tag => SourceFilter(tag, closedParam)));
                        break;
                    default:
                        throw new NotImplementedException(param.Catalog.Name);
                }
            }

            return source;
        }

        private static bool SourceFilter(object column, IFilterParameter param)
        {
            switch (param.ParameterType)
            {
                case FilterParameterType.Exact:
                    return object.Equals(column, param.Exact);
                case FilterParameterType.Enumerable:
                    return ((IEnumerable)param.Enumerable).Cast<object>().Any(obj => object.Equals(column, obj));
                case FilterParameterType.Range:
                    return ((IComparable)column).CompareTo(param.RangeMin) >= 0 && ((IComparable)column).CompareTo(param.RangeMax) <= 0;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Sort

        private static IOrderedEnumerable<EngineItem> SourceSort(IEnumerable<EngineItem> source, Query<EngineItem, int> query)
        {
            IOrderedEnumerable<EngineItem> sortedSource;

            if (query.SortParameters.Any())
            {
                sortedSource = SourceSort(source, query.SortParameters.First());

                for (int i = 1; i < query.SortParameters.Count(); i++)
                    sortedSource = SourceSort(sortedSource, query.SortParameters.ToArray()[i]);

                if (query.SortPrimaryKeyAscending.HasValue)
                    return query.SortPrimaryKeyAscending.Value ? sortedSource.ThenBy(item => item.Id) : sortedSource.ThenByDescending(item => item.Id);
                else
                    return sortedSource;
            }
            else
                return query.SortPrimaryKeyAscending.Value ? source.OrderBy(item => item.Id) : source.OrderByDescending(item => item.Id);
        }

        private static IOrderedEnumerable<EngineItem> SourceSort(IEnumerable<EngineItem> source, ISortParameter param)
        {
            switch (param.Catalog.Name)
            {
                case "SomeInt":
                    return param.Ascending ? source.OrderBy(item => item.SomeInt) : source.OrderByDescending(item => item.SomeInt);
                case "SomeDateTime":
                    return param.Ascending ? source.OrderBy(item => item.SomeDateTime) : source.OrderByDescending(item => item.SomeDateTime);
                case "SomeString":
                    return param.Ascending ? source.OrderBy(item => item.SomeString) : source.OrderByDescending(item => item.SomeString);
                case "SomeTags":
                    return param.Ascending ? source.OrderBy(item => item.SomeTags.Min()) : source.OrderByDescending(item => item.SomeTags.Max());
                default:
                    throw new NotImplementedException(param.Catalog.Name);
            }
        }

        private static IOrderedEnumerable<EngineItem> SourceSort(IOrderedEnumerable<EngineItem> source, ISortParameter param)
        {
            switch (param.Catalog.Name)
            {
                case "SomeInt":
                    return param.Ascending ? source.ThenBy(item => item.SomeInt) : source.ThenByDescending(item => item.SomeInt);
                case "SomeDateTime":
                    return param.Ascending ? source.ThenBy(item => item.SomeDateTime) : source.ThenByDescending(item => item.SomeDateTime);
                case "SomeString":
                    return param.Ascending ? source.ThenBy(item => item.SomeString) : source.ThenByDescending(item => item.SomeString);
                case "SomeTags":
                    return param.Ascending ? source.ThenBy(item => item.SomeTags.Min()) : source.ThenByDescending(item => item.SomeTags.Max());
                default:
                    throw new NotImplementedException(param.Catalog.Name);
            }
        }

        #endregion

        #region Facet

        private static void AssertFacet(EngineItem[] sourceResults, IFacetParameterInternal param, bool shortCircuitCounting)
        {
            switch (param.Catalog.Name)
            {
                case "SomeInt":
                    var sourceSomeIntFacet = sourceResults.GroupBy(item => item.SomeInt)
                        .Select(group => new FacetCategory<int>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    var paramSomeIntFacet = param.Facet
                        .Categories
                        .Cast<FacetCategory<int>>()
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    sourceSomeIntFacet.AssertFacet(paramSomeIntFacet, shortCircuitCounting);
                    break;
                case "SomeDateTime":
                    var sourceSomeDateTimeFacet = sourceResults.GroupBy(item => item.SomeDateTime)
                        .Select(group => new FacetCategory<DateTime>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    var paramSomeDateTimeFacet = param.Facet
                        .Categories
                        .Cast<FacetCategory<DateTime>>()
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    sourceSomeDateTimeFacet.AssertFacet(paramSomeDateTimeFacet, shortCircuitCounting);
                    break;
                case "SomeString":
                    var sourceSomeStringFacet = sourceResults.GroupBy(item => item.SomeString)
                        .Select(group => new FacetCategory<string>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    var paramSomeStringFacet = param.Facet
                        .Categories
                        .Cast<FacetCategory<string>>()
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    sourceSomeStringFacet.AssertFacet(paramSomeStringFacet, shortCircuitCounting);
                    break;
                case "SomeTags":
                    var sourceSomeTagsFacet = sourceResults.SelectMany(item => item.SomeTags)
                        .GroupBy(tag => tag)
                        .Select(group => new FacetCategory<string>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    var paramSomeTagsFacet = param.Facet
                        .Categories
                        .Cast<FacetCategory<string>>()
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    sourceSomeTagsFacet.AssertFacet(paramSomeTagsFacet, shortCircuitCounting);
                    break;
                default:
                    throw new NotImplementedException(param.Catalog.Name);
            }
        }

        private static void AssertFacet<T>(this FacetCategory<T>[] categories, FacetCategory<T>[] compare, bool shortCircuitCounting)
            where T : IEquatable<T>, IComparable<T>
        {
            Assert.AreEqual(categories.Length, compare.Length);

            for (int i = 0; i < categories.Length; i++)
            {
                Assert.AreEqual(categories[i].Key, compare[i].Key);
                Assert.AreEqual(shortCircuitCounting ? 1 : categories[i].Count, compare[i].Count);
            }
        }

        #endregion
    }
}
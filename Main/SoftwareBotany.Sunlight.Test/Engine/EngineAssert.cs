using System;
using System.Collections;
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

            foreach (var facetParam in search.FacetParameters)
                AssertFacet(sourceResults, facetParam, search.FacetShortCircuitCounting);
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
                    case "SomeTags":
                        source = source.Where(item => item.SomeTags.Any(tag => SourceSearch(tag, closedParam)));
                        break;
                    default:
                        throw new NotImplementedException(param.Catalog.Name);
                }
            }

            return source;
        }

        private static bool SourceSearch(object column, ISearchParameter param)
        {
            switch (param.ParameterType)
            {
                case SearchParameterType.Exact:
                    return object.Equals(column, param.Exact);
                case SearchParameterType.Enumerable:
                    return ((IEnumerable)param.Enumerable).Cast<object>().Any(obj => object.Equals(column, obj));
                case SearchParameterType.Range:
                    return ((IComparable)column).CompareTo(param.RangeMin) >= 0 && ((IComparable)column).CompareTo(param.RangeMax) <= 0;
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
                        case "SomeTags":
                            return param.Ascending ? source.OrderBy(item => item.SomeTags.Min()) : source.OrderByDescending(item => item.SomeTags.Max());
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
                        case "SomeTags":
                            return param.Ascending ? source.ThenBy(item => item.SomeTags.Min()) : source.ThenByDescending(item => item.SomeTags.Max());
                        default:
                            throw new NotImplementedException(param.Catalog.Name);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Facet

        private static void AssertFacet(EngineItem[] sourceResults, IFacetParameter param, bool shortCircuitCounting)
        {
            switch (param.Catalog.Name)
            {
                case "SomeInt":
                    var sourceSomeIntFacet = sourceResults.GroupBy(item => item.SomeInt)
                        .Select(group => new FacetCategory<int>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    var paramSomeIntFacet = ((IEnumerable<FacetCategory<int>>)param.Facet)
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    sourceSomeIntFacet.AssertFacet(paramSomeIntFacet, shortCircuitCounting);
                    break;
                case "SomeDateTime":
                    var sourceSomeDateTimeFacet = sourceResults.GroupBy(item => item.SomeDateTime)
                        .Select(group => new FacetCategory<DateTime>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    var paramSomeDateTimeFacet = ((IEnumerable<FacetCategory<DateTime>>)param.Facet)
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    sourceSomeDateTimeFacet.AssertFacet(paramSomeDateTimeFacet, shortCircuitCounting);
                    break;
                case "SomeString":
                    var sourceSomeStringFacet = sourceResults.GroupBy(item => item.SomeString)
                        .Select(group => new FacetCategory<string>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    var paramSomeStringFacet = ((IEnumerable<FacetCategory<string>>)param.Facet)
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

                    var paramSomeTagsFacet = ((IEnumerable<FacetCategory<string>>)param.Facet)
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
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

            // Testing convention: every FacetParameter must have a corresponding FacetAnyParameter.
            var facetAndAnyParameters = search.FacetParameters
                .Join(search.FacetAnyParameters,
                    facetParameter => facetParameter.Catalog,
                    facetAnyParameter => facetAnyParameter.Catalog,
                    (facetParameter, facetAnyParameter) => Tuple.Create(facetParameter, facetAnyParameter))
                .ToArray();

            Assert.AreEqual(search.FacetParameters.Count(), facetAndAnyParameters.Length);
            Assert.AreEqual(search.FacetAnyParameters.Count(), facetAndAnyParameters.Length);

            foreach (var facetAndAnyParam in facetAndAnyParameters)
                AssertFacet(sourceResults, facetAndAnyParam.Item1, facetAndAnyParam.Item2);
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
                        source = source.Where(item => SourceSearchTags(item.SomeTags, closedParam));
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
                    return column == (dynamic)param.Exact;
                case SearchParameterType.Enumerable:
                    return Enumerable.Contains((dynamic)param.Enumerable, column);
                case SearchParameterType.Range:
                    if (param.Catalog.Name == "SomeString")
                        return column.CompareTo((dynamic)param.RangeMin) >= 0 && column.CompareTo((dynamic)param.RangeMax) <= 0;
                    else
                        return column >= (dynamic)param.RangeMin && column <= (dynamic)param.RangeMax;
                default:
                    throw new NotImplementedException();
            }
        }

        private static bool SourceSearchTags(string[] tags, ISearchParameter param)
        {
            switch (param.ParameterType)
            {
                case SearchParameterType.Exact:
                    return tags.Any(tag => tag == (string)param.Exact);
                case SearchParameterType.Enumerable:
                    return tags.Any(tag => Enumerable.Contains((IEnumerable<string>)param.Enumerable, tag));
                case SearchParameterType.Range:
                    return tags.Any(tag => tag.CompareTo((string)param.RangeMin) >= 0 && tag.CompareTo((string)param.RangeMax) <= 0);
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

        private static void AssertFacet(EngineItem[] sourceResults, IFacetParameter param, IFacetAnyParameter paramAny)
        {
            switch (param.Catalog.Name)
            {
                case "SomeInt":
                    Facet<int>[] sourceSomeIntFacet = sourceResults.GroupBy(item => item.SomeInt)
                        .Select(group => new Facet<int>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    Facet<int>[] paramSomeIntFacet = ((IEnumerable<Facet<int>>)param.Facets)
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    int[] paramAnySomeIntFacet = ((IEnumerable<int>)paramAny.FacetAnys)
                        .OrderBy(key => key)
                        .ToArray();

                    sourceSomeIntFacet.AssertFacets(paramSomeIntFacet, paramAnySomeIntFacet);
                    break;
                case "SomeDateTime":
                    Facet<DateTime>[] sourceSomeDateTimeFacet = sourceResults.GroupBy(item => item.SomeDateTime)
                        .Select(group => new Facet<DateTime>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    Facet<DateTime>[] paramSomeDateTimeFacet = ((IEnumerable<Facet<DateTime>>)param.Facets)
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    DateTime[] paramAnySomeDateTimeFacet = ((IEnumerable<DateTime>)paramAny.FacetAnys)
                        .OrderBy(key => key)
                        .ToArray();

                    sourceSomeDateTimeFacet.AssertFacets(paramSomeDateTimeFacet, paramAnySomeDateTimeFacet);
                    break;
                case "SomeString":
                    Facet<string>[] sourceSomeStringFacet = sourceResults.GroupBy(item => item.SomeString)
                        .Select(group => new Facet<string>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    Facet<string>[] paramSomeStringFacet = ((IEnumerable<Facet<string>>)param.Facets)
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    string[] paramAnySomeStringFacet = ((IEnumerable<string>)paramAny.FacetAnys)
                        .OrderBy(key => key)
                        .ToArray();

                    sourceSomeStringFacet.AssertFacets(paramSomeStringFacet, paramAnySomeStringFacet);
                    break;
                case "SomeTags":
                    Facet<string>[] sourceSomeTagsFacet = sourceResults.SelectMany(item => item.SomeTags)
                        .GroupBy(tag => tag)
                        .Select(group => new Facet<string>(group.Key, group.Count()))
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    Facet<string>[] paramSomeTagsFacet = ((IEnumerable<Facet<string>>)param.Facets)
                        .OrderBy(facet => facet.Key)
                        .ToArray();

                    string[] paramAnySomeTagsFacet = ((IEnumerable<string>)paramAny.FacetAnys)
                        .OrderBy(key => key)
                        .ToArray();

                    sourceSomeTagsFacet.AssertFacets(paramSomeTagsFacet, paramAnySomeTagsFacet);
                    break;
                default:
                    throw new NotImplementedException(param.Catalog.Name);
            }
        }

        private static void AssertFacets<T>(this Facet<T>[] facets, Facet<T>[] compare, T[] compareAny)
            where T : IEquatable<T>, IComparable<T>
        {
            Assert.AreEqual(facets.Length, compare.Length);
            Assert.AreEqual(facets.Length, compareAny.Length);

            for (int i = 0; i < facets.Length; i++)
            {
                Assert.AreEqual(facets[i].Key, compare[i].Key);
                Assert.AreEqual(facets[i].Count, compare[i].Count);

                Assert.AreEqual(facets[i].Key, compareAny[i]);
            }
        }

        #endregion
    }
}
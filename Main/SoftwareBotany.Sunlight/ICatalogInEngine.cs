using System.Collections;

namespace SoftwareBotany.Sunlight
{
    internal interface ICatalogInEngine : ICatalogHandle
    {
        void OptimizeReadPhase(int[] bitPositionShifts);
        void OptimizeWritePhase();

        void Set(object key, int bitPosition, bool value);

        IFilterParameter CreateFilterParameter(object exact);
        IFilterParameter CreateFilterParameter(IEnumerable enumerable);
        IFilterParameter CreateFilterParameter(object rangeMin, object rangeMax);

        void FilterExact(Vector vector, object key);
        void FilterEnumerable(Vector vector, IEnumerable keys);
        void FilterRange(Vector vector, object keyMin, object keyMax);

        ISortParameter CreateSortParameter(bool ascending);

        ICatalogInEngineSortResult SortBitPositions(Vector vector, bool value, bool ascending);

        IFacetParameterInternal CreateFacetParameter();

        IFacet Facet(Vector vector, bool disableParallel, bool shortCircuitCounting);

        ICatalogStatistics GenerateStatistics();
    }
}
namespace SoftwareBotany.Sunlight
{
    internal interface ICatalogInEngine
    {
        string Name { get; }

        void OptimizeReadPhase(int[] bitPositionShifts);
        void OptimizeWritePhase();

        void Set(object key, int bitPosition, bool value);

        void FilterExact(Vector vector, object key);
        void FilterEnumerable(Vector vector, object keys);
        void FilterRange(Vector vector, object keyMin, object keyMax);

        ICatalogInEngineSortResult SortBitPositions(Vector vector, bool value, bool ascending);

        IFacet Facet(Vector vector, bool disableParallel, bool shortCircuitCounting);

        ICatalogStatistics GenerateStatistics();
    }
}
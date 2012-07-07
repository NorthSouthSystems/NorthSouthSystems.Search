namespace SoftwareBotany.Sunlight
{
    internal interface ICatalog
    {
        string Name { get; }

        void OptimizeReadPhase(int[] bitPositionShifts);
        void OptimizeWritePhase();

        void Set(object key, int bitPosition, bool value);

        void SearchExact(Vector vector, object key);
        void SearchEnumerable(Vector vector, object keys);
        void SearchRange(Vector vector, object keyMin, object keyMax);

        ICatalogSortResult SortBitPositions(Vector vector, bool value, bool ascending);

        object Facets(Vector vector);

        ICatalogStatistics GenerateStatistics();
    }
}
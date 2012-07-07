namespace SoftwareBotany.Sunlight
{
    internal interface IEngine<TPrimaryKey>
    {
        bool HasCatalog(ICatalog catalog);
        TPrimaryKey[] Search(Search<TPrimaryKey> search, int skip, int take, out int totalCount);
    }
}
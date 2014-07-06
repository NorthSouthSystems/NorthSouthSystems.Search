namespace SoftwareBotany.Sunlight
{
    internal interface IEngine<TPrimaryKey>
    {
        bool HasCatalog(ICatalog catalog);
        TPrimaryKey[] ExecuteQuery(Query<TPrimaryKey> query, int skip, int take, out int totalCount);
    }
}
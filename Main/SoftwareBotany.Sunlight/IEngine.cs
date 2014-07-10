namespace SoftwareBotany.Sunlight
{
    internal interface IEngine<TPrimaryKey>
    {
        bool HasCatalog(ICatalogHandle catalog);
        TPrimaryKey[] ExecuteQuery(Query<TPrimaryKey> query, int skip, int take, out int totalCount);
    }
}
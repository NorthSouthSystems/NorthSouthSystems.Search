namespace SoftwareBotany.Sunlight
{
    internal interface IEngine<TPrimaryKey>
    {
        bool HasCatalog(ICatalogHandle catalog);
        ICatalogInEngine GetCatalog(string name);

        TPrimaryKey[] ExecuteQuery(Query<TPrimaryKey> query, int skip, int take, out int totalCount);
    }
}
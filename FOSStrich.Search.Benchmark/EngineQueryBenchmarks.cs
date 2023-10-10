namespace FOSStrich.Search;

using FOSStrich.StackExchange;
using V = FOSStrich.BitVectors.PLWAH.Vector;

[MemoryDiagnoser]
public class EngineQueryBenchmarks : EngineBenchmarksBase
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        ConstructEngine();

        var posts = new StackExchangeSiteSerializer(Program.StackExchangeDirectory, Program.StackExchangeSite)
            .DeserializeMemoryPack<Post>();

        Engine.Add(posts);
    }

    [GlobalCleanup]
    public void GlobalCleanup() => Engine?.Dispose();

    [Benchmark]
    public void Filter() => Filter(Engine.CreateQuery()).Execute(0, 100);

    [Benchmark]
    public void Sort() => Sort(Engine.CreateQuery()).Execute(0, 100);

    [Benchmark]
    public void Facet() => Facet(Engine.CreateQuery()).Execute(0, 100);

    [Benchmark]
    public void Full() => Facet(Sort(Filter(Engine.CreateQuery()))).Execute(0, 100);

    private Query<V, Post, int> Filter(Query<V, Post, int> query)
    {
        const int jonSkeetUserId = 22656;

        return query.Filter(FilterParameter.Create(PostTypeCatalog, (byte)1)
            && FilterParameter.Create(OwnerUserIdCatalog, jonSkeetUserId));
    }

    private Query<V, Post, int> Sort(Query<V, Post, int> query) =>
        query.Sort(SortParameter.Create(CreationDateCatalog, false));

    private Query<V, Post, int> Facet(Query<V, Post, int> query) =>
        query.Facet(FacetParameter.Create<V, byte>(PostTypeCatalog),
            FacetParameter.Create<V, DateTime>(CreationDateCatalog),
            FacetParameter.Create<V, DateTime>(LastActivityDateCatalog),
            FacetParameter.Create<V, int>(ViewCountCatalog),
            FacetParameter.Create<V, string>(TagsCatalog),
            FacetParameter.Create<V, int>(AnswerCountCatalog),
            FacetParameter.Create<V, int>(CommentCountCatalog),
            FacetParameter.Create<V, int>(FavoriteCountCatalog));
}
namespace FOSStrich.Search;

using FOSStrich.StackExchange;

[MemoryDiagnoser]
public class EngineQueryBenchmarks : EngineBenchmarksBase
{
    [Params(VectorCompression.Compressed, VectorCompression.CompressedWithPackedPosition)]
    public VectorCompression Compression { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        ConstructEngine(Compression);

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

    private Query<Post, int> Filter(Query<Post, int> query)
    {
        const int jonSkeetUserId = 22656;

        return query.Filter(FilterParameter.Create(PostTypeCatalog, (byte)1)
            && FilterParameter.Create(OwnerUserIdCatalog, jonSkeetUserId));
    }

    private Query<Post, int> Sort(Query<Post, int> query) =>
        query.Sort(SortParameter.Create(CreationDateCatalog, false));

    private Query<Post, int> Facet(Query<Post, int> query) =>
        query.Facet(FacetParameter.Create(PostTypeCatalog),
            FacetParameter.Create(CreationDateCatalog),
            FacetParameter.Create(LastActivityDateCatalog),
            FacetParameter.Create(ViewCountCatalog),
            FacetParameter.Create(TagsCatalog),
            FacetParameter.Create(AnswerCountCatalog),
            FacetParameter.Create(CommentCountCatalog),
            FacetParameter.Create(FavoriteCountCatalog));
}
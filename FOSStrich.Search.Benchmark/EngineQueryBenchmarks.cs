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
    public void Query()
    {
        const int jonSkeetUserId = 22656;

        int[] resultPrimaryKeys = Engine.CreateQuery()
            .Filter(FilterParameter.Create(PostTypeCatalog, (byte)1)
                && FilterParameter.Create(OwnerUserIdCatalog, jonSkeetUserId))
            .Sort(SortParameter.Create(CreationDateCatalog, false))
            .Facet(FacetParameter.Create(PostTypeCatalog),
                FacetParameter.Create(CreationDateCatalog),
                FacetParameter.Create(LastActivityDateCatalog),
                FacetParameter.Create(ViewCountCatalog),
                FacetParameter.Create(TagsCatalog),
                FacetParameter.Create(AnswerCountCatalog),
                FacetParameter.Create(CommentCountCatalog),
                FacetParameter.Create(FavoriteCountCatalog))
            .Execute(0, 100)
            .ResultPrimaryKeys;
    }
}
namespace FOSStrich.Search;

[MemoryDiagnoser]
public class EngineAddBenchmarks : EngineBenchmarksBase
{
    [Params(true)]
    public bool AllowUnsafe { get; set; }

    [Params(VectorCompression.Compressed, VectorCompression.CompressedWithPackedPosition)]
    public VectorCompression Compression { get; set; }

    [GlobalSetup]
    public void GlobalSetup() =>
        _posts = new StackExchangeSiteSerializer(Program.StackExchangeDirectory, Program.StackExchangeSite)
            .DeserializeMemoryPackParallel<Post>()
            .Where(p => p.CreationDate.Year < 2011)
            .ToList();

    private List<Post> _posts;

    [Benchmark]
    public void Add()
    {
        using var engine = ConstructEngine(AllowUnsafe, Compression);

        engine.Add(_posts);
    }
}
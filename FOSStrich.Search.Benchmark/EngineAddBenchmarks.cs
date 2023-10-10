namespace FOSStrich.Search;

using FOSStrich.StackExchange;

[MemoryDiagnoser]
public class EngineAddBenchmarks : EngineBenchmarksBase
{
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
        using var engine = ConstructEngine();

        engine.Add(_posts);
    }
}
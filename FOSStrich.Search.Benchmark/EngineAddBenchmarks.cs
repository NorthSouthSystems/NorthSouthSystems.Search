namespace FOSStrich.Search;

using FOSStrich.BitVectors;
using FOSStrich.StackExchange;

[MemoryDiagnoser]
[GenericTypeArguments(typeof(FOSStrich.BitVectors.PLWAH.PLWAHVectorFactory), typeof(FOSStrich.BitVectors.PLWAH.Vector))]
[GenericTypeArguments(typeof(FOSStrich.BitVectors.WAH.WAHVectorFactory), typeof(FOSStrich.BitVectors.WAH.Vector))]
public class EngineAddBenchmarks<TBitVectorFactory, TBitVector> : EngineBenchmarksBase<TBitVector>
    where TBitVectorFactory : IBitVectorFactory<TBitVector>
    where TBitVector : IBitVector<TBitVector>
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        _bitVectorFactory = Activator.CreateInstance<TBitVectorFactory>();

        _posts = new StackExchangeSiteSerializer(Program.StackExchangeDirectory, Program.StackExchangeSite)
            .DeserializeMemoryPackParallel<Post>()
            .Where(p => p.CreationDate.Year < 2011)
            .ToList();
    }

    private TBitVectorFactory _bitVectorFactory;
    private List<Post> _posts;

    [Benchmark]
    public void Add()
    {
        using var engine = ConstructEngine(_bitVectorFactory);

        engine.Add(_posts);
    }
}
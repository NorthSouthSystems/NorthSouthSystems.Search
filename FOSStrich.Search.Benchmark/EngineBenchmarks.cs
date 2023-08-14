namespace FOSStrich.Search;

public class EngineBenchmarks
{
    [Params(false, true)]
    public bool AllowUnsafe { get; set; }

    [Params(VectorCompression.None, VectorCompression.Compressed, VectorCompression.CompressedWithPackedPosition)]
    public VectorCompression Compression { get; set; }

    [Benchmark]
    public void Sanity()
    {
        using var engine = new Engine<SimpleItem, int>(AllowUnsafe, item => item.Id);

        var catalog = engine.CreateCatalog(nameof(SimpleItem.SomeInt), Compression, item => item.SomeInt);

        engine.Add(new SimpleItem { Id = 43, SomeInt = 0 });

        var query = engine.CreateQuery();
        query.Amongst(new[] { 43, 44 });

        query.Execute(0, 10);
    }

    private class SimpleItem
    {
        public int Id;
        public int SomeInt;
    }
}
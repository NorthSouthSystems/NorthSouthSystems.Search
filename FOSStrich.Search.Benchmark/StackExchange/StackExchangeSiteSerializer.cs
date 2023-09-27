namespace FOSStrich.Search;

using FOSStrich.Xml.Linq;
using MemoryPack;
using MemoryPack.Compression;
using MoreLinq;
using System.IO;
using System.Xml;
using System.Xml.Linq;

internal class StackExchangeSiteSerializer
{
    private const string SevenZipExtension = ".7z";
    private const string MemoryPackExtension = ".mp.brotli";

    private const int MemoryPackBatchSize = 1_000_000;

    internal StackExchangeSiteSerializer(string stackExchangeDirectory, string stackExchangeSite)
    {
        _stackExchangeDirectory = stackExchangeDirectory;
        _stackExchangeSite = stackExchangeSite;
    }

    private readonly string _stackExchangeDirectory;
    private readonly string _stackExchangeSite;

    private string SiteDirectory => Path.Combine(_stackExchangeDirectory, _stackExchangeSite);
    private string MemoryPackDirectory<T>() => Path.Combine(SiteDirectory, typeof(T).Name);

    internal static void DeserializeSevenZippedXmlAndSerializeMemoryPackAll(string stackExchangeDirectory, string stackExchangeSite)
    {
        var serializer = new StackExchangeSiteSerializer(stackExchangeDirectory, stackExchangeSite);

        F(xe => new Post(xe));
        F(xe => new User(xe));
        F(xe => new Vote(xe));

        void F<T>(Func<XElement, T> fromXElementConstructor) =>
            serializer.SerializeMemoryPack(
                serializer.DeserializeSevenZippedXml(fromXElementConstructor));
    }

    internal IEnumerable<T> DeserializeSevenZippedXml<T>(Func<XElement, T> fromXElementConstructor)
    {
        var sevenZipFilepaths = Directory.GetFiles(_stackExchangeDirectory, $"{_stackExchangeSite}*{SevenZipExtension}")
            .Where(fp =>
                !Path.GetFileNameWithoutExtension(fp).EndsWith("-PostHistory", StringComparison.OrdinalIgnoreCase));

        foreach (string sevenZipFilepath in sevenZipFilepaths)
        {
            string extractDirectory = Path.Combine(SiteDirectory, Path.GetFileNameWithoutExtension(sevenZipFilepath));

            if (!Directory.Exists(extractDirectory))
                SevenZipConsole.ExtractAllFiles(sevenZipFilepath, extractDirectory);

            string xmlFilepath = Path.Combine(extractDirectory, $"{typeof(T).Name}s.xml");

            if (!File.Exists(xmlFilepath))
                continue;

            using var xmlReader = XmlReader.Create(xmlFilepath);

            foreach (T t in XElementSimpleStreamer.Stream(xmlReader, "row").Select(fromXElementConstructor))
                yield return t;
        }
    }

    internal void SerializeMemoryPack<T>(IEnumerable<T> source)
    {
        if (Directory.Exists(MemoryPackDirectory<T>()))
            throw new InvalidOperationException();

        Directory.CreateDirectory(MemoryPackDirectory<T>());

        int batchNumber = 1;

        foreach (var batch in source.Batch(MemoryPackBatchSize))
        {
            string batchFilename = $"{typeof(T).Name}_{batchNumber++:0000}{MemoryPackExtension}";
            string batchFilepath = Path.Combine(MemoryPackDirectory<T>(), batchFilename);

            using var memoryPackBrotli = new BrotliCompressor();

            MemoryPackSerializer.Serialize(memoryPackBrotli, batch.ToList());
            File.WriteAllBytes(batchFilepath, memoryPackBrotli.ToArray());
        }
    }

    internal IEnumerable<T> DeserializeMemoryPack<T>() =>
        DeserializeMemoryPackFilepaths<T>()
            .Select(File.ReadAllBytes)
            .SelectMany(DeserializeMemoryPackBrotliBytes<T>);

    internal ParallelQuery<T> DeserializeMemoryPackParallel<T>() =>
        DeserializeMemoryPackFilepaths<T>()
            .AsParallel()
            .Select(File.ReadAllBytes)
            .SelectMany(DeserializeMemoryPackBrotliBytes<T>);

    private string[] DeserializeMemoryPackFilepaths<T>() =>
        Directory.GetFiles(MemoryPackDirectory<T>(), $"{typeof(T).Name}_*{MemoryPackExtension}");

    private static List<T> DeserializeMemoryPackBrotliBytes<T>(byte[] bytes)
    {
        using var memoryPackBrotli = new BrotliDecompressor();

        return MemoryPackSerializer.Deserialize<List<T>>(memoryPackBrotli.Decompress(bytes));
    }
}
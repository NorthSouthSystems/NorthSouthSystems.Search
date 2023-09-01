namespace FOSStrich.Search;

using FOSStrich.Xml.Linq;
using MemoryPack;
using MemoryPack.Compression;
using MoreLinq;
using System.IO;
using System.Xml;
using System.Xml.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            BenchmarkDotNet.Running.BenchmarkRunner.Run<EngineBenchmarks>();
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "convert":
                string stackExchangeDirectory = args[1];
                string stackExchangeSite = args[2];

                string siteDirectory = Path.Combine(stackExchangeDirectory, stackExchangeSite);

                var filepaths = Directory.GetFiles(stackExchangeDirectory, stackExchangeSite + "*.7z")
                    .Where(filepath =>
                        !Path.GetFileNameWithoutExtension(filepath).EndsWith("-PostHistory", StringComparison.OrdinalIgnoreCase));

                foreach (string filepath in filepaths)
                    ConvertAll(Extract(filepath));

                string Extract(string filepath)
                {
                    string extractDirectory = Path.Combine(siteDirectory, Path.GetFileNameWithoutExtension(filepath));

                    if (!Directory.Exists(extractDirectory))
                        SevenZipConsole.ExtractAllFiles(filepath, extractDirectory);

                    return extractDirectory;
                }

                void ConvertAll(string extractDirectory)
                {
                    Convert(Filepath("Posts.xml"), xe => new Post(xe));
                    Convert(Filepath("Users.xml"), xe => new User(xe));
                    Convert(Filepath("Votes.xml"), xe => new Vote(xe));

                    string Filepath(string filename) => Path.Combine(extractDirectory, filename);
                }

                void Convert<T>(string filepath, Func<XElement, T> constructor)
                {
                    if (!File.Exists(filepath))
                        return;

                    string conversionDirectory = Path.Combine(siteDirectory, typeof(T).Name);

                    if (Directory.Exists(conversionDirectory))
                        return;

                    Directory.CreateDirectory(conversionDirectory);

                    using var xmlReader = XmlReader.Create(filepath);

                    int batchNumber = 1;

                    foreach (var batch in XElementSimpleStreamer.Stream(xmlReader, "row")
                        .Select(constructor)
                        .Batch(5_000_000))
                    {
                        string batchFilename = FormattableString.Invariant($"{typeof(T).Name}_{batchNumber++}.mp.brotli");
                        string batchFilepath = Path.Combine(conversionDirectory, batchFilename);

                        using var brotli = new BrotliCompressor();
                        MemoryPackSerializer.Serialize(brotli, batch.ToList());
                        File.WriteAllBytes(batchFilepath, brotli.ToArray());
                    }
                }
                break;
        }
    }
}
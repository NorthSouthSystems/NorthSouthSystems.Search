namespace FOSStrich.Search;

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
                SevenZippedXmlToMemoryPack.ExtractAndConvert(args[1], args[2]);
                break;
        }
    }
}
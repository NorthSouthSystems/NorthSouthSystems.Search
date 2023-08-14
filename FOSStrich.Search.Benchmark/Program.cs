namespace FOSStrich.Search;

internal class Program
{
    private static void Main(string[] args) =>
        BenchmarkDotNet.Running.BenchmarkRunner.Run<EngineBenchmarks>();
}
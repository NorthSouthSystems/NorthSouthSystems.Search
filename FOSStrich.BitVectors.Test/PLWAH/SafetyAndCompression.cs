namespace FOSStrich.BitVectors.PLWAH;

internal class SafetyAndCompression
{
    internal SafetyAndCompression(VectorCompression compression) =>
        Compression = compression;

    internal VectorCompression Compression { get; private set; }

    internal static VectorCompression RandomCompression(Random random)
    {
        var vectorCompressions = Enum.GetValues(typeof(VectorCompression))
            .Cast<VectorCompression>()
            .ToArray();

        return vectorCompressions.Skip(random.Next(vectorCompressions.Length)).First();
    }

    internal static void RunAll(Action<SafetyAndCompression> test) =>
        RunAllCompressions(compression =>
            test(new SafetyAndCompression(compression)));

    internal static void RunAllCompressions(Action<VectorCompression> test)
    {
        foreach (var compression in Enum.GetValues(typeof(VectorCompression)).Cast<VectorCompression>())
            test(compression);
    }
}
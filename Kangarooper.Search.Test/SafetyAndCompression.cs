namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Linq;

    internal class SafetyAndCompression
    {
        internal SafetyAndCompression(bool allowUnsafe, VectorCompression compression)
        {
            AllowUnsafe = allowUnsafe;
            Compression = compression;
        }

        internal bool AllowUnsafe { get; private set; }
        internal VectorCompression Compression { get; private set; }

        internal static VectorCompression RandomCompression(Random random)
        {
            var vectorCompressions = Enum.GetValues(typeof(VectorCompression))
                .Cast<VectorCompression>()
                .ToArray();

            return vectorCompressions.Skip(random.Next(vectorCompressions.Length)).First();
        }

        internal static void RunAll(Action<SafetyAndCompression> test)
        {
            RunAllSafeties(allowUnsafe =>
                RunAllCompressions(compression =>
                    test(new SafetyAndCompression(allowUnsafe, compression))));
        }

        internal static void RunAllSafeties(Action<bool> test)
        {
            test(false);
            test(true);
        }

        internal static void RunAllCompressions(Action<VectorCompression> test)
        {
            foreach (var compression in Enum.GetValues(typeof(VectorCompression)).Cast<VectorCompression>())
                test(compression);
        }
    }
}
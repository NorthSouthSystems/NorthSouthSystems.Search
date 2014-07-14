namespace SoftwareBotany.Sunlight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class SafetyVectorCompressionTuple : Tuple<bool, VectorCompression>
    {
        private SafetyVectorCompressionTuple(bool allowUnsafe, VectorCompression compression)
            : base(allowUnsafe, compression)
        { }

        public bool AllowUnsafe { get { return base.Item1; } }
        public VectorCompression Compression { get { return base.Item2; } }

        public static IEnumerable<SafetyVectorCompressionTuple> All
        {
            get
            {
                return from allowUnsafe in new[] { false, true }
                       from VectorCompression compression in Enum.GetValues(typeof(VectorCompression))
                       select new SafetyVectorCompressionTuple(allowUnsafe, compression);
            }
        }

        public static void RunAll(Action<SafetyVectorCompressionTuple> test)
        {
            foreach (var safetyVectorCompression in SafetyVectorCompressionTuple.All)
                test(safetyVectorCompression);
        }

        public static void RunAllSafeties(Action<SafetyVectorCompressionTuple> test, VectorCompression compression)
        {
            test(new SafetyVectorCompressionTuple(false, compression));
            test(new SafetyVectorCompressionTuple(true, compression));
        }
    }
}
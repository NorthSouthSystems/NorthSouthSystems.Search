#if WORDSIZE64
global using WordRawType = ulong;
#else
global using WordRawType = uint;
#endif

#if POSITIONLISTENABLED && WORDSIZE64
namespace NorthSouthSystems.BitVectors.PLWAH64;
#elif POSITIONLISTENABLED
namespace NorthSouthSystems.BitVectors.PLWAH;
#elif WORDSIZE64
namespace NorthSouthSystems.BitVectors.WAH64;
#else
namespace NorthSouthSystems.BitVectors.WAH;
#endif

internal static class WordExtensions
{
    private const uint LARGEPRIME = 9973u;

    internal const WordRawType LARGEPRIME32ORFULLCOVERAGE64
#if WORDSIZE64
        = LARGEPRIME * uint.MaxValue;
#else
        = LARGEPRIME;
#endif
}
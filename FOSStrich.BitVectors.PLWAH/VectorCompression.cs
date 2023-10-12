#if POSITIONLISTENABLED
namespace FOSStrich.BitVectors.PLWAH;
#else
namespace FOSStrich.BitVectors.WAH;
#endif

public enum VectorCompression
{
    None = 0,
    Compressed = 1,
    CompressedWithPackedPosition = 2
}
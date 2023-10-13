#if POSITIONLISTENABLED
namespace FOSStrich.BitVectors.PLWAH;
#else
namespace FOSStrich.BitVectors.WAH;
#endif

#if POSITIONLISTENABLED
public class PLWAHVectorFactory : IBitVectorFactory<Vector>
#else
public class WAHVectorFactory : IBitVectorFactory<Vector>
#endif
{
    public Vector Create(bool isCompressed) =>
        new(isCompressed);

    public Vector Create(bool isCompressed, Vector copy) =>
        new(isCompressed, copy);

    public Vector CreateUncompressedUnion(params Vector[] union) =>
        Vector.OrOutOfPlace(union);

    public int WordSize => Word.SIZE;
}
#if POSITIONLISTENABLED && WORDSIZE64
namespace NorthSouthSystems.BitVectors.PLWAH64;
#elif POSITIONLISTENABLED
namespace NorthSouthSystems.BitVectors.PLWAH;
#elif WORDSIZE64
namespace NorthSouthSystems.BitVectors.WAH64;
#else
namespace NorthSouthSystems.BitVectors.WAH;
#endif

#if POSITIONLISTENABLED && WORDSIZE64
public class PLWAH64VectorFactory : IBitVectorFactory<Vector>
#elif POSITIONLISTENABLED
public class PLWAHVectorFactory : IBitVectorFactory<Vector>
#elif WORDSIZE64
public class WAH64VectorFactory : IBitVectorFactory<Vector>
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
namespace FOSStrich.BitVectors.PLWAH;

public class VectorFactory : IBitVectorFactory<Vector>
{
    public Vector Create(bool isCompressed) =>
        new(isCompressed ? VectorCompression.CompressedWithPackedPosition : VectorCompression.None);

    public Vector Create(bool isCompressed, Vector copy) =>
        new(isCompressed ? VectorCompression.CompressedWithPackedPosition : VectorCompression.None, copy);

    public Vector CreateUncompressedUnion(params Vector[] union) =>
        Vector.OrOutOfPlace(union);
}
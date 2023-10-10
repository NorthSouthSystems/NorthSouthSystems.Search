namespace FOSStrich.Search;

using FOSStrich.BitVectors;

public class VectorFactory : IBitVectorFactory<Vector>
{
    public Vector Create(bool isCompressed) =>
        new(VectorCompression.CompressedWithPackedPosition);

    public Vector Create(bool isCompressed, Vector copy) =>
        new(VectorCompression.CompressedWithPackedPosition, copy);

    public Vector CreateUncompressedUnion(params Vector[] union) =>
        Vector.OrOutOfPlace(union);
}
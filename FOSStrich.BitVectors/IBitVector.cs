namespace FOSStrich.BitVectors;

public interface IBitVector<TBitVector>
    where TBitVector : IBitVector<TBitVector>
{
    void DecompressInPlace(TBitVector vector);

    void AndInPlace(TBitVector vector);
    TBitVector AndOutOfPlace(TBitVector vector, bool resultIsCompressed);
    int AndPopulation(TBitVector vector);
    bool AndPopulationAny(TBitVector vector);

    void OrInPlace(TBitVector vector);
}
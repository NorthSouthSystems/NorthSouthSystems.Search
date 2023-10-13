namespace FOSStrich.Search;

using System.Collections;

public class BitVectorFactories : IEnumerable<object[]>
{
    private static readonly object[] _bitVectorFactories = new object[]
    {
        new FOSStrich.BitVectors.PLWAH.PLWAHVectorFactory(),
        new FOSStrich.BitVectors.WAH.WAHVectorFactory()
    };

    private static readonly List<object[]> BitVectorFactoriesWrapped =
        _bitVectorFactories.Select(bvf => new object[] { bvf }).ToList();

    public IEnumerator<object[]> GetEnumerator() => BitVectorFactoriesWrapped.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => BitVectorFactoriesWrapped.GetEnumerator();
}
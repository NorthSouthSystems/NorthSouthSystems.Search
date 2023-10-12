namespace FOSStrich.Search;

using System.Collections;

public class BitVectorFactories : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator() => BitVectorFactoriesWrapped.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => BitVectorFactoriesWrapped.GetEnumerator();

    private static IEnumerable<object[]> BitVectorFactoriesWrapped =>
        _bitVectorFactories.Select(bvf => new object[] { bvf });

    private static readonly object[] _bitVectorFactories = new object[]
    {
        new FOSStrich.BitVectors.PLWAH.VectorFactory()
    };
}
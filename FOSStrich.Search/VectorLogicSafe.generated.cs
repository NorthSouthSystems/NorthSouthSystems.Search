namespace FOSStrich.Search;

internal sealed partial class VectorLogicSafe : IVectorLogic
{
    #region Decompress

    void IVectorLogic.DecompressInPlaceNoneCompressed(Word[] iWords, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int i = 0;
        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (j < jWords.Length)
        {
            Word jWord = jWords[j];

            if (jWord.IsCompressed)
            {
                if (jWord.FillBit)
                {
                    int k = i + jWord.FillCount;

                    while (i < k)
                    {
                        iWords[i].Raw = 0x7FFFFFFF;
                        i++;
                    }
                }
                else
                    i += jWord.FillCount;
            }
            else
            {
                iWords[i].Raw = jWord.Raw;
                i++;
            }

            j++;
        }
    }

    void IVectorLogic.DecompressInPlaceNoneCompressedWithPackedPosition(Word[] iWords, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int i = 0;
        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (j < jWords.Length)
        {
            Word jWord = jWords[j];

            if (jWord.IsCompressed)
            {
                if (jWord.FillBit)
                {
                    int k = i + jWord.FillCount;

                    while (i < k)
                    {
                        iWords[i].Raw = 0x7FFFFFFF;
                        i++;
                    }
                }
                else
                    i += jWord.FillCount;

                if (jWord.HasPackedWord)
                {
                    iWords[i].Raw = jWord.PackedWord.Raw;
                    i++;
                }
            }
            else
            {
                iWords[i].Raw = jWord.Raw;
                i++;
            }

            j++;
        }
    }

    #endregion
}

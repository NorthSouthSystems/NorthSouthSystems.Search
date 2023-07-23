namespace FOSStrich.Search;

internal sealed partial class VectorLogicUnsafe : IVectorLogic
{
    #region Decompress

    unsafe void IVectorLogic.DecompressInPlaceNoneCompressed(Word[] iWords, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (j < jMax)
            {
                if (j->IsCompressed)
                {
                    if (j->FillBit)
                    {
                        Word* k = i + j->FillCount;

                        while (i < k)
                        {
                            i->Raw = 0x7FFFFFFF;
                            i++;
                        }
                    }
                    else
                        i += j->FillCount;
                }
                else
                {
                    i->Raw = j->Raw;
                    i++;
                }

                j++;
            }
        }
    }

    unsafe void IVectorLogic.DecompressInPlaceNoneCompressedWithPackedPosition(Word[] iWords, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (j < jMax)
            {
                if (j->IsCompressed)
                {
                    if (j->FillBit)
                    {
                        Word* k = i + j->FillCount;

                        while (i < k)
                        {
                            i->Raw = 0x7FFFFFFF;
                            i++;
                        }
                    }
                    else
                        i += j->FillCount;

                    if (j->HasPackedWord)
                    {
                        i->Raw = j->PackedWord.Raw;
                        i++;
                    }
                }
                else
                {
                    i->Raw = j->Raw;
                    i++;
                }

                j++;
            }
        }
    }

    #endregion
}

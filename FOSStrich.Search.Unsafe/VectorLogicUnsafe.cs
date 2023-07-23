namespace FOSStrich.Search;

internal sealed partial class VectorLogicUnsafe : IVectorLogic
{
    #region And In-Place

    unsafe void IVectorLogic.AndInPlaceNoneNone(Word[] iWords, ref int iWordCountPhysical, ref int iWordCountLogical, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                i->Raw &= j->Raw;
                i++;
                j++;
            }

            if (i < iMax)
            {
                iWordCountPhysical = (int)(i - iFixed);
                iWordCountLogical = (int)(i - iFixed);

                while (i < iMax)
                {
                    i->Raw = 0;
                    i++;
                }
            }
        }
    }

    unsafe void IVectorLogic.AndInPlaceNoneCompressedWithPackedPosition(Word[] iWords, ref int iWordCountPhysical, ref int iWordCountLogical, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                if (j->IsCompressed)
                {
                    if (!j->FillBit)
                    {
                        Word* k = i + j->FillCount;

                        if (k >= iMax)
                            break;

                        while (i < k)
                        {
                            i->Raw = 0;
                            i++;
                        }
                    }
                    else
                        i += j->FillCount;

                    if (j->HasPackedWord && i < iMax)
                    {
                        i->Raw &= j->PackedWord.Raw;
                        i++;
                    }
                }
                else
                {
                    i->Raw &= j->Raw;
                    i++;
                }

                j++;
            }

            if (i < iMax)
            {
                iWordCountPhysical = (int)(i - iFixed);
                iWordCountLogical = (int)(i - iFixed);

                while (i < iMax)
                {
                    i->Raw = 0;
                    i++;
                }
            }
        }
    }

    #endregion

    #region And Out-of-Place

    unsafe Vector IVectorLogic.AndOutOfPlaceNoneNone(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical, VectorCompression resultCompression)
    {
        Vector result = new Vector(true, resultCompression);

        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                uint word = i->Raw & j->Raw;

                if (word > 0)
                    result.SetWord((int)(i - iFixed), new Word(word));

                i++;
                j++;
            }
        }

        return result;
    }

    unsafe Vector IVectorLogic.AndOutOfPlaceNoneCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical, VectorCompression resultCompression)
    {
        Vector result = new Vector(true, resultCompression);

        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                if (j->IsCompressed)
                {
                    if (j->FillBit)
                    {
                        Word* k = i + j->FillCount;

                        if (k > iMax)
                            k = iMax;

                        while (i < k)
                        {
                            result.SetWord((int)(i - iFixed), *i);
                            i++;
                        }
                    }
                    else
                        i += j->FillCount;

                    if (j->HasPackedWord && i < iMax)
                    {
                        uint word = i->Raw & j->PackedWord.Raw;

                        if (word > 0)
                            result.SetWord((int)(i - iFixed), new Word(word));

                        i++;
                    }
                }
                else
                {
                    uint word = i->Raw & j->Raw;

                    if (word > 0)
                        result.SetWord((int)(i - iFixed), new Word(word));

                    i++;
                }

                j++;
            }
        }

        return result;
    }

    unsafe Vector IVectorLogic.AndOutOfPlaceCompressedWithPackedPositionCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical, VectorCompression resultCompression)
    {
        Vector result = new Vector(true, resultCompression);

        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;
            int iLogical = 0;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;
            int jLogical = 0;
            bool jUsePackedWord = false;

            while (i < iMax)
            {
                if (i->IsCompressed)
                {
                    if (i->FillBit)
                    {
                        while (jLogical < iLogical + i->FillCount)
                        {
                            if (jUsePackedWord || !j->IsCompressed)
                            {
                                if (jLogical >= iLogical)
                                    result.SetWord(jLogical, jUsePackedWord ? j->PackedWord : *j);

                                jLogical++;
                            }
                            else
                            {
                                if (j->FillBit)
                                {
                                    int logical = Math.Max(iLogical, jLogical);
                                    int fillCount = Math.Min(iLogical + i->FillCount, jLogical + j->FillCount) - logical;

                                    result.SetWord(logical, new Word(true, fillCount));
                                }

                                if (jLogical + j->FillCount <= iLogical + i->FillCount)
                                {
                                    jLogical += j->FillCount;

                                    if (j->HasPackedWord)
                                    {
                                        jUsePackedWord = true;
                                        continue;
                                    }
                                }
                                else
                                    break;
                            }

                            if (++j >= jMax)
                                return result;

                            jUsePackedWord = false;
                        }
                    }

                    iLogical += i->FillCount;

                    if (i->HasPackedWord)
                    {
                        while (jLogical <= iLogical)
                        {
                            if (jUsePackedWord || !j->IsCompressed)
                            {
                                if (jLogical == iLogical)
                                    if ((i->PackedWord.Raw & (jUsePackedWord ? j->PackedWord.Raw : j->Raw)) > 0)
                                        result.SetWord(iLogical, i->PackedWord);

                                jLogical++;
                            }
                            else
                            {
                                if (j->FillBit && (jLogical + j->FillCount) > iLogical)
                                    result.SetWord(iLogical, i->PackedWord);

                                if (jLogical + j->FillCount <= iLogical + 1)
                                {
                                    jLogical += j->FillCount;

                                    if (j->HasPackedWord)
                                    {
                                        jUsePackedWord = true;
                                        continue;
                                    }
                                }
                                else
                                    break;
                            }

                            if (++j >= jMax)
                                return result;

                            jUsePackedWord = false;
                        }

                        iLogical++;
                    }
                }
                else
                {
                    while (jLogical <= iLogical)
                    {
                        if (jUsePackedWord || !j->IsCompressed)
                        {
                            if (jLogical == iLogical)
                            {
                                uint word = i->Raw & (jUsePackedWord ? j->PackedWord.Raw : j->Raw);

                                if (word > 0)
                                    result.SetWord(iLogical, new Word(word));
                            }

                            jLogical++;
                        }
                        else
                        {
                            if (j->FillBit && jLogical + j->FillCount > iLogical)
                                result.SetWord(iLogical, *i);

                            if (jLogical + j->FillCount <= iLogical + 1)
                            {
                                jLogical += j->FillCount;

                                if (j->HasPackedWord)
                                {
                                    jUsePackedWord = true;
                                    continue;
                                }
                            }
                            else
                                break;
                        }

                        if (++j >= jMax)
                            return result;

                        jUsePackedWord = false;
                    }

                    iLogical++;
                }

                i++;
            }
        }

        return result;
    }

    #endregion

    #region AndPopulation

    unsafe int IVectorLogic.AndPopulationNoneNone(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            int population = 0;

            while (i < iMax && j < jMax)
            {
                uint word = i->Raw & j->Raw;

                if (word > 0)
                    population += word.Population();

                i++;
                j++;
            }

            return population;
        }
    }

    unsafe int IVectorLogic.AndPopulationNoneCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            int population = 0;

            while (i < iMax && j < jMax)
            {
                if (j->IsCompressed)
                {
                    if (j->FillBit)
                    {
                        Word* k = i + j->FillCount;

                        if (k > iMax)
                            k = iMax;

                        while (i < k)
                        {
                            population += i->Population;
                            i++;
                        }
                    }
                    else
                        i += j->FillCount;

                    if (j->HasPackedWord && i < iMax)
                    {
                        if (i->Raw > 0 && (i->Raw & j->PackedWord.Raw) > 0)
                            population++;

                        i++;
                    }
                }
                else
                {
                    uint word = i->Raw & j->Raw;

                    if (word > 0)
                        population += word.Population();

                    i++;
                }

                j++;
            }

            return population;
        }
    }

    #endregion

    #region AndPopulationAny

    unsafe bool IVectorLogic.AndPopulationAnyNoneNone(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                uint word = i->Raw & j->Raw;

                if (word > 0)
                    return true;

                i++;
                j++;
            }

            return false;
        }
    }

    unsafe bool IVectorLogic.AndPopulationAnyNoneCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                if (j->IsCompressed)
                {
                    if (j->FillBit)
                    {
                        Word* k = i + j->FillCount;

                        if (k > iMax)
                            k = iMax;

                        while (i < k)
                        {
                            if (i->Raw > 0)
                                return true;

                            i++;
                        }
                    }
                    else
                        i += j->FillCount;

                    if (j->HasPackedWord && i < iMax)
                    {
                        if (i->Raw > 0 && (i->Raw & j->PackedWord.Raw) > 0)
                            return true;

                        i++;
                    }
                }
                else
                {
                    uint word = i->Raw & j->Raw;

                    if (word > 0)
                        return true;

                    i++;
                }

                j++;
            }

            return false;
        }
    }

    #endregion

    #region Or In-Place

    unsafe void IVectorLogic.OrInPlaceNoneNone(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                i->Raw |= j->Raw;
                i++;
                j++;
            }
        }
    }

    unsafe void IVectorLogic.OrInPlaceNoneCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
    {
        fixed (Word* iFixed = iWords, jFixed = jWords)
        {
            Word* i = iFixed;
            Word* iMax = iFixed + iWordCountPhysical;

            Word* j = jFixed;
            Word* jMax = jFixed + jWordCountPhysical;

            while (i < iMax && j < jMax)
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

                    if (j->HasPackedWord && i < iMax)
                    {
                        i->Raw |= j->PackedWord.Raw;
                        i++;
                    }
                }
                else
                {
                    i->Raw |= j->Raw;
                    i++;
                }

                j++;
            }
        }
    }

    #endregion
}
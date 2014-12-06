namespace SoftwareBotany.Sunlight
{
    using System;

    internal sealed class VectorLogicSafe : IVectorLogic
    {
        #region Construction

        void IVectorLogic.DecompressInPlaceNoneCompressedWithPackedPosition(Word[] iWords, Word[] jWords, int jWordCountPhysical)
        {
            int i = 0;
            int j = 0;
            int jMax = jWordCountPhysical;

            while (j < jMax)
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

        #region And In-Place

        void IVectorLogic.AndInPlaceNoneNone(Word[] iWords, ref int iWordCountPhysical, ref int iWordCountLogical, Word[] jWords, int jWordCountPhysical)
        {
            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                iWords[i].Raw &= jWords[j].Raw;
                i++;
                j++;
            }

            if (i < iMax)
            {
                iWordCountPhysical = i;
                iWordCountLogical = i;

                while (i < iMax)
                {
                    iWords[i].Raw = 0;
                    i++;
                }
            }
        }

        void IVectorLogic.AndInPlaceNoneCompressedWithPackedPosition(Word[] iWords, ref int iWordCountPhysical, ref int iWordCountLogical, Word[] jWords, int jWordCountPhysical)
        {
            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                Word jWord = jWords[j];

                if (jWord.IsCompressed)
                {
                    if (!jWord.FillBit)
                    {
                        int k = i + jWord.FillCount;

                        if (k >= iMax)
                            break;

                        while (i < k)
                        {
                            iWords[i].Raw = 0;
                            i++;
                        }
                    }
                    else
                        i += jWord.FillCount;

                    if (jWord.HasPackedWord && i < iMax)
                    {
                        iWords[i].Raw &= jWord.PackedWord.Raw;
                        i++;
                    }
                }
                else
                {
                    iWords[i].Raw &= jWord.Raw;
                    i++;
                }

                j++;
            }

            if (i < iMax)
            {
                iWordCountPhysical = i;
                iWordCountLogical = i;

                while (i < iMax)
                {
                    iWords[i].Raw = 0;
                    i++;
                }
            }
        }

        #endregion

        #region And Out-of-Place

        Vector IVectorLogic.AndOutOfPlaceNoneNone(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical, VectorCompression resultCompression)
        {
            Vector result = new Vector(false, resultCompression);

            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                uint word = iWords[i].Raw & jWords[j].Raw;

                if (word > 0)
                    result.SetWord(i, new Word(word));

                i++;
                j++;
            }

            return result;
        }

        Vector IVectorLogic.AndOutOfPlaceNoneCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical, VectorCompression resultCompression)
        {
            Vector result = new Vector(false, resultCompression);

            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                Word jWord = jWords[j];

                if (jWord.IsCompressed)
                {
                    if (jWord.FillBit)
                    {
                        int k = Math.Min(i + jWord.FillCount, iMax);

                        while (i < k)
                        {
                            result.SetWord(i, iWords[i]);
                            i++;
                        }
                    }
                    else
                        i += jWord.FillCount;

                    if (jWord.HasPackedWord && i < iMax)
                    {
                        uint word = iWords[i].Raw & jWord.PackedWord.Raw;

                        if (word > 0)
                            result.SetWord(i, new Word(word));

                        i++;
                    }
                }
                else
                {
                    uint word = iWords[i].Raw & jWord.Raw;

                    if (word > 0)
                        result.SetWord(i, new Word(word));

                    i++;
                }

                j++;
            }

            return result;
        }

        Vector IVectorLogic.AndOutOfPlaceCompressedWithPackedPositionCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical, VectorCompression resultCompression)
        {
            Vector result = new Vector(false, resultCompression);

            int i = 0;
            int iMax = iWordCountPhysical;
            int iLogical = 0;

            int j = 0;
            int jMax = jWordCountPhysical;
            int jLogical = 0;

            Word jWord = jWords[j];
            bool jUsePackedWord = false;

            while (i < iMax)
            {
                Word iWord = iWords[i];

                if (iWord.IsCompressed)
                {
                    if (iWord.FillBit)
                    {
                        while (jLogical < iLogical + iWord.FillCount)
                        {
                            if (jUsePackedWord || !jWord.IsCompressed)
                            {
                                if (jLogical >= iLogical)
                                    result.SetWord(jLogical, jUsePackedWord ? jWord.PackedWord : jWord);

                                jLogical++;
                            }
                            else
                            {
                                if (jWord.FillBit)
                                {
                                    int logical = Math.Max(iLogical, jLogical);
                                    int fillCount = Math.Min(iLogical + iWord.FillCount, jLogical + jWord.FillCount) - logical;

                                    result.SetWord(logical, new Word(true, fillCount));
                                }

                                if (jLogical + jWord.FillCount <= iLogical + iWord.FillCount)
                                {
                                    jLogical += jWord.FillCount;

                                    if (jWord.HasPackedWord)
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

                            jWord = jWords[j];
                            jUsePackedWord = false;
                        }
                    }

                    iLogical += iWord.FillCount;

                    if (iWord.HasPackedWord)
                    {
                        while (jLogical <= iLogical)
                        {
                            if (jUsePackedWord || !jWord.IsCompressed)
                            {
                                if (jLogical == iLogical)
                                    if ((iWord.PackedWord.Raw & (jUsePackedWord ? jWord.PackedWord.Raw : jWord.Raw)) > 0)
                                        result.SetWord(iLogical, iWord.PackedWord);

                                jLogical++;
                            }
                            else
                            {
                                if (jWord.FillBit && (jLogical + jWord.FillCount) > iLogical)
                                    result.SetWord(iLogical, iWord.PackedWord);

                                if (jLogical + jWord.FillCount <= iLogical + 1)
                                {
                                    jLogical += jWord.FillCount;

                                    if (jWord.HasPackedWord)
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

                            jWord = jWords[j];
                            jUsePackedWord = false;
                        }

                        iLogical++;
                    }
                }
                else
                {
                    while (jLogical <= iLogical)
                    {
                        if (jUsePackedWord || !jWord.IsCompressed)
                        {
                            if (jLogical == iLogical)
                            {
                                uint word = iWord.Raw & (jUsePackedWord ? jWord.PackedWord.Raw : jWord.Raw);

                                if (word > 0)
                                    result.SetWord(iLogical, new Word(word));
                            }

                            jLogical++;
                        }
                        else
                        {
                            if (jWord.FillBit && jLogical + jWord.FillCount > iLogical)
                                result.SetWord(iLogical, iWord);

                            if (jLogical + jWord.FillCount <= iLogical + 1)
                            {
                                jLogical += jWord.FillCount;

                                if (jWord.HasPackedWord)
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

                        jWord = jWords[j];
                        jUsePackedWord = false;
                    }

                    iLogical++;
                }

                i++;
            }

            return result;
        }

        #endregion

        #region AndPopulation

        int IVectorLogic.AndPopulationNoneNone(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
        {
            int population = 0;

            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                uint word = iWords[i].Raw & jWords[j].Raw;

                if (word > 0)
                    population += word.Population();

                i++;
                j++;
            }

            return population;
        }

        int IVectorLogic.AndPopulationNoneCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
        {
            int population = 0;

            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                Word jWord = jWords[j];

                if (jWord.IsCompressed)
                {
                    if (jWord.FillBit)
                    {
                        int k = i + jWord.FillCount;

                        if (k > iMax)
                            k = iMax;

                        while (i < k)
                        {
                            population += iWords[i].Population;
                            i++;
                        }
                    }
                    else
                        i += jWord.FillCount;

                    if (jWord.HasPackedWord && i < iMax)
                    {
                        Word iWord = iWords[i];

                        if (iWord.Raw > 0 && (iWord.Raw & jWord.PackedWord.Raw) > 0)
                            population++;

                        i++;
                    }
                }
                else
                {
                    uint word = iWords[i].Raw & jWord.Raw;

                    if (word > 0)
                        population += word.Population();

                    i++;
                }

                j++;
            }

            return population;
        }

        #endregion

        #region AndPopulationAny

        bool IVectorLogic.AndPopulationAnyNoneNone(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
        {
            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                uint word = iWords[i].Raw & jWords[j].Raw;

                if (word > 0)
                    return true;

                i++;
                j++;
            }

            return false;
        }

        bool IVectorLogic.AndPopulationAnyNoneCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
        {
            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                Word jWord = jWords[j];

                if (jWord.IsCompressed)
                {
                    if (jWord.FillBit)
                    {
                        int k = i + jWord.FillCount;

                        if (k > iMax)
                            k = iMax;

                        while (i < k)
                        {
                            if (iWords[i].Raw > 0)
                                return true;

                            i++;
                        }
                    }
                    else
                        i += jWord.FillCount;

                    if (jWord.HasPackedWord && i < iMax)
                    {
                        Word iWord = iWords[i];

                        if (iWord.Raw > 0 && (iWord.Raw & jWord.PackedWord.Raw) > 0)
                            return true;

                        i++;
                    }
                }
                else
                {
                    uint word = iWords[i].Raw & jWord.Raw;

                    if (word > 0)
                        return true;

                    i++;
                }

                j++;
            }

            return false;
        }

        #endregion

        #region Or In-Place

        void IVectorLogic.OrInPlaceNoneNone(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
        {
            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
            {
                iWords[i].Raw |= jWords[j].Raw;
                i++;
                j++;
            }
        }

        void IVectorLogic.OrInPlaceNoneCompressedWithPackedPosition(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
        {
            int i = 0;
            int iMax = iWordCountPhysical;

            int j = 0;
            int jMax = jWordCountPhysical;

            while (i < iMax && j < jMax)
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

                    if (jWord.HasPackedWord && i < iMax)
                    {
                        iWords[i].Raw |= jWord.PackedWord.Raw;
                        i++;
                    }
                }
                else
                {
                    iWords[i].Raw |= jWord.Raw;
                    i++;
                }

                j++;
            }
        }

        #endregion
    }
}
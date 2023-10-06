namespace FOSStrich.Search;

internal sealed partial class VectorLogicSafe : IVectorLogic
{
    #region And In-Place

    void IVectorLogic.AndInPlaceNoneNone(Word[] iWordsBuffer, ref int iWordCountPhysical, ref int iWordCountLogical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            iWords[i].Raw &= jWords[j].Raw;
            i++;
            j++;
        }

        if (i < iWords.Length)
        {
            iWordCountPhysical = i;
            iWordCountLogical = i;

            while (i < iWords.Length)
            {
                iWords[i].Raw = 0;
                i++;
            }
        }
    }

    void IVectorLogic.AndInPlaceNoneCompressedWithPackedPosition(Word[] iWordsBuffer, ref int iWordCountPhysical, ref int iWordCountLogical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            Word jWord = jWords[j];

            if (jWord.IsCompressed)
            {
                if (!jWord.FillBit)
                {
                    int k = i + jWord.FillCount;

                    if (k >= iWords.Length)
                        break;

                    while (i < k)
                    {
                        iWords[i].Raw = 0;
                        i++;
                    }
                }
                else
                    i += jWord.FillCount;

                if (jWord.HasPackedWord && i < iWords.Length)
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

        if (i < iWords.Length)
        {
            iWordCountPhysical = i;
            iWordCountLogical = i;

            while (i < iWords.Length)
            {
                iWords[i].Raw = 0;
                i++;
            }
        }
    }

    #endregion

    #region And Out-of-Place

    Vector IVectorLogic.AndOutOfPlaceNoneNone(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical, VectorCompression resultCompression)
    {
        Vector result = new Vector(resultCompression);

        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            uint word = iWords[i].Raw & jWords[j].Raw;

            if (word > 0)
                result.SetWord(i, new Word(word));

            i++;
            j++;
        }

        return result;
    }

    Vector IVectorLogic.AndOutOfPlaceNoneCompressedWithPackedPosition(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical, VectorCompression resultCompression)
    {
        Vector result = new Vector(resultCompression);

        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            Word jWord = jWords[j];

            if (jWord.IsCompressed)
            {
                if (jWord.FillBit)
                {
                    int k = Math.Min(i + jWord.FillCount, iWords.Length);

                    while (i < k)
                    {
                        result.SetWord(i, iWords[i]);
                        i++;
                    }
                }
                else
                    i += jWord.FillCount;

                if (jWord.HasPackedWord && i < iWords.Length)
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

    Vector IVectorLogic.AndOutOfPlaceCompressedWithPackedPositionCompressedWithPackedPosition(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical, VectorCompression resultCompression)
    {
        Vector result = new Vector(resultCompression);

        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);
        int iLogical = 0;

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);
        int jLogical = 0;

        Word jWord = jWords[j];
        bool jUsePackedWord = false;

        while (i < iWords.Length)
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

                        if (++j >= jWords.Length)
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

                        if (++j >= jWords.Length)
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

                    if (++j >= jWords.Length)
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

    int IVectorLogic.AndPopulationNoneNone(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int population = 0;

        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            uint word = iWords[i].Raw & jWords[j].Raw;

            if (word > 0)
                population += word.Population();

            i++;
            j++;
        }

        return population;
    }

    int IVectorLogic.AndPopulationNoneCompressedWithPackedPosition(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int population = 0;

        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            Word jWord = jWords[j];

            if (jWord.IsCompressed)
            {
                if (jWord.FillBit)
                {
                    int k = i + jWord.FillCount;

                    if (k > iWords.Length)
                        k = iWords.Length;

                    while (i < k)
                    {
                        population += iWords[i].Population;
                        i++;
                    }
                }
                else
                    i += jWord.FillCount;

                if (jWord.HasPackedWord && i < iWords.Length)
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

    int IVectorLogic.AndPopulationCompressedWithPackedPositionCompressedWithPackedPosition(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int population = 0;

        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);
        int iLogical = 0;

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);
        int jLogical = 0;

        Word jWord = jWords[j];
        bool jUsePackedWord = false;

        while (i < iWords.Length)
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
                                population += (jUsePackedWord ? jWord.PackedWord : jWord).Population;

                            jLogical++;
                        }
                        else
                        {
                            if (jWord.FillBit)
                            {
                                int logical = Math.Max(iLogical, jLogical);
                                int fillCount = Math.Min(iLogical + iWord.FillCount, jLogical + jWord.FillCount) - logical;

                                population += fillCount * (Word.SIZE - 1);
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

                        if (++j >= jWords.Length)
                            return population;

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
                                    population++;

                            jLogical++;
                        }
                        else
                        {
                            if (jWord.FillBit && (jLogical + jWord.FillCount) > iLogical)
                                population++;

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

                        if (++j >= jWords.Length)
                            return population;

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
                                population += word.Population();
                        }

                        jLogical++;
                    }
                    else
                    {
                        if (jWord.FillBit && jLogical + jWord.FillCount > iLogical)
                            population += iWord.Population;

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

                    if (++j >= jWords.Length)
                        return population;

                    jWord = jWords[j];
                    jUsePackedWord = false;
                }

                iLogical++;
            }

            i++;
        }

        return population;
    }

    #endregion

    #region AndPopulationAny

    bool IVectorLogic.AndPopulationAnyNoneNone(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            uint word = iWords[i].Raw & jWords[j].Raw;

            if (word > 0)
                return true;

            i++;
            j++;
        }

        return false;
    }

    bool IVectorLogic.AndPopulationAnyNoneCompressedWithPackedPosition(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            Word jWord = jWords[j];

            if (jWord.IsCompressed)
            {
                if (jWord.FillBit)
                {
                    int k = i + jWord.FillCount;

                    if (k > iWords.Length)
                        k = iWords.Length;

                    while (i < k)
                    {
                        if (iWords[i].Raw > 0)
                            return true;

                        i++;
                    }
                }
                else
                    i += jWord.FillCount;

                if (jWord.HasPackedWord && i < iWords.Length)
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

    void IVectorLogic.OrInPlaceNoneNone(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
        {
            iWords[i].Raw |= jWords[j].Raw;
            i++;
            j++;
        }
    }

    void IVectorLogic.OrInPlaceNoneCompressedWithPackedPosition(Word[] iWordsBuffer, int iWordCountPhysical, Word[] jWordsBuffer, int jWordCountPhysical)
    {
        int i = 0;
        var iWords = new Span<Word>(iWordsBuffer, 0, iWordCountPhysical);

        int j = 0;
        var jWords = new Span<Word>(jWordsBuffer, 0, jWordCountPhysical);

        while (i < iWords.Length && j < jWords.Length)
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

                if (jWord.HasPackedWord && i < iWords.Length)
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
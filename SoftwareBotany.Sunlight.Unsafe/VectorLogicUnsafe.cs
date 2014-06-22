namespace SoftwareBotany.Sunlight
{
    internal sealed class VectorLogicUnsafe : IVectorLogic
    {
        #region Construction

        unsafe void IVectorLogic.Decompress(Word[] iWords, Word[] jWords, int jWordCountPhysical)
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

        #region And

        unsafe void IVectorLogic.And(Word[] iWords, ref int iWordCountPhysical, ref int iWordCountLogical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical)
        {
            fixed (Word* iFixed = iWords, jFixed = jWords)
            {
                Word* i = iFixed;
                Word* iMax = iFixed + iWordCountPhysical;

                Word* j = jFixed;
                Word* jMax = jFixed + jWordCountPhysical;

                i = jIsCompressed
                    ? AndCompressed(i, iMax, j, jMax)
                    : AndUncompressed(i, iMax, j, jMax);

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

        private unsafe static Word* AndCompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
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

            return i;
        }

        private unsafe static Word* AndUncompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
            while (i < iMax && j < jMax)
            {
                i->Raw &= j->Raw;
                i++;
                j++;
            }

            return i;
        }

        #endregion

        #region AndPopulation

        unsafe int IVectorLogic.AndPopulation(Word[] iWords, int iWordCountPhysical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical)
        {
            fixed (Word* iFixed = iWords, jFixed = jWords)
            {
                Word* i = iFixed;
                Word* iMax = iFixed + iWordCountPhysical;

                Word* j = jFixed;
                Word* jMax = jFixed + jWordCountPhysical;

                return jIsCompressed
                    ? AndPopulationCompressed(i, iMax, j, jMax)
                    : AndPopulationUncompressed(i, iMax, j, jMax);
            }
        }

        private unsafe static int AndPopulationCompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
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

        private unsafe static int AndPopulationUncompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
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

        #endregion

        #region AndPopulationAny

        unsafe bool IVectorLogic.AndPopulationAny(Word[] iWords, int iWordCountPhysical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical)
        {
            fixed (Word* iFixed = iWords, jFixed = jWords)
            {
                Word* i = iFixed;
                Word* iMax = iFixed + iWordCountPhysical;

                Word* j = jFixed;
                Word* jMax = jFixed + jWordCountPhysical;

                return jIsCompressed
                    ? AndPopulationAnyCompressed(i, iMax, j, jMax)
                    : AndPopulationAnyUncompressed(i, iMax, j, jMax);
            }
        }

        private unsafe static bool AndPopulationAnyCompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
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

        private unsafe static bool AndPopulationAnyUncompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
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

        #endregion

        #region Or

        unsafe void IVectorLogic.Or(Word[] iWords, int iWordCountPhysical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical)
        {
            fixed (Word* iFixed = iWords, jFixed = jWords)
            {
                Word* i = iFixed;
                Word* iMax = iFixed + iWordCountPhysical;

                Word* j = jFixed;
                Word* jMax = jFixed + jWordCountPhysical;

                if (jIsCompressed)
                    OrCompressed(i, iMax, j, jMax);
                else
                    OrUncompressed(i, iMax, j, jMax);
            }
        }

        private unsafe static void OrCompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
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

        private unsafe static void OrUncompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
            while (i < iMax && j < jMax)
            {
                i->Raw |= j->Raw;
                i++;
                j++;
            }
        }

        #endregion
    }
}
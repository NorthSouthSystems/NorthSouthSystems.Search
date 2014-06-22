namespace SoftwareBotany.Sunlight
{
    internal sealed class VectorLogicSafe : IVectorLogic
    {
        #region Construction

        void IVectorLogic.Decompress(Word[] iWords, Word[] jWords, int jWordCountPhysical)
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

        #region And

        void IVectorLogic.And(Word[] iWords, ref int iWordCountPhysical, ref int iWordCountLogical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical)
        {
            int i = jIsCompressed
                ? AndCompressed(iWords, iWordCountPhysical, jWords, jWordCountPhysical)
                : AndUncompressed(iWords, iWordCountPhysical, jWords, jWordCountPhysical);

            int iMax = iWordCountPhysical;

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

        private static int AndCompressed(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
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

            return i;
        }

        private static int AndUncompressed(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
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

            return i;
        }

        #endregion

        #region AndPopulation

        int IVectorLogic.AndPopulation(Word[] iWords, int iWordCountPhysical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical)
        {
            return jIsCompressed
                ? AndPopulationCompressed(iWords, iWordCountPhysical, jWords, jWordCountPhysical)
                : AndPopulationUncompressed(iWords, iWordCountPhysical, jWords, jWordCountPhysical);
        }

        private static int AndPopulationCompressed(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
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

        private static int AndPopulationUncompressed(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
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

        #endregion

        #region AndPopulationAny

        bool IVectorLogic.AndPopulationAny(Word[] iWords, int iWordCountPhysical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical)
        {
            return jIsCompressed
                ? AndPopulationAnyCompressed(iWords, iWordCountPhysical, jWords, jWordCountPhysical)
                : AndPopulationAnyUncompressed(iWords, iWordCountPhysical, jWords, jWordCountPhysical);
        }

        private static bool AndPopulationAnyCompressed(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
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

        private static bool AndPopulationAnyUncompressed(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
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

        #endregion

        #region Or

        void IVectorLogic.Or(Word[] iWords, int iWordCountPhysical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical)
        {
            if (jIsCompressed)
                OrCompressed(iWords, iWordCountPhysical, jWords, jWordCountPhysical);
            else
                OrUncompressed(iWords, iWordCountPhysical, jWords, jWordCountPhysical);
        }

        private static void OrCompressed(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
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

        private static void OrUncompressed(Word[] iWords, int iWordCountPhysical, Word[] jWords, int jWordCountPhysical)
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

        #endregion
    }
}
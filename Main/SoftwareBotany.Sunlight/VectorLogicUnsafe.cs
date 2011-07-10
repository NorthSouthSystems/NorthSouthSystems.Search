#if UNSAFE
namespace SoftwareBotany.Sunlight
{
    public sealed partial class Vector
    {
        #region Construction

        private unsafe void Decompress(Vector vector)
        {
            fixed (Word* iFixed = _words, jFixed = vector._words)
            {
                Word* i = iFixed;
                Word* j = jFixed;
                Word* jMax = jFixed + vector._wordCountPhysical;

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

#if POSITIONLIST
                        if (j->HasPackedWord)
                        {
                            i->Raw = j->PackedWord.Raw;
                            i++;
                        }
#endif
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

#if POSITIONLIST
                    if (j->HasPackedWord && i < iMax)
                    {
                        i->Raw &= j->PackedWord.Raw;
                        i++;
                    }
#endif
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

#if POSITIONLIST
                    if (j->HasPackedWord && i < iMax)
                    {
                        if (i->Raw > 0 && (i->Raw & j->PackedWord.Raw) > 0)
                            population++;

                        i++;
                    }
#endif
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

        #region Or

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

#if POSITIONLIST
                    if (j->HasPackedWord && i < iMax)
                    {
                        i->Raw |= j->PackedWord.Raw;
                        i++;
                    }
#endif
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
#endif
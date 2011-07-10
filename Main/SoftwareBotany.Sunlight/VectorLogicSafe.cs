#if !UNSAFE
namespace SoftwareBotany.Sunlight
{
    public sealed partial class Vector
    {
        #region Construction

        private void Decompress(Vector vector)
        {
            int i = 0;
            int j = 0;
            int jMax = vector._wordCountPhysical;

            while (j < jMax)
            {
                Word jWord = vector._words[j];

                if (jWord.IsCompressed)
                {
                    if (jWord.FillBit)
                    {
                        int k = i + jWord.FillCount;

                        while (i < k)
                        {
                            _words[i].Raw = 0x7FFFFFFF;
                            i++;
                        }
                    }
                    else
                        i += jWord.FillCount;

#if POSITIONLIST
                    if (jWord.HasPackedWord)
                    {
                        _words[i].Raw = jWord.PackedWord.Raw;
                        i++;
                    }
#endif
                }
                else
                {
                    _words[i].Raw = jWord.Raw;
                    i++;
                }

                j++;
            }
        }

        #endregion

        #region And

        private int AndCompressed(Vector vector)
        {
            int i = 0;
            int iMax = _wordCountPhysical;

            int j = 0;
            int jMax = vector._wordCountPhysical;

            while (i < iMax && j < jMax)
            {
                Word jWord = vector._words[j];

                if (jWord.IsCompressed)
                {
                    if (!jWord.FillBit)
                    {
                        int k = i + jWord.FillCount;

                        if (k >= iMax)
                            break;

                        while (i < k)
                        {
                            _words[i].Raw = 0;
                            i++;
                        }
                    }
                    else
                        i += jWord.FillCount;

#if POSITIONLIST
                    if (jWord.HasPackedWord && i < iMax)
                    {
                        _words[i].Raw &= jWord.PackedWord.Raw;
                        i++;
                    }
#endif
                }
                else
                {
                    _words[i].Raw &= jWord.Raw;
                    i++;
                }

                j++;
            }

            return i;
        }

        private int AndUncompressed(Vector vector)
        {
            int i = 0;
            int iMax = _wordCountPhysical;

            int j = 0;
            int jMax = vector._wordCountPhysical;

            while (i < iMax && j < jMax)
            {
                _words[i].Raw &= vector._words[j].Raw;
                i++;
                j++;
            }

            return i;
        }

        #endregion

        #region AndPopulation

        private int AndPopulationCompressed(Vector vector)
        {
            int population = 0;

            int i = 0;
            int iMax = _wordCountPhysical;

            int j = 0;
            int jMax = vector._wordCountPhysical;

            while (i < iMax && j < jMax)
            {
                Word jWord = vector._words[j];

                if (jWord.IsCompressed)
                {
                    if (jWord.FillBit)
                    {
                        int k = i + jWord.FillCount;

                        if (k > iMax)
                            k = iMax;

                        while (i < k)
                        {
                            population += _words[i].Population;
                            i++;
                        }
                    }
                    else
                        i += jWord.FillCount;

#if POSITIONLIST
                    if (jWord.HasPackedWord && i < iMax)
                    {
                        if (_words[i].Raw > 0 && (_words[i].Raw & jWord.PackedWord.Raw) > 0)
                            population++;

                        i++;
                    }
#endif
                }
                else
                {
                    uint word = _words[i].Raw & jWord.Raw;

                    if (word > 0)
                        population += word.Population();

                    i++;
                }

                j++;
            }

            return population;
        }

        private int AndPopulationUncompressed(Vector vector)
        {
            int population = 0;

            int i = 0;
            int iMax = _wordCountPhysical;

            int j = 0;
            int jMax = vector._wordCountPhysical;

            while (i < iMax && j < jMax)
            {
                uint word = _words[i].Raw & vector._words[j].Raw;

                if (word > 0)
                    population += word.Population();

                i++;
                j++;
            }

            return population;
        }

        #endregion

        #region Or

        private void OrCompressed(Vector vector)
        {
            int i = 0;
            int iMax = _wordCountPhysical;

            int j = 0;
            int jMax = vector._wordCountPhysical;

            while (i < iMax && j < jMax)
            {
                Word jWord = vector._words[j];

                if (jWord.IsCompressed)
                {
                    if (jWord.FillBit)
                    {
                        int k = i + jWord.FillCount;

                        while (i < k)
                        {
                            _words[i].Raw = 0x7FFFFFFF;
                            i++;
                        }
                    }
                    else
                        i += jWord.FillCount;

#if POSITIONLIST
                    if (jWord.HasPackedWord && i < iMax)
                    {
                        _words[i].Raw |= jWord.PackedWord.Raw;
                        i++;
                    }
#endif
                }
                else
                {
                    _words[i].Raw |= jWord.Raw;
                    i++;
                }

                j++;
            }
        }

        private void OrUncompressed(Vector vector)
        {
            int i = 0;
            int iMax = _wordCountPhysical;

            int j = 0;
            int jMax = vector._wordCountPhysical;

            while (i < iMax && j < jMax)
            {
                _words[i].Raw |= vector._words[j].Raw;
                i++;
                j++;
            }
        }

        #endregion
    }
}
#endif
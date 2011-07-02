using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SoftwareBotany.Sunlight
{
    /// <summary>
    /// Word-aligned hybrid bit vector.
    /// </summary>
    /// <remarks>
    /// The implementation REQUIRES that <c>_words[WordCount - 1].IsCompressed == false</c> at all times and only allows
    /// <c>Set</c>ting bits on that Word and forward. In code comments will refer to this as the LAW.
    /// </remarks>
    public unsafe partial class Vector
    {
        public static Vector CreateUnion(params Vector[] vectors)
        {
            if (vectors == null || vectors.Any(v => v == null))
                throw new ArgumentNullException("vectors");

            Contract.EndContractBlock();

            if (vectors.Length == 0)
                return new Vector(false);

            if (vectors.Length == 1)
                return new Vector(vectors[0]);

            int maxWordCountLogical = vectors.Max(v => v._wordCountLogical);

            Vector vector = new Vector(false, vectors[0], maxWordCountLogical);

            for (int i = 1; i < vectors.Length; i++)
                vector.Or(vectors[i]);

            return vector;
        }

        #region Construction

        public Vector(Vector vector)
            : this(vector.IsCompressed, vector) { }

        public Vector(bool isCompressed)
            : this(isCompressed, null) { }

        public Vector(bool isCompressed, Vector vector)
            : this(isCompressed, vector, 0) { }

        public Vector(bool isCompressed, Vector vector, int wordsLength)
        {
            IsCompressed = isCompressed;

            if (vector == null)
            {
                WordsGrow(wordsLength);
                _wordCountPhysical = 1;
                _wordCountLogical = 1;
            }
            else if (IsCompressed && !vector.IsCompressed)
            {
                WordsGrow(wordsLength);
                _wordCountPhysical = 1;
                _wordCountLogical = 1;

                for (int i = 0; i < vector._wordCountPhysical; i++)
                    Set(i, vector._words[i]);
            }
            else if (!IsCompressed && vector.IsCompressed)
            {
                WordsGrow(Math.Max(vector._wordCountLogical, wordsLength));
                _wordCountPhysical = vector._wordCountLogical;
                _wordCountLogical = vector._wordCountLogical;

                fixed (Word* iFixed = _words, jFixed = vector._words)
                {
                    Word* jMax = jFixed + vector._wordCountPhysical;
                    Decompress(iFixed, jFixed, jMax);
                }
            }
            else
            {
                WordsGrow(Math.Max(vector._wordCountPhysical, wordsLength));
                _wordCountPhysical = vector._wordCountPhysical;
                _wordCountLogical = vector._wordCountLogical;

                Array.Copy(vector._words, _words, vector._wordCountPhysical);
            }
        }

        #endregion

        #region Rebuild

        private Vector _rebuilt;

        internal void RebuildHotReadPhase(int[] bitPositionShifts)
        {
            _rebuilt = new Vector(IsCompressed);

            foreach (int bitPosition in GetBitPositions(true))
            {
                int positionShift = bitPositionShifts[bitPosition];

                if (positionShift >= 0)
                    _rebuilt[bitPosition - positionShift] = true;
            }
        }

        internal bool RebuildHotWritePhase()
        {
            _words = _rebuilt._words;
            _wordCountPhysical = _rebuilt._wordCountPhysical;
            _wordCountLogical = _rebuilt._wordCountLogical;

            _rebuilt = null;

            return _wordCountLogical > 1 || _words[0].Raw > 0;
        }

        #endregion

        #region Words

        public readonly bool IsCompressed;

        private Word[] _words;
        private int _wordCountPhysical;
        private int _wordCountLogical;

        public void WordsClear()
        {
            _words = null;
            WordsGrow(0);
            _wordCountPhysical = 1;
            _wordCountLogical = 1;
        }

        private void WordsGrow(int length)
        {
            length = Math.Max(length, 2);

            if (_words == null)
                _words = new Word[length];
            else if (_words.Length < length)
            {
                length = Convert.ToInt32(length * WORDGROWTHFACTOR);

                Word[] tempWords = _words;
                _words = new Word[length];
                Array.Copy(tempWords, _words, _wordCountPhysical);
            }
        }

        private const double WORDGROWTHFACTOR = 1.1;

        #endregion

        #region Indexers

        public bool this[int bitPosition]
        {
            get
            {
                if (bitPosition < 0)
                    throw new ArgumentOutOfRangeException("bitPosition", bitPosition, "Must be > 0.");

                Contract.EndContractBlock();

                int wordPositionLogical = WordPositionLogical(bitPosition);
                Word word = GetWordLogical(wordPositionLogical);
                int wordBitPosition = WordBitPosition(bitPosition);
                return word[wordBitPosition];
            }
            set
            {
                if (bitPosition < 0)
                    throw new ArgumentOutOfRangeException("bitPosition", bitPosition, "Must be > 0.");

                Contract.EndContractBlock();

                Set(bitPosition, value);
            }
        }

        internal Word GetWordLogical(int wordPositionLogical)
        {
            if (wordPositionLogical < 0)
                throw new ArgumentOutOfRangeException("wordPositionLogical", wordPositionLogical, "Must be > 0.");

            Contract.EndContractBlock();

            if (wordPositionLogical >= _wordCountLogical)
                return new Word(0);

            bool isPacked;
            int wordPosition = WordPositionPhysical(wordPositionLogical, out isPacked);
            Word word = _words[wordPosition];

            if (word.IsCompressed)
            {
#if POSITIONLISTENABLED
                if (isPacked)
                    return word.PackedWord;
                else
#endif
                return new Word((word.FillBit && word.FillCount > 0) ? Word.COMPRESSIBLEMASK : 0);
            }
            else
                return word;
        }

        private static int WordPositionLogical(int bitPosition)
        {
            return bitPosition / (Word.SIZE - 1);
        }

        private static int WordBitPosition(int bitPosition)
        {
            return bitPosition % (Word.SIZE - 1);
        }

        private int WordPositionPhysical(int wordPositionLogical, out bool isPacked)
        {
            isPacked = false;

            // PERF : Short circuits. When !IsCompressed wordPositionLogical === wordPositionPhysical, ALWAYS.
            // When it IsCompressed, we take advantage of the fact that if you are asking for the last logical word,
            // that word must reside on the last physical word.
            if (!IsCompressed)
                return wordPositionLogical;
            else if (wordPositionLogical == _wordCountLogical - 1)
                return _wordCountPhysical - 1;

            int logical = 0;

            for (int i = 0; i < _wordCountPhysical; i++)
            {
                Word word = _words[i];
                isPacked = false;

                if (word.IsCompressed)
                {
                    logical += word.FillCount;

                    if (logical > wordPositionLogical)
                        return i;

#if POSITIONLISTENABLED
                    if (word.HasPackedWord)
                    {
                        isPacked = true;
                        logical++;

                        if (logical > wordPositionLogical)
                            return i;
                    }
#endif
                }
                else
                {
                    logical++;

                    if (logical > wordPositionLogical)
                        return i;
                }
            }

            throw new Exception("Error in algorithm.");
        }

        #endregion

        #region Setting

        internal void Set(int wordPositionLogical, Word word)
        {
            if (wordPositionLogical < 0)
                throw new ArgumentOutOfRangeException("wordPositionLogical", wordPositionLogical, "Must be >= 0.");

            if (word.IsCompressed)
                throw new NotSupportedException("Compressed Words not supported.");

            Contract.EndContractBlock();

            if (IsCompressed && wordPositionLogical < (_wordCountLogical - 1))
                throw new NotSupportedException("Writing is forward-only for a Compressed Vector.");

            if (word.Raw == 0 && ZeroFillCount(wordPositionLogical) > 0)
                return;

            ZeroFill(wordPositionLogical);

            // IsPacked can be safely ignored here because of the LAW.
            bool isPacked;
            int wordPosition = WordPositionPhysical(wordPositionLogical, out isPacked);
            _words[wordPosition] = word;
        }

        private void Set(int bitPosition, bool value)
        {
            if (bitPosition < 0)
                throw new ArgumentOutOfRangeException("bitPosition", bitPosition, "Must be >= 0.");

            Contract.EndContractBlock();

            int wordPositionLogical = WordPositionLogical(bitPosition);
            int wordBitPosition = WordBitPosition(bitPosition);

            if (IsCompressed && wordPositionLogical < _wordCountLogical - 1)
                throw new NotSupportedException("Writing is forward-only for a Compressed Vector.");

            if (!value && ZeroFillCount(wordPositionLogical) > 0)
                return;

            ZeroFill(wordPositionLogical);

            // IsPacked can be safely ignored here because of the LAW.
            bool isPacked;
            int wordPosition = WordPositionPhysical(wordPositionLogical, out isPacked);
            _words[wordPosition][wordBitPosition] = value;
        }

        #endregion

        #region Zero Filling

        private int ZeroFillCount(int wordPositionLogical)
        {
            return wordPositionLogical - (_wordCountLogical - 1);
        }

        private void ZeroFill(int wordPositionLogical)
        {
            if (IsCompressed)
            {
                ZeroFillWhenCompressedAndSingleWord(wordPositionLogical);
                ZeroFillWhenCompressedAndTailIsCompressedAndCompressible(wordPositionLogical);
#if POSITIONLISTENABLED
                ZeroFillWhenCompressedAndTailIsPackable(wordPositionLogical);
#endif
                ZeroFillWhenCompressedAndTailIsZero(wordPositionLogical);
                ZeroFillWhenCompressedAndLargeFill(wordPositionLogical);
            }

            ZeroFillFinish(wordPositionLogical);
        }

        private void ZeroFillWhenCompressedAndSingleWord(int wordPositionLogical)
        {
            int zeroFillCount = ZeroFillCount(wordPositionLogical);

            if (zeroFillCount > 0 && _wordCountPhysical == 1)
            {
                WordsGrow(2);
                _wordCountPhysical++;
                _wordCountLogical++;

                _words[_wordCountPhysical - 2].Compress();
                _words[_wordCountPhysical - 1].Raw = 0;
            }
        }

        private void ZeroFillWhenCompressedAndTailIsCompressedAndCompressible(int wordPositionLogical)
        {
            int zeroFillCount = ZeroFillCount(wordPositionLogical);

            if (zeroFillCount > 0
              && _words[_wordCountPhysical - 2].IsCompressed
#if POSITIONLISTENABLED
              && !_words[_wordCountPhysical - 2].HasPackedWord
#endif
              && _words[_wordCountPhysical - 1].IsCompressible
              && _words[_wordCountPhysical - 2].FillBit == _words[_wordCountPhysical - 1].CompressibleFillBit)
            {
                if (_words[_wordCountPhysical - 2].FillBit)
                {
                    _wordCountLogical++;

                    _words[_wordCountPhysical - 2].Raw++;
                    _words[_wordCountPhysical - 1].Raw = 0;
                }
                else
                {
                    _wordCountLogical += zeroFillCount;

                    _words[_wordCountPhysical - 2].Raw += (uint)zeroFillCount;
                    _words[_wordCountPhysical - 1].Raw = 0;
                }
            }
        }

#if POSITIONLISTENABLED
        private void ZeroFillWhenCompressedAndTailIsPackable(int wordPositionLogical)
        {
            int zeroFillCount = ZeroFillCount(wordPositionLogical);

            if (zeroFillCount > 0
              && _words[_wordCountPhysical - 2].IsCompressed
              && !_words[_wordCountPhysical - 2].HasPackedWord
              && _words[_wordCountPhysical - 1].Population == 1)
            {
                _wordCountLogical++;

                _words[_wordCountPhysical - 2].Pack(_words[_wordCountPhysical - 1]);
                _words[_wordCountPhysical - 1].Raw = 0;
            }
        }
#endif

        private void ZeroFillWhenCompressedAndTailIsZero(int wordPositionLogical)
        {
            int zeroFillCount = ZeroFillCount(wordPositionLogical);

            if (zeroFillCount > 0 && _words[_wordCountPhysical - 1].Raw == 0)
            {
                WordsGrow(_wordCountPhysical + 1);
                _wordCountPhysical++;
                _wordCountLogical += zeroFillCount;

                _words[_wordCountPhysical - 2].Compress();
                _words[_wordCountPhysical - 2].Raw += (uint)(zeroFillCount - 1);
                _words[_wordCountPhysical - 1].Raw = 0;
            }
        }

        private void ZeroFillWhenCompressedAndLargeFill(int wordPositionLogical)
        {
            int zeroFillCount = ZeroFillCount(wordPositionLogical);

            if (zeroFillCount > 1)
            {
                _words[_wordCountPhysical - 1].Compress();

                WordsGrow(_wordCountPhysical + 2);
                _wordCountPhysical += 2;
                _wordCountLogical += zeroFillCount;

                _words[_wordCountPhysical - 2].Compress();
                _words[_wordCountPhysical - 2].Raw += (uint)(zeroFillCount - 2);
                _words[_wordCountPhysical - 1].Raw = 0;
            }
        }

        private void ZeroFillFinish(int wordPositionLogical)
        {
            int zeroFillCount = ZeroFillCount(wordPositionLogical);

            if (zeroFillCount > 0)
            {
                if (IsCompressed)
                    _words[_wordCountPhysical - 1].Compress();

                WordsGrow(_wordCountPhysical + zeroFillCount);
                _wordCountPhysical += zeroFillCount;
                _wordCountLogical += zeroFillCount;
            }
        }

        #endregion

        public int Population
        {
            get
            {
                int population = 0;

                for (int i = 0; i < _wordCountPhysical; i++)
                    population += _words[i].Population;

                return population;
            }
        }

        #region Bit Positions

        public IEnumerable<bool> GetBits()
        {
            if (IsCompressed)
                throw new NotSupportedException("GetBits not supported for a Compressed Vector.");

            Contract.EndContractBlock();

            for (int i = 0; i < _wordCountPhysical; i++)
                foreach (bool bit in _words[i].Bits)
                    yield return bit;
        }

        public IEnumerable<int> GetBitPositions(bool value)
        {
            int bitPositionOffset = 0;

            for (int i = 0; i < _wordCountPhysical; i++)
            {
                Word word = _words[i];

                if (word.IsCompressed)
                {
                    if (word.FillBit == value)
                    {
                        int population = word.FillCount * (Word.SIZE - 1);
                        int stopPosition = bitPositionOffset + population;

                        while (bitPositionOffset < stopPosition)
                            yield return bitPositionOffset++;
                    }
                    else
                        bitPositionOffset += word.FillCount * (Word.SIZE - 1);

#if POSITIONLISTENABLED
                    if (word.HasPackedWord)
                    {
                        yield return word.PackedPosition + bitPositionOffset;
                        bitPositionOffset += (Word.SIZE - 1);
                    }
#endif
                }
                else
                {
                    int[] wordBitPositions = word.GetBitPositions(value);

                    // PERF : This empty check is a micro-optimization
                    if (wordBitPositions != Word.EmptyBitPositions)
                    {
                        foreach (int bitPosition in wordBitPositions)
                            yield return bitPosition + bitPositionOffset;
                    }

                    bitPositionOffset += (Word.SIZE - 1);
                }
            }
        }

        public IEnumerable<int> AndFilterBitPositions(Vector vector, bool value)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (IsCompressed)
                throw new NotSupportedException("AndFilterGetBitPositions not supported for a Compressed Vector.");

            Contract.EndContractBlock();

            int i = 0;
            int iMax = _wordCountPhysical;

            int j = 0;
            int jMax = vector._wordCountPhysical;

            while (i < iMax && j < jMax)
            {
                Word jWord = vector._words[j];

                if (jWord.IsCompressed)
                {
                    if (jWord.FillBit == value)
                    {
                        int bitPositionOffset = i * (Word.SIZE - 1);

                        for (int k = i; k < i + jWord.FillCount && k < iMax; k++)
                        {
                            foreach (int bitPosition in _words[k].GetBitPositions(value))
                                yield return bitPosition + bitPositionOffset;

                            bitPositionOffset += (Word.SIZE - 1);
                        }
                    }

                    i += jWord.FillCount;

#if POSITIONLISTENABLED
                    if (jWord.HasPackedWord && i < iMax)
                    {
                        if (_words[i].Raw > 0 && _words[i][jWord.PackedPosition])
                            yield return (i * (Word.SIZE - 1)) + jWord.PackedPosition;

                        i++;
                    }
#endif

                    j++;
                }
                else
                {
                    uint word = _words[i].Raw & jWord.Raw;
                    int[] wordBitPositions = new Word(word).GetBitPositions(value);

                    // PERF : This empty check is a micro-optimization
                    if (wordBitPositions != Word.EmptyBitPositions)
                    {
                        foreach (int bitPosition in wordBitPositions)
                        {
                            int bitPositionOffset = i * (Word.SIZE - 1);
                            yield return bitPosition + bitPositionOffset;
                        }
                    }

                    i++;
                    j++;
                }
            }
        }

        #endregion

        #region And

        public void And(Vector vector)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (IsCompressed)
                throw new NotSupportedException("And (in-place) not supported for a Compressed Vector.");

            Contract.EndContractBlock();

            fixed (Word* iFixed = _words, jFixed = vector._words)
            {
                Word* i = iFixed;
                Word* iMax = iFixed + _wordCountPhysical;

                Word* j = jFixed;
                Word* jMax = jFixed + vector._wordCountPhysical;

                i = vector.IsCompressed ? AndCompressed(i, iMax, j, jMax) : AndUncompressed(i, iMax, j, jMax);

                if (i < iMax)
                {
                    _wordCountPhysical = (int)(i - iFixed);
                    _wordCountLogical = (int)(i - iFixed);

                    while (i < iMax)
                    {
                        i->Raw = 0;
                        i++;
                    }
                }
            }
        }

        private static Word* AndCompressed(Word* i, Word* iMax, Word* j, Word* jMax)
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

#if POSITIONLISTENABLED
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

        private static Word* AndUncompressed(Word* i, Word* iMax, Word* j, Word* jMax)
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

        public int AndPopulation(Vector vector)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (IsCompressed)
                throw new NotSupportedException("AndPopulation not supported for a compressed Vector.");

            Contract.EndContractBlock();

            fixed (Word* iFixed = _words, jFixed = vector._words)
            {
                Word* i = iFixed;
                Word* iMax = iFixed + _wordCountPhysical;

                Word* j = jFixed;
                Word* jMax = jFixed + vector._wordCountPhysical;

                return vector.IsCompressed ? AndPopulationCompressed(i, iMax, j, jMax) : AndPopulationUncompressed(i, iMax, j, jMax);
            }
        }

        private static int AndPopulationCompressed(Word* i, Word* iMax, Word* j, Word* jMax)
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

#if POSITIONLISTENABLED
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
                        population += Census.ComputePopulation(word);

                    i++;
                }

                j++;
            }

            return population;
        }

        private static int AndPopulationUncompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
            int population = 0;

            while (i < iMax && j < jMax)
            {
                uint word = i->Raw & j->Raw;

                if (word > 0)
                    population += Census.ComputePopulation(word);

                i++;
                j++;
            }

            return population;
        }

        #endregion

        #region Or

        public void Or(Vector vector)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (IsCompressed)
                throw new NotSupportedException("Or (in-place) not supported for a Compressed Vector.");

            Contract.EndContractBlock();

            WordsGrow(vector._wordCountLogical);
            _wordCountPhysical = Math.Max(_wordCountPhysical, vector._wordCountLogical);
            _wordCountLogical = Math.Max(_wordCountLogical, vector._wordCountLogical);

            fixed (Word* iFixed = _words, jFixed = vector._words)
            {
                Word* i = iFixed;
                Word* iMax = iFixed + _wordCountPhysical;

                Word* j = jFixed;
                Word* jMax = jFixed + vector._wordCountPhysical;

                if (vector.IsCompressed)
                    OrCompressed(i, iMax, j, jMax);
                else
                    OrUncompressed(i, iMax, j, jMax);
            }
        }

        private static void OrCompressed(Word* i, Word* iMax, Word* j, Word* jMax)
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

#if POSITIONLISTENABLED
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

        private static void OrUncompressed(Word* i, Word* iMax, Word* j, Word* jMax)
        {
            while (i < iMax && j < jMax)
            {
                i->Raw |= j->Raw;
                i++;
                j++;
            }
        }

        #endregion

        #region Helpers

        private static void Decompress(Word* i, Word* j, Word* jMax)
        {
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

#if POSITIONLISTENABLED
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

        #endregion
    }
}
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
    public sealed partial class Vector
    {
        static Vector()
        {
            _safeVectorLogic = new VectorLogicSafe();

            Type unsafeVectorLogicType = Type.GetType("SoftwareBotany.Sunlight.VectorLogicUnsafe, SoftwareBotany.Sunlight.Unsafe");

            if (unsafeVectorLogicType != null)
                _unsafeVectorLogic = (IVectorLogic)Activator.CreateInstance(unsafeVectorLogicType);
        }

        private static readonly IVectorLogic _safeVectorLogic;
        private static readonly IVectorLogic _unsafeVectorLogic;

        public static Vector CreateUnion(params Vector[] vectors)
        {
            if (vectors == null || vectors.Any(v => v == null))
                throw new ArgumentNullException("vectors");

            if (vectors.Length < 2)
                throw new ArgumentOutOfRangeException("vectors", "At least 2 Vectors must be provided in order to CreateUnion.");

            Contract.EndContractBlock();

            int maxWordCountLogical = vectors.Max(v => v._wordCountLogical);

            Vector vector = new Vector(vectors[0]._allowUnsafe, VectorCompression.None, vectors[0], maxWordCountLogical);

            for (int i = 1; i < vectors.Length; i++)
                vector.Or(vectors[i]);

            return vector;
        }

        #region Construction

        public Vector(bool allowUnsafe, VectorCompression compression)
            : this(allowUnsafe, compression, null) { }

        public Vector(bool allowUnsafe, VectorCompression compression, Vector vector)
            : this(allowUnsafe, compression, vector, 0) { }

        private Vector(bool allowUnsafe, VectorCompression compression, Vector vector, int wordsLength)
        {
            if (allowUnsafe && _unsafeVectorLogic == null)
                throw new ArgumentException("Cannot create an unsafe Vector unless the SoftwareBotany.Sunlight.Unsafe Assembly is included in the project.");

            _allowUnsafe = allowUnsafe;
            _isCompressed = (compression == VectorCompression.Compressed || compression == VectorCompression.CompressedWithPackedPosition);
            _isPackedPositionEnabled = compression == VectorCompression.CompressedWithPackedPosition;

            if (vector == null)
            {
                WordsGrow(wordsLength);
                _wordCountPhysical = 1;
                _wordCountLogical = 1;
            }
            else if (_isCompressed && !vector._isCompressed)
            {
                WordsGrow(wordsLength);
                _wordCountPhysical = 1;
                _wordCountLogical = 1;

                for (int i = 0; i < vector._wordCountPhysical; i++)
                    SetWord(i, vector._words[i]);
            }
            else if (!_isCompressed && vector._isCompressed)
            {
                WordsGrow(Math.Max(vector._wordCountLogical, wordsLength));
                _wordCountPhysical = vector._wordCountLogical;
                _wordCountLogical = vector._wordCountLogical;

                VectorLogic.Decompress(this._words, vector._words, vector._wordCountPhysical);
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

        #region Optimize

        internal bool OptimizeReadPhase(int[] bitPositionShifts, out Vector optimized)
        {
            optimized = new Vector(_allowUnsafe, Compression);

            foreach (int bitPosition in GetBitPositions(true))
            {
                int positionShift = bitPositionShifts[bitPosition];

                if (positionShift >= 0)
                    optimized[bitPosition - positionShift] = true;
            }

            return optimized._wordCountLogical > 1 || optimized._words[0].Raw > 0;
        }

        #endregion

        #region Words

        public bool AllowUnsafe { get { return _allowUnsafe; } }
        private readonly bool _allowUnsafe;

        private IVectorLogic VectorLogic { get { return _allowUnsafe ? _unsafeVectorLogic : _safeVectorLogic; } }

        public VectorCompression Compression
        {
            get
            {
                return _isCompressed
                    ? (_isPackedPositionEnabled ? VectorCompression.CompressedWithPackedPosition : VectorCompression.Compressed)
                    : VectorCompression.None;
            }
        }

        public bool IsCompressed { get { return _isCompressed; } }
        private readonly bool _isCompressed;

        public bool IsPackedPositionEnabled { get { return _isPackedPositionEnabled; } }
        private readonly bool _isPackedPositionEnabled;

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
            {
                _words = new Word[length];
            }
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

                SetBit(bitPosition, value);
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
                if (isPacked)
                    return word.PackedWord;
                else
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
            if (!_isCompressed)
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

                    if (word.HasPackedWord)
                    {
                        isPacked = true;
                        logical++;

                        if (logical > wordPositionLogical)
                            return i;
                    }
                }
                else
                {
                    logical++;

                    if (logical > wordPositionLogical)
                        return i;
                }
            }

            throw new NotImplementedException("Error in algorithm.");
        }

        #endregion

        #region Setting

        internal void SetWord(int wordPositionLogical, Word word)
        {
            if (wordPositionLogical < 0)
                throw new ArgumentOutOfRangeException("wordPositionLogical", wordPositionLogical, "Must be >= 0.");

            if (word.IsCompressed)
                throw new NotSupportedException("Compressed Words not supported.");

            Contract.EndContractBlock();

            if (_isCompressed && wordPositionLogical < (_wordCountLogical - 1))
                throw new NotSupportedException("Writing is forward-only for a compressed Vector.");

            if (word.Raw == 0 && ZeroFillCount(wordPositionLogical) > 0)
                return;

            ZeroFill(wordPositionLogical);

            // IsPacked can be safely ignored here because of the LAW.
            bool isPacked;
            int wordPosition = WordPositionPhysical(wordPositionLogical, out isPacked);
            _words[wordPosition] = word;
        }

        private void SetBit(int bitPosition, bool value)
        {
            if (bitPosition < 0)
                throw new ArgumentOutOfRangeException("bitPosition", bitPosition, "Must be >= 0.");

            Contract.EndContractBlock();

            int wordPositionLogical = WordPositionLogical(bitPosition);
            int wordBitPosition = WordBitPosition(bitPosition);

            if (_isCompressed && wordPositionLogical < _wordCountLogical - 1)
                throw new NotSupportedException("Writing is forward-only for a compressed Vector.");

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
            if (_isCompressed)
            {
                ZeroFillWhenCompressedAndSingleWord(wordPositionLogical);
                ZeroFillWhenCompressedAndTailIsCompressedAndCompressible(wordPositionLogical);
                ZeroFillWhenCompressedAndTailIsPackable(wordPositionLogical);
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
                && !_words[_wordCountPhysical - 2].HasPackedWord
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

        private void ZeroFillWhenCompressedAndTailIsPackable(int wordPositionLogical)
        {
            if (!_isPackedPositionEnabled)
                return;

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
                if (_isCompressed)
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

        public IEnumerable<bool> Bits
        {
            get
            {
                if (_isCompressed)
                    throw new NotSupportedException("Not supported for a compressed Vector.");

                Contract.EndContractBlock();

                for (int i = 0; i < _wordCountPhysical; i++)
                    foreach (bool bit in _words[i].Bits)
                        yield return bit;
            }
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

                    if (word.HasPackedWord)
                    {
                        yield return word.PackedPosition + bitPositionOffset;
                        bitPositionOffset += (Word.SIZE - 1);
                    }
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

            if (_isCompressed)
                throw new NotSupportedException("Not supported for a compressed Vector.");

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

                    if (jWord.HasPackedWord && i < iMax)
                    {
                        if (_words[i].Raw > 0 && _words[i][jWord.PackedPosition])
                            yield return (i * (Word.SIZE - 1)) + jWord.PackedPosition;

                        i++;
                    }

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

        #region Logical Operations

        public void And(Vector vector)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (_isCompressed)
                throw new NotSupportedException("Not supported for a compressed Vector.");

            Contract.EndContractBlock();

            VectorLogic.And(_words, ref _wordCountPhysical, ref _wordCountLogical, vector._isCompressed, vector._words, vector._wordCountPhysical);
        }

        public int AndPopulation(Vector vector)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (_isCompressed)
                throw new NotSupportedException("Not supported for a compressed Vector.");

            Contract.EndContractBlock();

            return VectorLogic.AndPopulation(_words, _wordCountPhysical, vector._isCompressed, vector._words, vector._wordCountPhysical);
        }

        public void Or(Vector vector)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            if (_isCompressed)
                throw new NotSupportedException("Not supported for a compressed Vector.");

            Contract.EndContractBlock();

            WordsGrow(vector._wordCountLogical);
            _wordCountPhysical = Math.Max(_wordCountPhysical, vector._wordCountLogical);
            _wordCountLogical = Math.Max(_wordCountLogical, vector._wordCountLogical);

            VectorLogic.Or(_words, _wordCountPhysical, vector._isCompressed, vector._words, vector._wordCountPhysical);
        }

        #endregion
    }
}
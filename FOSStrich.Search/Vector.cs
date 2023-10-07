namespace FOSStrich.Search;

using System.Buffers;

/// <summary>
/// Word-aligned hybrid bit vector.
/// </summary>
/// <remarks>
/// The implementation REQUIRES that <c>_words[WordCount - 1].IsCompressed == false</c> at all times and only allows
/// <c>Set</c>ting bits on that Word and forward. In code comments will refer to this as the LAW.
/// </remarks>
public sealed partial class Vector
{
    #region Construction

    public Vector(VectorCompression compression)
        : this(compression, null) { }

    public Vector(VectorCompression compression, Vector vector)
        : this(compression, vector, 0) { }

    private Vector(VectorCompression compression, Vector vector, int wordsLengthHint)
    {
        IsCompressed = (compression == VectorCompression.Compressed || compression == VectorCompression.CompressedWithPackedPosition);
        IsPackedPositionEnabled = compression == VectorCompression.CompressedWithPackedPosition;

        if (vector == null)
        {
            WordsGrow(wordsLengthHint);
            _wordCountPhysical = 1;
            _wordCountLogical = 1;

            return;
        }

        // There are 9 possible "copy" combinations: VectorCompression x VectorCompression
        // 1. Handle the binary copies first.
        //      N,      N
        //      C,      C
        //      CWPP,   CWPP
        // 2. Use an optimized decompression algorithm next.
        //      N,      C
        //      N,      CWPP
        // 3. The remainder are more complicated compressions or decompressions. An optimization we can make is to use WordsGrow smartly.
        //      C,      N
        //      C,      CWPP (WordsGrow to vector._wordCountPhysical because that is the minimum size [occurs when CWPP contains no PackedWords])
        //      CWPP,   N
        //      CWPP,   C    (WordsGrow to vector._wordCountPhysical / 2 because that is the minimum size [occurs when every other Word in C is uncompressed and can be Packed])
        if (Compression == vector.Compression)
        {
            WordsGrow(Math.Max(vector._wordCountPhysical, wordsLengthHint));
            _wordCountPhysical = vector._wordCountPhysical;
            _wordCountLogical = vector._wordCountLogical;

            Array.Copy(vector._words, _words, vector._wordCountPhysical);
        }
        else if (!IsCompressed)
        {
            WordsGrow(Math.Max(vector._wordCountLogical, wordsLengthHint));
            _wordCountPhysical = vector._wordCountLogical;
            _wordCountLogical = vector._wordCountLogical;

            DecompressInPlace(vector);
        }
        else
        {
            int wordCountPhysical = 0;

            if (!IsPackedPositionEnabled && vector.IsPackedPositionEnabled)
                wordCountPhysical = vector._wordCountPhysical;
            else if (IsPackedPositionEnabled && vector.IsCompressed)
                wordCountPhysical = vector._wordCountPhysical / 2;

            WordsGrow(Math.Max(wordCountPhysical, wordsLengthHint));
            _wordCountPhysical = 1;
            _wordCountLogical = 1;

            // We must track vector's wordPositionLogical manually because SetWord is optimized to ignore 0's, thereby not increasing _wordCountLogical.
            int wordPositionLogical = 0;

            for (int i = 0; i < vector._wordCountPhysical; i++)
            {
                Word word = vector._words[i];
                SetWord(wordPositionLogical, word);

                wordPositionLogical += word.IsCompressed ? word.FillCount : 1;

                if (word.HasPackedWord)
                    wordPositionLogical++;
            }
        }
    }

    internal bool IsUnused => _wordCountLogical == 1 && _words[0].Raw == 0u;

    #endregion

    #region Optimize

    internal bool OptimizeReadPhase(int[] bitPositionShifts, out Vector optimized)
    {
        optimized = new Vector(Compression);

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

    public VectorCompression Compression =>
        IsCompressed
            ? (IsPackedPositionEnabled ? VectorCompression.CompressedWithPackedPosition : VectorCompression.Compressed)
            : VectorCompression.None;

    public bool IsCompressed { get; }
    public bool IsPackedPositionEnabled { get; }

    private Word[] _words;
    private int _wordCountPhysical;
    private int _wordCountLogical;

    public void WordsClear()
    {
        if (_words != null)
            ArrayPool<Word>.Shared.Return(_words);

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
            _words = ArrayPool<Word>.Shared.Rent(length);

            // We choose to explicitly Clear (rather than use the Rent overload) so that in the "else if" case below
            // we don't waste cycles Clearing parts of the Array that are going to receive a Copy.
            Array.Clear(_words, 0, _words.Length);
        }
        else if (_words.Length < length)
        {
            length = Convert.ToInt32(length * WORDGROWTHFACTOR);

            Word[] tempWords = _words;
            _words = ArrayPool<Word>.Shared.Rent(length);
            Array.Copy(tempWords, _words, _wordCountPhysical);

            // We choose to explicitly Clear (rather than use the Rent overload) so that
            // we don't waste cycles Clearing parts of the Array that received a Copy.
            //
            // We know that _wordCountPhysical < _words.Length <= length, so we don't need to check it here.
            Array.Clear(_words, _wordCountPhysical, _words.Length - _wordCountPhysical);

            ArrayPool<Word>.Shared.Return(tempWords);
        }
    }

    private const double WORDGROWTHFACTOR = 1.1;

    private Span<Word> GetWordsSpanPhysical() => new(_words, 0, _wordCountPhysical);

    #endregion

    #region Indexers

    public bool this[int bitPosition]
    {
        get
        {
            if (bitPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(bitPosition), bitPosition, "Must be > 0.");

            int wordPositionLogical = WordPositionLogical(bitPosition);
            Word word = GetWordLogical(wordPositionLogical);
            int wordBitPosition = WordBitPosition(bitPosition);
            return word[wordBitPosition];
        }
        set
        {
            if (bitPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(bitPosition), bitPosition, "Must be > 0.");

            SetBit(bitPosition, value);
        }
    }

    internal Word GetWordLogical(int wordPositionLogical)
    {
        if (wordPositionLogical < 0)
            throw new ArgumentOutOfRangeException(nameof(wordPositionLogical), wordPositionLogical, "Must be > 0.");

        if (wordPositionLogical >= _wordCountLogical)
            return new Word(0);

        int wordPositionPhysical = WordPositionPhysical(wordPositionLogical, out bool isPacked);
        Word word = _words[wordPositionPhysical];

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

    private static int WordPositionLogical(int bitPosition) => bitPosition / (Word.SIZE - 1);

    private static int WordBitPosition(int bitPosition) => bitPosition % (Word.SIZE - 1);

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
            throw new ArgumentOutOfRangeException(nameof(wordPositionLogical), wordPositionLogical, "Must be >= 0.");

        if (IsCompressed && wordPositionLogical < (_wordCountLogical - 1))
            throw new NotSupportedException("Writing is forward-only for a compressed Vector.");

        bool isZero = word.Raw == 0u || (word.IsCompressed && !word.FillBit && !word.HasPackedWord);

        if (isZero && ZeroFillCount(wordPositionLogical) > 0)
            return;

        ZeroFill(wordPositionLogical);

        // IsPacked can be safely ignored here because of the LAW.
        bool isPacked;
        int wordPositionPhysical = WordPositionPhysical(wordPositionLogical, out isPacked);

        if (isZero)
        {
            _words[wordPositionPhysical].Raw = 0;
        }
        else if (!word.IsCompressed)
        {
            _words[wordPositionPhysical] = word;
        }
        else
        {
            if (!IsCompressed)
            {
                WordsGrow(_wordCountPhysical + word.FillCount - 1);
                _wordCountPhysical += word.FillCount - 1;
                _wordCountLogical += word.FillCount - 1;

                if (word.FillBit)
                    for (int i = wordPositionPhysical; i < _wordCountPhysical; i++)
                        _words[i].Raw = 0x7FFFFFFF;

                if (word.HasPackedWord)
                {
                    WordsGrow(_wordCountPhysical + 1);
                    _wordCountPhysical++;
                    _wordCountLogical++;

                    _words[_wordCountPhysical - 1] = word.PackedWord;
                }
            }
            else
            {
                // We're breaking the LAW here; however, we will NOT enter any other Vector functions until it is restored.
                _words[wordPositionPhysical] = word;
                _wordCountLogical += word.FillCount - 1;

                if (word.HasPackedWord)
                    _wordCountLogical++;

                // Immediately restore the LAW.
                WordsGrow(_wordCountPhysical + 1);
                _wordCountPhysical++;
                _wordCountLogical++;

                if (word.HasPackedWord && !IsPackedPositionEnabled)
                {
                    _words[_wordCountPhysical - 1] = _words[_wordCountPhysical - 2].Unpack();
                    _wordCountLogical--;
                }
            }
        }
    }

    private void SetBit(int bitPosition, bool value)
    {
        if (bitPosition < 0)
            throw new ArgumentOutOfRangeException(nameof(bitPosition), bitPosition, "Must be >= 0.");

        int wordPositionLogical = WordPositionLogical(bitPosition);
        int wordBitPosition = WordBitPosition(bitPosition);

        if (IsCompressed && wordPositionLogical < _wordCountLogical - 1)
            throw new NotSupportedException("Writing is forward-only for a compressed Vector.");

        if (!value && ZeroFillCount(wordPositionLogical) > 0)
            return;

        ZeroFill(wordPositionLogical);

        // IsPacked can be safely ignored here because of the LAW.
        int wordPositionPhysical = WordPositionPhysical(wordPositionLogical, out bool isPacked);
        _words[wordPositionPhysical][wordBitPosition] = value;
    }

    #endregion

    #region Zero Filling

    private int ZeroFillCount(int wordPositionLogical) => wordPositionLogical - (_wordCountLogical - 1);

    private void ZeroFill(int wordPositionLogical)
    {
        if (IsCompressed)
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
        if (!IsPackedPositionEnabled)
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
            if (IsCompressed)
                _words[_wordCountPhysical - 1].Compress();

            WordsGrow(_wordCountPhysical + zeroFillCount);
            _wordCountPhysical += zeroFillCount;
            _wordCountLogical += zeroFillCount;
        }
    }

    #endregion

    #region Population

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

    public bool PopulationAny
    {
        get
        {
            for (int i = 0; i < _wordCountPhysical; i++)
                if (_words[i].Population > 0)
                    return true;

            return false;
        }
    }

    #endregion

    #region Bit Positions

    public IEnumerable<bool> Bits
    {
        get
        {
            if (IsCompressed)
                throw new NotSupportedException("Not supported for a compressed Vector.");

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
                // PERF : Always check HasBitPositions before enumerating on GetBitPositions.
                if (word.HasBitPositions(value))
                    foreach (int bitPosition in word.GetBitPositions(value))
                        yield return bitPosition + bitPositionOffset;

                bitPositionOffset += (Word.SIZE - 1);
            }
        }
    }

    #endregion

    #region VectorOperations

    public void DecompressInPlace(Vector vector)
    {
        if (vector == null)
            throw new ArgumentNullException(nameof(vector));

        if (IsCompressed)
            throw new NotSupportedException("Not supported for a compressed Vector.");

        if (!vector.IsCompressed)
            throw new NotSupportedException("Not supported for two uncompressed Vectors.");

        WordsGrow(vector._wordCountLogical);
        _wordCountPhysical = vector._wordCountLogical;
        _wordCountLogical = vector._wordCountLogical;

        if (!vector.IsPackedPositionEnabled)
            DecompressInPlaceNoneCompressed(this, vector);
        else
            DecompressInPlaceNoneCompressedWithPackedPosition(this, vector);
    }

    public void AndInPlace(Vector vector)
    {
        if (vector == null)
            throw new ArgumentNullException(nameof(vector));

        if (IsCompressed)
            throw new NotSupportedException("Not supported for a compressed Vector.");

        if (!vector.IsCompressed)
            AndInPlaceNoneNone(this, vector);
        else
            AndInPlaceNoneCompressedWithPackedPosition(this, vector);
    }

    public Vector AndOutOfPlace(Vector vector, VectorCompression resultCompression)
    {
        if (vector == null)
            throw new ArgumentNullException(nameof(vector));

        var (lessCompression, moreCompression) = OrderByCompression(vector);

        if (!moreCompression.IsCompressed)
            return AndOutOfPlaceNoneNone(lessCompression, moreCompression, resultCompression);
        else if (!lessCompression.IsCompressed)
            return AndOutOfPlaceNoneCompressedWithPackedPosition(lessCompression, moreCompression, resultCompression);
        else
            return AndOutOfPlaceCompressedWithPackedPositionCompressedWithPackedPosition(lessCompression, moreCompression, resultCompression);
    }

    public int AndPopulation(Vector vector)
    {
        if (vector == null)
            throw new ArgumentNullException(nameof(vector));

        var (lessCompression, moreCompression) = OrderByCompression(vector);

        if (!moreCompression.IsCompressed)
            return AndPopulationNoneNone(lessCompression, moreCompression);
        else if (!lessCompression.IsCompressed)
            return AndPopulationNoneCompressedWithPackedPosition(lessCompression, moreCompression);
        else
            return AndPopulationCompressedWithPackedPositionCompressedWithPackedPosition(lessCompression, moreCompression);
    }

    public bool AndPopulationAny(Vector vector)
    {
        if (vector == null)
            throw new ArgumentNullException(nameof(vector));

        if (IsCompressed && vector.IsCompressed)
            throw new NotImplementedException("Not implemented for two compressed Vector.");

        var (lessCompression, moreCompression) = OrderByCompression(vector);

        if (!moreCompression.IsCompressed)
            return AndPopulationAnyNoneNone(lessCompression, moreCompression);
        else if (!lessCompression.IsCompressed)
            return AndPopulationAnyNoneCompressedWithPackedPosition(lessCompression, moreCompression);
        else
            throw new NotImplementedException("See above. This code will never execute.");
    }

    public void OrInPlace(Vector vector)
    {
        if (vector == null)
            throw new ArgumentNullException(nameof(vector));

        if (IsCompressed)
            throw new NotSupportedException("Not supported for a compressed Vector.");

        WordsGrow(vector._wordCountLogical);
        _wordCountPhysical = Math.Max(_wordCountPhysical, vector._wordCountLogical);
        _wordCountLogical = Math.Max(_wordCountLogical, vector._wordCountLogical);

        if (!vector.IsCompressed)
            OrInPlaceNoneNone(this, vector);
        else
            OrInPlaceNoneCompressedWithPackedPosition(this, vector);
    }

    private (Vector LessCompression, Vector MoreCompression) OrderByCompression(Vector vector) =>
        Compression <= vector.Compression ? (this, vector) : (vector, this);

    #endregion
}
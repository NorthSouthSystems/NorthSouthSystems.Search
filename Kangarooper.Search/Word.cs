namespace Kangarooper.Search
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    internal struct Word
    {
        public const int SIZE = 32;

        #region Construction

        public Word(uint raw)
        {
            if (raw >= COMPRESSEDMASK)
                throw new ArgumentOutOfRangeException("raw", raw, string.Format(CultureInfo.InvariantCulture, "Must be < COMPRESSEDMASK : 0x{0:X}.", COMPRESSEDMASK));

            Contract.EndContractBlock();

            Raw = raw;
        }

        public Word(bool fillBit, int fillCount)
        {
            if (fillCount < 0)
                throw new ArgumentOutOfRangeException("fillCount", fillCount, "Must be >= 0.");

            if (fillCount > FILLCOUNTMASK)
                throw new ArgumentOutOfRangeException("fillCount", fillCount, string.Format(CultureInfo.InvariantCulture, "Must be <= FILLCOUNTMASK : 0x{0:X}.", FILLCOUNTMASK));

            Contract.EndContractBlock();

            Raw = COMPRESSEDMASK | (fillBit ? FILLBITMASK : 0u) | (uint)fillCount;
        }

        #endregion

        public uint Raw;

        #region Indexers

        public bool this[int position]
        {
            get
            {
                return (Raw & ComputeIndexerMask(position)) > 0u;
            }
            set
            {
                uint mask = ComputeIndexerMask(position);

                if (value)
                    Raw |= mask;
                else
                    Raw &= (~mask);
            }
        }

        private uint ComputeIndexerMask(int position)
        {
            if (IsCompressed)
                throw new NotSupportedException("Not supported for compressed Words.");

            if (position < 0)
                throw new ArgumentOutOfRangeException("position", position, "Must be >= 0.");

            if (position >= SIZE - 1)
                throw new ArgumentOutOfRangeException("position", position, string.Format(CultureInfo.InvariantCulture, "Must be < SIZE - 1 : {0}.", SIZE - 1));

            Contract.EndContractBlock();

            return 1u << (30 - position);
        }

        public bool[] Bits
        {
            get
            {
                if (IsCompressed)
                    throw new NotSupportedException("Not supported for compressed Words.");

                Contract.EndContractBlock();

                bool[] bits = new bool[SIZE - 1];
                int current = 0;

                uint mask = 0x40000000u;

                for (int i = 0; i < SIZE - 1; i++)
                {
                    bits[current++] = (Raw & mask) > 0;
                    mask >>= 1;
                }

                return bits;
            }
        }

        public bool HasBitPositions(bool value)
        {
            if (IsCompressed)
                throw new NotSupportedException("Not supported for compressed Words.");

            Contract.EndContractBlock();

            return (value && Raw != 0u) || (!value && Raw != COMPRESSIBLEMASK);
        }

        public int[] GetBitPositions(bool value)
        {
            if (IsCompressed)
                throw new NotSupportedException("Not supported for compressed Words.");

            Contract.EndContractBlock();

            if ((value && Raw == 0u) || (!value && Raw == COMPRESSIBLEMASK))
                return _emptyBitPositions;

            int count = value ? Population : SIZE - 1 - Population;
            int[] bitPositions = new int[count];
            int current = 0;

            uint mask = 0x40000000u;

            for (int i = 0; i < SIZE - 1; i++)
            {
                if ((Raw & mask) > 0 == value)
                    bitPositions[current++] = i;

                mask >>= 1;
            }

            return bitPositions;
        }

        private static readonly int[] _emptyBitPositions = new int[0];

        #endregion

        #region Compression

        internal const uint COMPRESSIBLEMASK = 0x7FFFFFFFu;
        internal const uint COMPRESSEDMASK = 0x80000000u;
        internal const uint FILLBITMASK = 0x40000000u;
        internal const uint FILLCOUNTMASK = 0x01FFFFFFu;

        public bool IsCompressible { get { return Raw == 0u || Raw == COMPRESSIBLEMASK; } }
        public bool CompressibleFillBit { get { return Raw != 0u; } }
        public bool IsCompressed { get { return Raw >= COMPRESSEDMASK; } }
        public bool FillBit { get { return (Raw & FILLBITMASK) > 0u; } }
        public int FillCount { get { return (int)(Raw & FILLCOUNTMASK); } }

        public void Compress()
        {
            if (IsCompressible)
                Raw = CompressibleFillBit ? 0xC0000001u : 0x80000001u;
        }

        #endregion

        #region Packing

        private const uint PACKEDPOSITIONMASK = 0x3E000000u;

        public bool HasPackedWord { get { return IsCompressed && (Raw & PACKEDPOSITIONMASK) > 0u; } }

        /// <summary>
        /// Position is relative to the global bit position.  More specifically, it ignores the most significant bit which is
        /// reserved for compression.  E.g. for a given word's most significant byte, X012 3456.  This follows the same convention
        /// as every Word and Vector function.  There are SIZE - 1 bit positions in a uncompressed Word using this convention,
        /// and therefore bit positions go from 0 to 30.  Because 5 bits represent the PACKEDPOSITIONMASK, values 0 through 31 can be
        /// represented.  Because 00000 = 0 should mean the absence of a PackedWord, we take the value stored under the
        /// PACKEDPOSITIONMASK and subtract 1 to get the real PackedPosition. This function will then return -1 should a PackedWord
        /// not be present. This offset is reflected in the PackedWord property and also the Pack function.
        /// </summary>
        public int PackedPosition
        {
            get
            {
                if (!HasPackedWord)
                    throw new NotSupportedException("Cannot retrieve the PackedPosition for a Word that does not contain a Packed Word.");

                Contract.EndContractBlock();

                return (int)((Raw & PACKEDPOSITIONMASK) >> (SIZE - 7)) - 1;
            }
        }

        public Word PackedWord
        {
            get
            {
                if (!HasPackedWord)
                    throw new NotSupportedException("Cannot retrieve the PackedWord for a Word that does not contain a Packed Word.");

                Contract.EndContractBlock();

                return new Word(1u << (SIZE - 2 - PackedPosition));
            }
        }

        internal void Pack(Word word)
        {
            if (!IsCompressed)
                throw new NotSupportedException("Cannot pack a Word into an uncompressed Word.");

            if (HasPackedWord)
                throw new NotSupportedException("Cannot pack a Word into a Word that already HasPackedWord.");

            if (word.IsCompressed)
                throw new NotSupportedException("Cannot pack a compressed Word.");

            if (word.Population != 1)
                throw new NotSupportedException("Can only pack a Word with exactly 1 bit set (Population = 1).");

            Contract.EndContractBlock();

            uint packedPosition = (uint)word.GetBitPositions(true)[0];
            Raw |= (packedPosition + 1) << (SIZE - 7);
        }

        internal Word Unpack()
        {
            if (!IsCompressed)
                throw new NotSupportedException("Cannot unpack a Word from an uncompressed Word.");

            if (!HasPackedWord)
                throw new NotSupportedException("Cannot unpack a Word from a Word that does not HasPackedWord.");

            Contract.EndContractBlock();

            var packedWord = PackedWord;

            Raw &= ~PACKEDPOSITIONMASK;

            return packedWord;
        }

        #endregion

        #region Population

        public int Population
        {
            get
            {
                if (Raw == 0u)
                    return 0;

                int population = 0;

                if (IsCompressed)
                {
                    if (FillBit)
                        population = FillCount * (SIZE - 1);

                    if (HasPackedWord)
                        population++;
                }
                else
                    population = Raw.Population();

                return population;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "0x{0:X}", Raw);
        }
    }
}
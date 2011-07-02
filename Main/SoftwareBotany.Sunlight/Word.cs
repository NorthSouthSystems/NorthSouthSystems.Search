using System;
using System.Diagnostics.Contracts;

namespace SoftwareBotany.Sunlight
{
    internal struct Word
    {
        public const int SIZE = 32;

        #region Construction

        public Word(uint raw)
        {
            if (raw >= COMPRESSEDMASK)
                throw new ArgumentOutOfRangeException(string.Format("value must be less than COMPRESSEDMASK : 0x{0:X}.", COMPRESSEDMASK), "value");

            Contract.EndContractBlock();

            Raw = raw;
        }

        public Word(bool fillBit, int fillCount)
        {
            if (fillCount < 0)
                throw new ArgumentOutOfRangeException("fillCount must be greater than or equal to 0.", "fillCount");

            if (fillCount > FILLCOUNTMASK)
                throw new ArgumentOutOfRangeException(string.Format("fillCount must be less than or equal to COMPRESSEDFILLCOUNTMASK : 0x{0:X}.", FILLCOUNTMASK), "fillCount");

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
                throw new NotSupportedException("Indexer bit get/set not supported for Compressed Words.");

            if (position < 0)
                throw new ArgumentOutOfRangeException("position must be greater than or equal to 0.", "position");

            if (position >= SIZE - 1)
                throw new ArgumentOutOfRangeException(string.Format("position must be less than SIZE - 1 : {0}.", SIZE - 1), "position");

            Contract.EndContractBlock();

            return 1u << (30 - position);
        }

        public bool[] Bits
        {
            get
            {
                if (IsCompressed)
                    throw new NotSupportedException("Bits not supported for Compressed Words.");

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

        public int[] GetBitPositions(bool value)
        {
            if (IsCompressed)
                throw new NotSupportedException("GetBitPositions not supported for Compressed Words.");

            Contract.EndContractBlock();

            if ((value && Raw == 0u) || (!value && Raw == COMPRESSIBLEMASK))
                return EmptyBitPositions;

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

        public static readonly int[] EmptyBitPositions = new int[0];

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

#if POSITIONLISTENABLED
        private const uint PACKEDPOSITIONMASK = 0x3E000000u;

        public bool HasPackedWord { get { return (Raw & PACKEDPOSITIONMASK) > 0u; } }

        /// <summary>
        /// Position is relative to the global bit position.  More specifically, it ignores the most significant bit which is
        /// reserved for compression.  E.g. for a given word's most significant byte, X012 3456.  This follows the same convention
        /// as every Word and Vector function.  There are SIZE - 1 bit positions in a uncompressed Word using this convention,
        /// and therefore bit positions go from 0 to 30.  Because 5 bits represent the PACKEDPOSITIONMASK, values 0 through 31 can be
        /// represented.  Because 00000 = 0 should mean the absence of a PackedWord, we take the value stored under the
        /// PACKEDPOSITIONMASK and subtract 1 to get the real PackedPosition. This function will then return -1 should a PackedWord
        /// not be present. This offset is reflected in the PackedWord property and also the Pack function.
        /// </summary>
        public int PackedPosition { get { return (int)((Raw & PACKEDPOSITIONMASK) >> (SIZE - 7)) - 1; } }

        public Word PackedWord { get { return new Word(1u << (SIZE - 2 - PackedPosition)); } }

        internal void Pack(Word wahWord)
        {
            if (!IsCompressed)
                throw new NotSupportedException("Cannot Pack a Word into an Uncompessed Word.");

            if (wahWord.IsCompressed)
                throw new NotSupportedException("Cannot Pack a Compressed Word.");

            Contract.EndContractBlock();

            uint packedPosition = (uint)wahWord.GetBitPositions(true)[0];
            Raw |= (packedPosition + 1) << (SIZE - 7);
        }

        internal static readonly bool PositionListEnabled = true;
#else
        internal static readonly bool PositionListEnabled = false;
#endif

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

#if POSITIONLISTENABLED
                    if (HasPackedWord)
                        population++;
#endif
                }
                else
                    population = Census.ComputePopulation(Raw);

                return population;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("0x{0:X}", Raw);
        }
    }
}
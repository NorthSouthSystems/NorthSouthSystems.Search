using System;

namespace SoftwareBotany.Sunlight
{
    /// <summary>
    /// Provides a method for computing the population (number of bits set to 1) of an
    /// unsigned integer.
    /// </summary>
    public unsafe static class Census
    {
        static Census()
        {
            _bytePopulations[0] = 0;

            for (int i = 0; i < 256; i++)
                _bytePopulations[i] = (byte)((i & 1) + _bytePopulations[i / 2]);
        }

        /// <summary>
        /// Compute the number of bits set to 1 for an unsigned integer value.
        /// </summary>
        public static int ComputePopulation(uint word)
        {
            byte* w = (byte*)&word;
            return _bytePopulations[w[0]] + _bytePopulations[w[1]] + _bytePopulations[w[2]] + _bytePopulations[w[3]];
        }

        private static readonly byte[] _bytePopulations = new byte[256];
    }
}
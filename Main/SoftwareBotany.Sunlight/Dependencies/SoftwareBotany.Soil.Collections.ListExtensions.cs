using System;
using System.Collections.Generic;

namespace SoftwareBotany.Sunlight
{
    /// <summary>
    /// This class has been copied verbatim from SoftwareBotany.Soil.  It is the only method from Soil used in the
    /// Sunlight project, and this copy paste reuse has been done in order to eliminate the dependency.
    /// Unused methods from the class have been removed.
    /// </summary>
    internal static class ListExtensions
    {
        /// <summary>
        /// Follows the same patterns as Array.BinarySearch and List(Of T).BinarySearch.
        /// </summary>
        public static int BinarySearch<T>(this IList<T> list, T value)
          where T : IComparable<T>
        {
            if (list == null)
                throw new ArgumentNullException("list");

            int i = 0;
            int j = list.Count - 1;

            while (i <= j)
            {
                int k = i + ((j - i) >> 1);

                int result = list[k].CompareTo(value);

                if (result == 0)
                    return k;

                if (result < 0)
                    i = k + 1;
                else
                    j = k - 1;
            }

            return ~i;
        }
    }
}
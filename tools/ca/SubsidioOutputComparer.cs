using System.Collections.Generic;

namespace Kontecg
{
    /// <summary>
    /// Provides a comparer for <see cref="SubsidioOutputDto"/> objects based on the employee badge number (Chapa).
    /// </summary>
    public class SubsidioOutputComparer : IComparer<SubsidioOutputDto>
    {
        /// <summary>
        /// Compares two <see cref="SubsidioOutputDto"/> objects by their Chapa property.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A value less than zero if x is less than y, zero if x equals y, or greater than zero if x is greater than y.
        /// </returns>
        public int Compare(SubsidioOutputDto x, SubsidioOutputDto y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(null, y))
            {
                return 1;
            }

            if (ReferenceEquals(null, x))
            {
                return -1;
            }

            if(int.Parse(x.Chapa) < int.Parse(y.Chapa)) return -1;
            if (int.Parse(x.Chapa) == int.Parse(y.Chapa)) return 0;
            return 1;
        }
    }
}

using System.Collections.Generic;

namespace Kontecg.Organizations
{
    public class ComplexityGroupComparer : IComparer<ComplexityGroup>
    {
        public static readonly ComplexityGroupComparer Instance = new();

        private readonly Dictionary<char, int> _romanMap = new()
        {
            {'I', 1},
            {'V', 5},
            {'X', 10},
            {'L', 50},
            {'C', 100},
            {'D', 500},
            {'M', 1000}
        };

        private int RomanToDecimal(string romanNumber)
        {
            int result = 0;
            for (int i = 0; i < romanNumber.Length; i++)
            {
                if (i + 1 < romanNumber.Length && _romanMap[romanNumber[i]] < _romanMap[romanNumber[i + 1]])
                    result -= _romanMap[romanNumber[i]];
                else
                    result += _romanMap[romanNumber[i]];
            }

            return result;
        }

        public int Compare(ComplexityGroup x, ComplexityGroup y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (ReferenceEquals(null, y))
                return 1;

            if (ReferenceEquals(null, x))
                return -1;

            if(RomanToDecimal(x.Group.ToUpperInvariant()) < RomanToDecimal(y.Group.ToUpperInvariant()))
                return -1;

            return RomanToDecimal(x.Group.ToUpperInvariant()) > RomanToDecimal(y.Group.ToUpperInvariant()) ? 1 : x.BaseSalary.CompareTo(y.BaseSalary);
        }
    }
}

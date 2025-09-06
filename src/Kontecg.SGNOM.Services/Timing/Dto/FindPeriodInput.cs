using System;
using Itenso.TimePeriod;

namespace Kontecg.Timing.Dto
{
    public class FindPeriodInput
    {
        public int? Year { get; set; }

        public YearMonth? Month { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }
    }
}

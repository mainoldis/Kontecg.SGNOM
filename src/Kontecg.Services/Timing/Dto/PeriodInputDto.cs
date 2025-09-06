using System;
using System.ComponentModel.DataAnnotations;
using Itenso.TimePeriod;

namespace Kontecg.Timing.Dto
{
    public class PeriodInputDto
    {
        public int? Year { get; set; }

        public YearMonth? Month { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        [Required]
        public string ModuleKey { get; set; }
    }
}

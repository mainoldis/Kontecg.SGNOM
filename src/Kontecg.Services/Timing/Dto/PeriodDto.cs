using System;

namespace Kontecg.Timing.Dto
{
    public class PeriodDto
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public TimeSpan Duration { get; set; }
    }
}

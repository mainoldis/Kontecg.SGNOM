using System.Collections.Generic;
using System;

namespace Kontecg.Timing.Dto
{
    public class WorkingHoursDto
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan FreeTime { get; set; }

        public List<CalendarTimeDecorationDto> Decorators { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Itenso.TimePeriod;

namespace Kontecg.Timing
{
    public class WorkingHours : TimeRange
    {
        public WorkingHours()
        {
            FreeTime = TimeSpan.Zero;
        }

        public WorkingHours(DateTime moment, TimeSpan? freeTime = null)
            : base(moment)
        {
            FreeTime = freeTime ?? TimeSpan.Zero;
        }

        public WorkingHours(DateTime start, DateTime end, TimeSpan? freeTime = null)
            : base(start, end)
        {
            FreeTime = freeTime ?? TimeSpan.Zero;
        }

        public WorkingHours(DateTime start, TimeSpan duration, TimeSpan? freeTime = null)
            : base(start, duration)
        {
            FreeTime = freeTime ?? TimeSpan.Zero;
        }

        public TimeSpan FreeTime { get; set; }

        public List<CalendarTimeDecoration> Decorators { get; } = new();
    }
}

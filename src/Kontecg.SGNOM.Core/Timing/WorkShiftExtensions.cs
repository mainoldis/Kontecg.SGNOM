using System;
using Itenso.TimePeriod;

namespace Kontecg.Timing
{
    public static class WorkShiftExtensions
    {
        public static WorkPattern ToWorkPattern(this WorkShift workShift)
        {
            return workShift is null
                ? throw new ArgumentNullException(nameof(workShift))
                : new WorkPattern(workShift.Regime.DaysScheduling, workShift.Regime.TimeScheduling,
                    workShift.StartDate, workShift.HoursWorking, workShift.RestingTimesPerShift,
                    WorkCalendarTool.New(), workShift.Regime.SpecialGroup);
        }

        public static WorkPattern ToWorkPattern(this WorkShift workShift, ITimeCalendar calendar)
        {
            return workShift is null
                ? throw new ArgumentNullException(nameof(workShift))
                : new WorkPattern(workShift.Regime.DaysScheduling, workShift.Regime.TimeScheduling,
                    workShift.StartDate, workShift.HoursWorking, workShift.RestingTimesPerShift,
                    calendar, workShift.Regime.SpecialGroup);
        }

        public static WorkYear ToSchedule(this WorkShift workShift, ITimeCalendar calendar, DateTime? since = null)
        {
            return workShift is null
                ? throw new ArgumentNullException(nameof(workShift))
                : new WorkYear(since ?? Clock.Now, workShift.ToWorkPattern(calendar));
        }
    }
}

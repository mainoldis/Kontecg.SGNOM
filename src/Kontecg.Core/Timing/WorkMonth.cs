using System;
using System.Collections.Generic;
using Itenso.TimePeriod;
using Kontecg.Primitives;

namespace Kontecg.Timing
{
    public class WorkMonth : CalendarTimeRange
    {
        public WorkMonth() :
            this(WorkPattern.Default)
        {
        }

        public WorkMonth(WorkPattern pattern) :
            this(Clock.Now, pattern)
        {
        }

        public WorkMonth(DateTime moment, WorkPattern pattern) :
            this(GetYearOf(moment), GetMonthOf(moment), pattern)
        {
        }

        public WorkMonth(int year, YearMonth month, WorkPattern pattern) :
            base(GetPeriodOf(year, month, pattern.Calendar), pattern.Calendar)
        {
            Year = year;
            Month = month;
            Pattern = pattern;
        }

        public YearMonth Month { get; }

        public int Year { get; }

        public WorkPattern Pattern { get; }

        public Dictionary<DateTime, CalendarTimeDecorator> Decorators { get; } = new();

        public ITimePeriodCollection WorkingPeriods => GetWorkingTimeRange();

        public ITimePeriodCollection GetDays()
        {
            var days = new TimePeriodCollection();
            var moment = Start.Date;
            while (moment < End.Date)
            {
                if (!Decorators.TryGetValue(moment, out var decorator))
                    decorator = CalendarTimeDecorator.None;
                days.Add(new WorkDay(moment.Date, Pattern, decorator));
                moment = moment.AddDays(1);
            }

            return days;
        }

        public WorkMonth GetPreviousMonth()
        {
            return AddMonths(-1);
        }

        public WorkMonth GetNextMonth()
        {
            return AddMonths(1);
        }

        public WorkMonth AddMonths(int months)
        {
            WorkCalendarTool.AddMonths(Year, Month, months, out int targetYear, out YearMonth targetMonth);
            return new WorkMonth(targetYear, targetMonth, Pattern);
        }

        protected override string Format(ITimeFormatter formatter)
        {
            return formatter.GetCalendarPeriod($"{Year}/{Month}",
                formatter.GetShortDate(Start), formatter.GetShortDate(End), Duration);
        }

        public WorkingHours GetNextWorkingTimeRange(ITimePeriod period)
        {
            var workingTimeRange = WorkingPeriods;

            int indexOf = workingTimeRange.IndexOf(period);
            if (indexOf == -1 || WorkingPeriods.Count == 0)
                return null;

            if (indexOf == WorkingPeriods.Count - 1)
                return GetNextMonth().WorkingPeriods[0] as WorkingHours;

            return WorkingPeriods[indexOf + 1] as WorkingHours;
        }

        private ITimePeriodCollection GetWorkingTimeRange()
        {
            TimePeriodCollection cycles = new();
            if (!Pattern.IsValidPattern) return cycles;
            //LTTTTDDDD TTTTDDDD TTTTDDDD TTTTDDDD TTTTDDD
            var rtdArray = Pattern.RegimenWorkRest.ToUpper().ToCharArray();
            int cycleLength = rtdArray.Length > 0 ? rtdArray.Length : 1;
            var moment = Start.Date.AddDays(-1);
            if (Pattern.StartingTimeRotation > moment) moment = Pattern.StartingTimeRotation;
            int startingDayOnCycle = CalculateDayOnCycle(Pattern.StartingTimeRotation, moment, cycleLength);
            int day = 1;
            while (moment < End.Date)
            {
                //startingStepOnCycle = 1446 % 40 = 6
                //dayOfCycle = (1 + 6 - 1) % 40
                var indexOnCycle = (day + startingDayOnCycle - 2) % cycleLength;
                if (rtdArray[indexOnCycle] == 'T' || rtdArray[indexOnCycle] == 'L')
                {
                    var hourStarting = !Pattern.HoursRangePerShift[indexOnCycle].Night
                        ? Pattern.HoursRangePerShift[indexOnCycle].Start
                        : Pattern.HoursRangePerShift[indexOnCycle].End;
                    var start = TimeTool.SetTimeOfDay(moment.Date, hourStarting.Hour, hourStarting.Minute);
                    var workingHours = new WorkingHours(start, start + Pattern.WorkingTimePerRTD[indexOnCycle], Pattern.FreeTime);

                    if (workingHours.Start < Start)
                        workingHours.ShrinkStartTo(Start);

                    if (workingHours.End > End)
                        workingHours.ShrinkEndTo(End);

                    ITimeRange intersection;
                    var decoration = GetDecoration(moment.Date);
                    if (decoration != CalendarTimeDecorator.None)
                    {
                        intersection = workingHours.GetIntersection(new Day(moment, Pattern.Calendar));
                        if (intersection != null && intersection.Duration > TimeSpan.Zero)
                            workingHours.Decorators.Add(new CalendarTimeDecoration(intersection, decoration));
                    }

                    if (!TimeCompare.IsSameDay(moment.Date, workingHours.End) && GetDecoration(workingHours.End.Date) != CalendarTimeDecorator.None)
                    {
                        decoration = GetDecoration(workingHours.End.Date);
                        intersection = workingHours.GetIntersection(new Day(workingHours.End.Date, Pattern.Calendar));
                        if (intersection != null && intersection.Duration > TimeSpan.Zero)
                            workingHours.Decorators.Add(new CalendarTimeDecoration(intersection, decoration));
                    }

                    if (rtdArray[indexOnCycle] == 'L')
                    {
                        intersection = workingHours.GetIntersection(new TimeRange(TimeTool.SetTimeOfDay(moment.Date, 7, 0), TimeSpan.FromHours(12)));
                        if (intersection != null && intersection.Duration > TimeSpan.Zero) workingHours.Decorators.Add(new CalendarTimeDecoration(intersection, CalendarTimeDecorator.CrazyWorkShiftTime, Priority.High));
                    }

                    intersection = workingHours.GetIntersection(new TimeRange(TimeTool.SetTimeOfDay(moment.Date, 19, 0), TimeSpan.FromHours(4)));
                    if (intersection != null && intersection.Duration > TimeSpan.Zero) workingHours.Decorators.Add(new CalendarTimeDecoration(intersection, CalendarTimeDecorator.EarlyNightTime));

                    intersection = workingHours.GetIntersection(new TimeRange(TimeTool.SetTimeOfDay(moment.Date, 23, 0), TimeSpan.FromHours(8)));
                    if (intersection != null && intersection.Duration > TimeSpan.Zero) workingHours.Decorators.Add(new CalendarTimeDecoration(intersection, CalendarTimeDecorator.LateNightTime));

                    if (workingHours.IntersectsWith(this))
                        cycles.Add(workingHours);
                }
                moment = moment.AddDays(1);
                day++;
            }

            return cycles;
        }

        private int CalculateDayOnCycle(DateTime date1, DateTime date2, int lengthOfCycle)
        {
            var start = date2 > date1 ? date1 : date2;
            var end = date2 > date1 ? date2 : date1;

            var dateDiff = new DateDiff(start, end);
            return (dateDiff.Days) % lengthOfCycle + 1;
        }

        private CalendarTimeDecorator GetDecoration(DateTime moment)
        {
            if (!Decorators.TryGetValue(moment, out var decorator)) decorator = CalendarTimeDecorator.None;
            return decorator;
        }

        public Dictionary<CalendarTimeDecorator, TimeSpan> SummaryTime => SummarizeDecorations();

        private Dictionary<CalendarTimeDecorator, TimeSpan> SummarizeDecorations()
        {
            var summary = WorkingPeriods.Summarize();
            return summary;
        }

        private static int GetYearOf(DateTime moment)
        {
            WorkCalendarTool.GetYearOf(moment, out int year);
            return year;
        }

        private static YearMonth GetMonthOf(DateTime moment)
        {
            WorkCalendarTool.GetMonthOf(moment, out int _, out YearMonth month);
            return month;
        }

        private static ITimeRange GetPeriodOf(int year, YearMonth month, ITimeCalendar calendar)
        {
            var start = WorkCalendarTool.GetStartOfMonth(year, month);
            if (month == YearMonth.December)
            {
                year++;
                month = YearMonth.January;
            }
            else
                month++;

            var end = WorkCalendarTool.GetStartOfMonth(year, month);
            return new CalendarTimeRange(start, end, calendar);
        }
    }
}

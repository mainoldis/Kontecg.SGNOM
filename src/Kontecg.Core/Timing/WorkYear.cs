using System;
using System.Collections.Generic;
using System.Globalization;
using Itenso.TimePeriod;
using Kontecg.Primitives;

namespace Kontecg.Timing
{
    public class WorkYear : CalendarTimeRange
    {
        public WorkYear(DateTime moment, WorkPattern pattern)
            : this(GetYearOf(moment), pattern)
        {
        }

        public WorkYear(int year, WorkPattern pattern)
            : base(GetPeriodOf(year, pattern.Calendar), pattern.Calendar)
        {
            Year = year;
            Pattern = pattern;
            FillDecorations();
        }

        public Dictionary<DateTime, CalendarTimeDecorator> Decorators { get; } = new();

        public WorkPattern Pattern { get; }

        public int Year { get; }

        public ITimePeriodCollection WorkingPeriods => GetWorkingTimeRange();

        public void AddDecorator(YearMonth month, int day, DayDecorator decorator)
        {
            AddDecorator((int) month, day, decorator);
        }

        public void AddDecorator(int month, int day, DayDecorator decorator)
        {
            AddDecorator(new DateTime(Year, month, day), decorator);
        }

        public void AddDecorator(DateTime moment, DayDecorator decorator)
        {
            if (!Decorators.ContainsKey(new DateTime(moment.Year, moment.Month, moment.Day)))
                Decorators.Add(new DateTime(moment.Year, moment.Month, moment.Day), decorator.ToCalendarTimeDecorator());
        }

        public void AddAlternateSaturdays(DateTime firstSaturday)
        {
            if (firstSaturday.DayOfWeek != DayOfWeek.Saturday) return;

            var current = firstSaturday;
            AddDecorator(current, DayDecorator.BreakSaturday);

            while (current.Date >= Start)
            {
                AddDecorator(current, DayDecorator.BreakSaturday);
                current = current.AddDays(-14);
            }

            while (current.Date <= End)
            {
                AddDecorator(current, DayDecorator.BreakSaturday);
                current = current.AddDays(14);
            }
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
                    if(intersection != null && intersection.Duration > TimeSpan.Zero) workingHours.Decorators.Add(new CalendarTimeDecoration(intersection, CalendarTimeDecorator.EarlyNightTime));

                    intersection = workingHours.GetIntersection(new TimeRange(TimeTool.SetTimeOfDay(moment.Date, 23, 0), TimeSpan.FromHours(8)));
                    if (intersection != null && intersection.Duration > TimeSpan.Zero) workingHours.Decorators.Add(new CalendarTimeDecoration(intersection, CalendarTimeDecorator.LateNightTime));

                    if(workingHours.IntersectsWith(this))
                        cycles.Add(workingHours);
                }
                moment = moment.AddDays(1);
                day++;
            }

            return cycles;
        }

        private void FillDecorations()
        {
            Decorators.Clear();
            Decorators.Add(new DateTime(Year - 1, 12, 31), CalendarTimeDecorator.NationalHolidayTime);
            Decorators.Add(new DateTime(Year, 1, 1), CalendarTimeDecorator.NationalCelebrationDayTime);
            Decorators.Add(new DateTime(Year, 1, 2), CalendarTimeDecorator.NationalHolidayTime);
            //Decorators.Add(GetEasterFriday(), CalendarTimeDecorator.BreakDay);
            Decorators.Add(new DateTime(Year, 5, 1), CalendarTimeDecorator.NationalCelebrationDayTime);
            //RULE: Artículo 97 Ley 116 - Código del trabajo
            if (new DateTime(Year, 5, 1).DayOfWeek == DayOfWeek.Sunday)
                Decorators.Add(new DateTime(Year, 5, 2), CalendarTimeDecorator.BreakTime);
            Decorators.Add(new DateTime(Year, 7, 25), CalendarTimeDecorator.NationalHolidayTime);
            Decorators.Add(new DateTime(Year, 7, 26), CalendarTimeDecorator.NationalCelebrationDayTime);
            Decorators.Add(new DateTime(Year, 7, 27), CalendarTimeDecorator.NationalHolidayTime);
            Decorators.Add(new DateTime(Year, 10, 10), CalendarTimeDecorator.NationalCelebrationDayTime);
            //RULE: Artículo 97 Ley 116 - Código del trabajo
            if (new DateTime(Year, 10, 10).DayOfWeek == DayOfWeek.Sunday)
                Decorators.Add(new DateTime(Year, 10, 11), CalendarTimeDecorator.BreakTime);
            Decorators.Add(new DateTime(Year, 12, 25), CalendarTimeDecorator.NationalHolidayTime);
            Decorators.Add(new DateTime(Year, 12, 31), CalendarTimeDecorator.NationalHolidayTime);
            Decorators.Add(new DateTime(Year + 1, 1, 1), CalendarTimeDecorator.NationalCelebrationDayTime);
        }

        private DateTime GetEasterFriday()
        {
            var start = GetEasterDay(Year);
            while (start.DayOfWeek != DayOfWeek.Friday)
                start = start.AddDays(-1);
            return start;
        }

        private DateTime GetEasterDay(int year)
        {
            int a = year % 19;
            int b = year % 4;
            int c = year % 7;
            int d = (19 * a + 24) % 30;
            int e = (2 * b + 4 * c + 6 * d + 5) % 7;
            int f = d + e - 9;
            int day;
            int month;

            if (f < 0)
            {
                day = f + 31;
                month = 3;
            }
            else
            {
                day = f + 1;
                month = 4;
            }
            return new DateTime(year, month, day + 13);
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

        public WorkYear GetPreviousYear()
        {
            return AddYears(-1);
        }

        public WorkYear GetNextYear()
        {
            return AddYears(1);
        }

        public WorkYear AddYears(int count)
        {
            return new WorkYear(Year + count, Pattern);
        }

        public ITimePeriodCollection GetMonths()
        {
            var months = new TimePeriodCollection();
            for (var month = 1; month <= TimeSpec.MonthsPerYear; month++)
            {
                var m = new WorkMonth(Year, (YearMonth) month, Pattern);
                foreach (KeyValuePair<DateTime, CalendarTimeDecorator> decorator in Decorators)
                    m.Decorators.Add(decorator.Key, decorator.Value);
                months.Add(m);
            }
            return months;
        }

        public WorkMonth GetMonth(YearMonth month)
        {
            var m = new WorkMonth(Year, month, Pattern);
            foreach (KeyValuePair<DateTime, CalendarTimeDecorator> decorator in Decorators)
                m.Decorators.Add(decorator.Key, decorator.Value);
            return m;
        }

        public WorkingHours GetNextWorkingTimeRange(ITimePeriod period)
        {
            var workingTimeRange = WorkingPeriods;

            int indexOf = workingTimeRange.IndexOf(period);
            if (indexOf == -1 || WorkingPeriods.Count == 0)
                return null;

            if (indexOf == WorkingPeriods.Count - 1)
                return GetNextYear().WorkingPeriods[0] as WorkingHours;

            return WorkingPeriods[indexOf + 1] as WorkingHours;
        }

        private int CalculateDayOnCycle(DateTime date1, DateTime date2, int lengthOfCycle)
        {
            var start = date2 > date1 ? date1 : date2;
            var end = date2 > date1 ? date2 : date1;

            var dateDiff = new DateDiff(start, end);
            return (dateDiff.Days) % lengthOfCycle + 1;
        }

        private static int GetYearOf(DateTime moment)
        {
            WorkCalendarTool.GetYearOf(moment, out int year);
            return year;
        }

        private static ITimeRange GetPeriodOf(int year, ITimeCalendar calendar)
        {
            var start = WorkCalendarTool.GetStartOfYear(year);
            var end = WorkCalendarTool.GetStartOfYear(year+1);
            return new CalendarTimeRange(start, end, calendar);
        }

        protected override string Format(ITimeFormatter formatter)
        {
            return formatter.GetCalendarPeriod(Year.ToString(CultureInfo.InvariantCulture),
                formatter.GetShortDate(Start), formatter.GetShortDate(End), Duration);
        }
    }
}

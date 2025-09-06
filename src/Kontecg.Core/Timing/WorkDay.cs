using System;
using Itenso.TimePeriod;
using Kontecg.Primitives;

namespace Kontecg.Timing
{
    public class WorkDay : DayTimeRange
    {
        public WorkDay() :
            this(WorkPattern.Default)
        {
        }

        public WorkDay(WorkPattern pattern, CalendarTimeDecorator decorator = CalendarTimeDecorator.None) :
            this(Clock.Now, pattern, decorator)
        {
        }

        public WorkDay(DateTime moment, WorkPattern pattern, CalendarTimeDecorator decorator = CalendarTimeDecorator.None) :
            this(pattern.Calendar.GetYear(moment), pattern.Calendar.GetMonth(moment), pattern.Calendar.GetDayOfMonth(moment), pattern, decorator)
        {
        }

        public WorkDay(int year, int month, WorkPattern pattern, CalendarTimeDecorator decorator) :
            this(year, month, 1, pattern, decorator)
        {
        }

        public WorkDay(int year, int month, int day, WorkPattern pattern, CalendarTimeDecorator decorator) :
            base(year, month, day, 1, pattern.Calendar)
        {
            Pattern = pattern;
            Decorator = decorator;
        }

        public int Year => StartYear;

        public int Month => StartMonth;

        public int Day => StartDay;

        public DayOfWeek DayOfWeek => StartDayOfWeek;

        public string DayName => StartDayName;

        public WorkPattern Pattern { get; }

        public CalendarTimeDecorator Decorator { get; }

        public ITimePeriodCollection WorkingPeriods => GetWorkingTimeRange();

        public WorkDay GetPreviousDay()
        {
            return AddDays(-1);
        }

        public WorkDay GetNextDay()
        {
            return AddDays(1);
        }

        public WorkDay AddDays(int days)
        {
            var startDay = new DateTime(StartYear, StartMonth, StartDay);
            return new WorkDay(startDay.AddDays(days), Pattern);
        }

        protected override string Format(ITimeFormatter formatter)
        {
            return formatter.GetCalendarPeriod(DayName,
                formatter.GetShortDate(Start), formatter.GetShortDate(End), Duration);
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
                    if (Decorator != CalendarTimeDecorator.None)
                    {
                        intersection = workingHours.GetIntersection(this);
                        if (intersection != null && intersection.Duration > TimeSpan.Zero)
                            workingHours.Decorators.Add(new CalendarTimeDecoration(intersection, Decorator));
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
    }
}

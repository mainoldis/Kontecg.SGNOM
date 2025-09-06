using System;
using Itenso.TimePeriod;
using Kontecg.Validation;

namespace Kontecg.Timing
{
    public static class WorkCalendarTool
    {
        #region Calendar

        public static TimeCalendar New()
        {
            TimeCalendarConfig config = new()
            {
                YearType = YearType.CalendarYear,
                DayNameType = CalendarNameType.Abbreviated,
                MonthNameType = CalendarNameType.Abbreviated,
                StartOffset = TimeSpan.Zero,
                EndOffset = TimeSpan.Zero
            };

            return new TimeCalendar(config);
        }

        #endregion

        #region Time

        public static TimeSpan GetDurationOf(string durationPattern)
        {
            if (!ValidationHelper.IsTime(durationPattern)) return TimeSpan.Zero;
            var strings = durationPattern.Split(':');
            return Duration.Hours(int.Parse(strings[0]), int.Parse(strings[1]));
        }

        #endregion

        #region Month

        public static void AddMonths(int startYear, YearMonth startMonth, int addMonths, out int targetYear,
            out YearMonth targetMonth)
        {
            var totalMonths = startYear * TimeSpec.MonthsPerYear + ((int)startMonth - 1) + addMonths;
            targetYear = totalMonths / TimeSpec.MonthsPerYear;
            targetMonth = (YearMonth)(totalMonths % TimeSpec.MonthsPerYear + 1);
        }

        public static void GetMonthOf(DateTime moment, out int year, out YearMonth month)
        {
            var currentYear = moment.Year;
            var currentMonth = YearMonth.January;
            do
            {
                AddMonths(currentYear, currentMonth, 1, out currentYear, out currentMonth);
                if (GetStartOfMonth(currentYear, currentMonth) > moment)
                {
                    AddMonths(currentYear, currentMonth, -1, out year, out month);
                    break;
                }
            } while (true);
        }

        public static DateTime GetStartOfMonth(DateTime moment)
        {
            GetMonthOf(moment, out var year, out var month);
            return GetStartOfMonth(year, month);
        }

        public static DateTime GetStartOfMonth(int year, YearMonth month)
        {
            return new DateTime(year, (int)month, 1);
        }

        public static DateTime GetStartOfLastSixMonths(DateTime moment)
        {
            GetMonthOf(moment, out var currentYear, out var currentMonth);
            AddMonths(currentYear, currentMonth, -6, out var year, out var month);
            return GetStartOfMonth(year, month);
        }

        public static DateTime GetStartOfLastTwelveMonths(DateTime moment)
        {
            GetMonthOf(moment, out var currentYear, out var currentMonth);
            AddMonths(currentYear, currentMonth, -12, out var year, out var month);
            return GetStartOfMonth(year, month);
        }

        #endregion

        #region Year

        public static void GetYearOf(DateTime moment, out int year)
        {
            year = moment.Year;

            while (GetStartOfYear(year + 1) <= moment) year++;
        }

        public static DateTime GetStartOfYear(int year)
        {
            return GetStartOfMonth(year, YearMonth.January);
        }

        #endregion
    }
}

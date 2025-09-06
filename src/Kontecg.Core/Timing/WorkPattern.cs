using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Itenso.TimePeriod;
using Kontecg.Extensions;

namespace Kontecg.Timing
{
    public class WorkPattern
    {
        public static WorkPattern Default = new();

        private const string RtdPattern = @"^(\d{1,2})\*(\d{1,2})|(\d{1,2})\*(\d{1,2})";

        /// <summary>
        /// 5*2-6*1
        /// </summary>
        public string RegimenWorkRest { get; }

        /// <summary>
        /// 5*2-6*1
        /// </summary>

        /// <summary>
        /// 08:00*08:00*08:00*08:00*08:00-08:00*08:00*08:00*08:00*08:00*08:00
        /// </summary>
        public TimeSpan[] WorkingTimePerRTD { get; }

        /// <summary>
        /// 2023-02-08
        /// </summary>
        public DateTime StartingTimeRotation { get; }

        /// <summary>
        /// [08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[08:00-17:00]-[08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[08:00-17:00]...
        /// </summary>
        public WorkHourRange[] HoursRangePerShift { get; }

        /// <summary>
        /// 00:30
        /// </summary>
        public TimeSpan FreeTime { get; }

        /// <summary>
        /// Base calendar for calculus
        /// </summary>
        public ITimeCalendar Calendar { get; }

        public WorkPattern(string regimenWorkRest, string workingTimePerRtd, DateTime startingRotation,
            string hoursRangePerShift, string freeTime, ITimeCalendar calendar, string specialGroup = null)
        {
            Check.NotNullOrWhiteSpace(regimenWorkRest, nameof(regimenWorkRest));
            Check.NotNullOrWhiteSpace(workingTimePerRtd, nameof(workingTimePerRtd));

            RegimenWorkRest = GetPattern(regimenWorkRest, specialGroup);
            WorkingTimePerRTD = GetWorkingTimePerRTD(workingTimePerRtd);
            HoursRangePerShift = GetHourRangesPerShift(hoursRangePerShift);
            FreeTime = GetDurationFromPattern(freeTime);
            StartingTimeRotation = startingRotation;
            Calendar = calendar;
        }

        public void SetDefault(string regimenWorkRest, string workingTimePerRtd, DateTime startingRotation,
            string hoursRangePerShift, string freeTime, ITimeCalendar calendar, string specialGroup = null)
        {
            Default = new WorkPattern(regimenWorkRest, workingTimePerRtd, startingRotation,
                hoursRangePerShift, freeTime, calendar, specialGroup);
        }

        private WorkPattern()
        {
            Calendar = WorkCalendarTool.New();
            RegimenWorkRest = GetPattern("5*2-6*1");
            WorkingTimePerRTD = GetWorkingTimePerRTD("08:00*08:00*08:00*08:00*08:00*00:00*00:00-08:00*08:00*08:00*08:00*08:00*04:00*00:00");
            HoursRangePerShift = GetHourRangesPerShift("[08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[00:00-00:00]*[00:00-00:00]-[08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[08:00-17:00]*[08:00-13:00]*[00:00-00:00]");
            FreeTime = Duration.Hour;
            StartingTimeRotation = new(1999, 12, 27);
            while (StartingTimeRotation.DayOfWeek != Calendar.FirstDayOfWeek)
                StartingTimeRotation = StartingTimeRotation.AddDays(-1);
        }

        public bool IsValidPattern
        {
            get
            {
                var workdays = RegimenWorkRest.ToUpper().ToCharArray().Length;
                return workdays == WorkingTimePerRTD.Length && workdays == HoursRangePerShift.Length;
            }
        }

        private string GetPattern(string pattern, string specialGroup = null)
        {
            Regex rtdRegex = new(RtdPattern);
            string rtd = string.Empty;
            var rtdCycles = rtdRegex.Matches(pattern);
            foreach (Match rtdMatch in rtdCycles)
            {
                var workDaysToGen = int.Parse(rtdMatch.Value.Split('*')[0]);
                var restingDaysToGen = int.Parse(rtdMatch.Value.Split('*')[1]);

                for (int i = 0; i < workDaysToGen; i++)
                    rtd += rtdMatch.Value == specialGroup && i == 0 ? 'L' : 'T';

                for (int i = 0; i < restingDaysToGen; i++)
                    rtd += 'D';
            }
            return rtd;
        }

        private TimeSpan GetDurationFromPattern(string pattern)
        {
            if(pattern.IsNullOrWhiteSpace()) return TimeSpan.Zero;

            Regex timeRegex = new("([01]?[0-9]|2[0-3]):[0-5][0-9]");
            var matches = timeRegex.Matches(pattern);
            return matches.Count == 0 ? TimeSpan.Zero : WorkCalendarTool.GetDurationOf(matches[0].Value);
        }

        private TimeSpan[] GetWorkingTimePerRTD(string pattern)
        {
            if (pattern.IsNullOrWhiteSpace()) return new[] {TimeSpan.Zero};
            Regex timeRegex = new("([01]?[0-9]|2[0-3]):[0-5][0-9]");
            var matches = timeRegex.Matches(pattern);
            List<TimeSpan> durationList = new();
            foreach (Match match in matches) durationList.Add(WorkCalendarTool.GetDurationOf(match.Value));
            return durationList.ToArray();
        }

        private WorkHourRange[] GetHourRangesPerShift(string pattern)
        {
            if (pattern.IsNullOrWhiteSpace()) return null;
            List<WorkHourRange> ranges = new();
            Regex timeRangesRegex = new("([01]?[0-9]|2[0-3]):[0-5][0-9]-([01]?[0-9]|2[0-3]):[0-5][0-9]");
            Regex timeRegex = new("([01]?[0-9]|2[0-3]):[0-5][0-9]");
            var matches = timeRangesRegex.Matches(pattern);
            try
            {
                foreach (Match match in matches)
                {
                    var rangeMatches = timeRegex.Matches(match.Value);
                    WorkHourRange hourRange = new WorkHourRange(
                        new Time(WorkCalendarTool.GetDurationOf(rangeMatches[0].Value)),
                        new Time(WorkCalendarTool.GetDurationOf(rangeMatches[1].Value)));
                    ranges.Add(hourRange);
                }
            }
            catch
            {
                // ignored
            }

            return ranges.ToArray();
        }
    }
}

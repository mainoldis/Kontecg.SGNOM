using System;
using Itenso.TimePeriod;
using Kontecg.Extensions;

namespace Kontecg.Timing
{
    [Serializable]
    public class PeriodInfo
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public YearMonth Month { get; set; }

        public YearQuarter Quarter { get; set; }

        public DateTime Since { get; set; }

        public DateTime Until { get; set; }

        public PeriodStatus Status { get; set; }

        public int CompanyId { get; set; }

        public string ReferenceGroup { get; set; }

        public ITimeCalendar Calendar { get; set; }

        public string ToIdentifier()
        {
            return $"{ReferenceGroup}@{CompanyId}:{Id}";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Year} - {Month}";
        }

        public static PeriodInfo Create(string moduleKey, int year, YearMonth month, ITimeCalendar calendar)
        {
            Month timeRange = new(year, month, calendar);

            return new PeriodInfo()
            {
                Status = PeriodStatus.Opened,
                Year = timeRange.Year,
                Month = timeRange.YearMonth,
                Quarter = TimeTool.GetQuarterOfMonth(timeRange.YearMonth),
                Since = timeRange.Start,
                Until = timeRange.End,
                ReferenceGroup = moduleKey.IsNullOrEmpty() ? KontecgCoreConsts.DefaultReferenceGroup : moduleKey,
                Calendar = calendar
            };
        }

        public static PeriodInfo Create(string moduleKey, DateTime startingDate, DateTime finishingDate, ITimeCalendar calendar)
        {
            Month timeRange = new(startingDate, calendar);

            return new PeriodInfo()
            {
                Status = PeriodStatus.Opened,
                Year = timeRange.Year,
                Month = timeRange.YearMonth,
                Quarter = TimeTool.GetQuarterOfMonth(timeRange.YearMonth),
                Since = startingDate,
                Until = finishingDate,
                ReferenceGroup = moduleKey.IsNullOrEmpty() ? KontecgCoreConsts.DefaultReferenceGroup : moduleKey,
                Calendar = calendar
            };
        }
    }
}

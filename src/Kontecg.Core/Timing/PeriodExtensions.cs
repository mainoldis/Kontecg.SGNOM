using Itenso.TimePeriod;

namespace Kontecg.Timing
{
    public static class PeriodExtensions
    {
        public static PeriodInfo ToPeriodInfo(this Period period)
        {
            return new PeriodInfo
            {
                Id = period.Id,
                CompanyId = period.CompanyId,
                Year = period.Year,
                Month = period.Month,
                Quarter = period.Quarter,
                Since = period.Since,
                Until = period.Until,
                ReferenceGroup = period.ReferenceGroup,
                Status = period.Status
            };
        }

        public static ITimePeriod ToTimePeriod(this Period period)
        {
            return new TimeRange(period.Since, period.Until, true);
        }

        public static ITimePeriod ToTimePeriod(this PeriodInfo periodInfo)
        {
            return new TimeRange(periodInfo.Since, periodInfo.Until, true);
        }
    }
}

using Itenso.TimePeriod;

namespace Kontecg.Salary
{
    public static class IncidentExtensions
    {
        public static ITimePeriod ToTimePeriod(this Incident incident)
        {
            return new TimeRange(incident.Start, incident.End, true);
        }

        public static ITimePeriod ToTimePeriod(this TimeDistributionDocument timeDistributionDocument)
        {
            return new TimeRange(timeDistributionDocument.Since, timeDistributionDocument.Until, true);
        }
    }
}

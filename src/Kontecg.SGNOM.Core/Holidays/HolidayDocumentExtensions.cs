using Itenso.TimePeriod;

namespace Kontecg.Holidays
{
    public static class HolidayDocumentExtensions
    {
        public static ITimePeriod ToTimePeriod(this HolidayDocument document)
        {
            return new TimeRange(document.Since, document.Until, true);
        }

        public static ITimePeriod ToTimePeriod(this HolidayNote document)
        {
            return new TimeRange(document.Since, document.Until, true);
        }
    }
}

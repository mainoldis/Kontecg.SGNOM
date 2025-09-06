using Itenso.TimePeriod;

namespace Kontecg.SocialSecurity
{
    public static class SubsidyDocumentExtensions
    {
        public static ITimePeriod ToTimePeriod(this SubsidyDocument document)
        {
            return new TimeRange(document.Since, document.Until, true);
        }

        public static ITimePeriod ToTimePeriod(this SubsidyNote document)
        {
            return new TimeRange(document.Since, document.Until, true);
        }
    }
}
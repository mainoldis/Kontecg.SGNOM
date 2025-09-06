using Kontecg.Events.Bus;

namespace Kontecg.Timing
{
    public class PeriodStatusChangedEventData : EventData
    {
        public PeriodStatusChangedEventData(PeriodInfo periodInfo)
        {
            PeriodInfo = periodInfo;
        }

        public PeriodInfo PeriodInfo { get; }
    }
}

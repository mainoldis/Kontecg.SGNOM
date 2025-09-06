using Itenso.TimePeriod;

namespace Kontecg.Timing
{
    public class WorkHourRange
    {
        public WorkHourRange(int hour) :
            this(hour, hour)
        {
        }

        public WorkHourRange(int startHour, int endHour) :
            this(new Time(startHour), new Time(endHour))
        {
        }

        public WorkHourRange(Time start, Time end)
        {
            if (start.Ticks <= end.Ticks)
            {
                Start = start;
                End = end;
            }
            else
            {
                End = start;
                Start = end;
                Night = true;
            }
        }

        public Time Start { get; }

        public Time End { get; }

        public bool Night { get; }

        public bool IsMoment => Start.Equals(End);

        public override string ToString()
        {
            return Night ? End + " - " + Start : Start + " - " + End;
        }
    }
}

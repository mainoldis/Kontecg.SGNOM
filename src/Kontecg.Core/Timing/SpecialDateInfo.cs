using System;

namespace Kontecg.Timing
{
    [Serializable]
    public class SpecialDateInfo
    {
        public DateTime Date { get; set; }

        public DayDecorator Cause { get; set; }
    }
}

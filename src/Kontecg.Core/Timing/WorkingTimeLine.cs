using System.Collections.Generic;
using Itenso.TimePeriod;

namespace Kontecg.Timing
{
    public class WorkingTimeLine : TimeLine<WorkingHours>
    {
        public WorkingTimeLine(IEnumerable<WorkingHours> periods, WorkYear year = null)
            : base(new TimePeriodCollection(periods), year, year?.Calendar ?? WorkCalendarTool.New())
        {
        }

        public WorkingTimeLine(IEnumerable<WorkingHours> periods, WorkMonth month)
            : base(new TimePeriodCollection(periods), month, month.Calendar)
        {
        }
    }
}

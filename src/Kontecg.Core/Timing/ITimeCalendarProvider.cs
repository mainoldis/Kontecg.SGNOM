using System.Collections.Generic;
using Itenso.TimePeriod;

namespace Kontecg.Timing
{
    public interface ITimeCalendarProvider
    {
        TimeCalendar GetWorkTimeCalendar();

        TimeCalendar GetCalendar(bool forWorkCalculus = true);

        IReadOnlyList<SpecialDateInfo> GetSpecialDates();
    }
}

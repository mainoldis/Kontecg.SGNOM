using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using Kontecg.Dependency;
using Kontecg.Timing;

namespace Kontecg.Presenters.Timing
{
    public class KontecgDisabledDateProvider : ICalendarDisabledDateProvider, ITransientDependency
    {
        private readonly IList<SpecialDateInfo> _defaultSpecialDates;

        public KontecgDisabledDateProvider(ITimeCalendarProvider timeCalendarProvider)
        {
            _defaultSpecialDates = timeCalendarProvider.GetSpecialDates()
                .Where(d => d.Cause == DayDecorator.Disabled).ToArray();
        }

        public bool IsDisabledDate(DateTime date, DateEditCalendarViewType view)
        {
            return _defaultSpecialDates.Any(h => h.Date.Year == date.Year && h.Date.Month == date.Month && h.Date.Day == date.Day);
        }
    }
}

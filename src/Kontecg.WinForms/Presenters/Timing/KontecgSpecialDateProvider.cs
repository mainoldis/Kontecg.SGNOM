using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using Kontecg.Dependency;
using Kontecg.Timing;

namespace Kontecg.Presenters.Timing
{
    public class KontecgSpecialDateProvider : ICalendarSpecialDateProvider, ITransientDependency
    {
        private readonly IList<SpecialDateInfo> _defaultSpecialDates;

        public KontecgSpecialDateProvider(ITimeCalendarProvider timeCalendarProvider)
        {
            _defaultSpecialDates = timeCalendarProvider.GetSpecialDates()
                .Where(d => d.Cause != DayDecorator.None && d.Cause != DayDecorator.Disabled).ToArray();
        }

        public bool IsSpecialDate(DateTime date, DateEditCalendarViewType view)
        {
            return _defaultSpecialDates.Any(h => h.Date.Month == date.Month && h.Date.Day == date.Day);
        }
    }
}

using System;
using System.Drawing;
using DevExpress.XtraEditors.Controls;
using Kontecg.Dependency;

namespace Kontecg.Presenters.Timing
{
    public static class CalendarAppearancesHelper
    {
        public static void Apply(CalendarControl calendar)
        {
            if (calendar != null)
            {
                calendar.SuspendLayout();
                calendar.SpecialDateProvider = IocManager.Instance.Resolve<KontecgSpecialDateProvider>();
                calendar.DisabledDateProvider = IocManager.Instance.Resolve<KontecgDisabledDateProvider>();
                calendar.InactiveDaysVisibility = CalendarInactiveDaysVisibility.Hidden;
                calendar.WeekNumberRule = WeekNumberRule.FirstDay;
                calendar.ShowWeekNumbers = true;
                calendar.CalendarAppearance.DayCellSpecial.FontStyleDelta = FontStyle.Bold;
                calendar.CaseMonthNames = TextCaseMode.UpperCase;
                calendar.CaseWeekDayAbbreviations = TextCaseMode.UpperCase;
                calendar.FirstDayOfWeek = DayOfWeek.Monday;
                calendar.ColumnCount = 4;
                calendar.RowCount = 3;
                calendar.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
                calendar.ResumeLayout(false);
            }
        }
    }
}

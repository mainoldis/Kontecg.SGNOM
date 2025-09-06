using System;

namespace Kontecg.Timing
{
    public static class DayDecoratorExtensions
    {
        public static CalendarTimeDecorator ToCalendarTimeDecorator(this DayDecorator decorator)
        {
            switch (decorator)
            {
                case DayDecorator.None:
                    return CalendarTimeDecorator.None;
                case DayDecorator.BreakSaturday:
                    return CalendarTimeDecorator.BreakSaturdayTime;
                case DayDecorator.BreakDay:
                    return CalendarTimeDecorator.BreakTime;
                case DayDecorator.NationalCelebrationDay:
                    return CalendarTimeDecorator.NationalCelebrationDayTime;
                case DayDecorator.NationalHoliday:
                    return CalendarTimeDecorator.NationalHolidayTime;
                default:
                    throw new ArgumentOutOfRangeException(nameof(decorator), decorator, null);
            }
        }
    }
}

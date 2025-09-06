namespace Kontecg.Timing
{
    public enum DayDecorator
    {
        None = 0,
        BreakSaturday = 1, // non-working Saturday
        BreakDay = 2, // Labor break
        NationalCelebrationDay = 3, // National celebration day, in case of falling Sunday they move
        NationalHoliday = 4, // Holidays
        Disabled = 5,
    }
}

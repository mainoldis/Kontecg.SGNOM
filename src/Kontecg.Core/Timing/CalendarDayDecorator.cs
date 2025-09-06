namespace Kontecg.Timing
{
    /// <summary>
    /// 
    /// </summary>
    public enum CalendarTimeDecorator 
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// non-working Saturday
        /// </summary>
        BreakSaturdayTime = 1, // non-working Saturday
        /// <summary>
        /// Labor break
        /// </summary>
        /// <remarks>88</remarks>
        BreakTime = 2, // Labor break
        /// <summary>
        /// National celebration day, in case of falling Sunday they move
        /// </summary>
        /// <remarks>508 or 581</remarks>
        NationalCelebrationDayTime = 3, // National celebration day, in case of falling Sunday they move
        /// <summary>
        /// National Holidays
        /// </summary>
        /// <remarks>508 or 581</remarks>
        NationalHolidayTime = 4, // National Holidays
        /// <summary>
        /// Early night working time
        /// </summary>
        /// <remarks>131, between 7:00 pm and 11:00 pm</remarks>
        EarlyNightTime = 5,
        /// <summary>
        /// Late night working time
        /// </summary>
        /// <remarks>132, between 11:00 pm and 07:00 am</remarks>
        LateNightTime = 6,
        /// <summary>
        /// Holiday time
        /// </summary>
        /// <remarks>505</remarks>
        HolidayTime = 7,
        /// <summary>
        /// Any subsidized time
        /// </summary>
        SubsidizedTime = 8,
        /// <summary>
        /// Extra working time
        /// </summary>
        /// <remarks>509 or 513</remarks>
        CrazyWorkShiftTime = 9,
        /// <summary>
        /// Normal working time
        /// </summary>
        /// <remarks>504</remarks>
        WorkingTime = 10,
        /// <summary>
        /// Another working time that is not classified
        /// </summary>
        /// <remarks>504</remarks>
        OtherTime = 11,
    }
}

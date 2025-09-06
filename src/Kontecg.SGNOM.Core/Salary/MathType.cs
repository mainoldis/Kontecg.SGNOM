namespace Kontecg.Salary
{
    /// <summary>
    /// Specifies the type of calculation to use in the formula
    /// </summary>
    public enum MathType
    {
        MinimumWage = 0, // Fixed 2100
        Average = 1,
        Percent = 2,
        RatePerHour = 3,
        Formula = 4,
    }
}

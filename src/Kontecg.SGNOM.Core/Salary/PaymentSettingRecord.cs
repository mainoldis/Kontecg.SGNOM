namespace Kontecg.Salary
{
    public record PaymentSettingRecord(
        string WorkingTime,
        string CrazyWorkingTime,
        string ExtraHours,
        string HolidayTime,
        string SubsidizedTime,
        string NormalBreakTime,
        string NormalNationalCelebrationDayTime,
        string NormalNationalHolidayTime,
        string SpecialBreakTime,
        string SpecialNationalCelebrationDayTime,
        string SpecialNationalHolidayTime,
        string EarlyNightTime,
        string LateNightTime,
        EmployeeSalaryForm EmployeeSalaryForm,
        PaymentSystem PaymentSystem);
}

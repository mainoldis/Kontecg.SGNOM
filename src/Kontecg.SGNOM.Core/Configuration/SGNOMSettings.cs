namespace Kontecg.Configuration
{
    /// <summary>
    ///     Defines string constants for setting names in the application.
    ///     See <see cref="SGNOMSettingProvider" /> for setting definitions.
    /// </summary>
    public static class SGNOMSettings
    {
        public static class General
        {
            public const string MinimumWageAmount = "SGNOM.General.MinimumWageAmount"; //2100
            public const string AverageWorkingHoursPerPeriod = "SGNOM.General.AverageWorkingHoursPerPeriod"; //190.6
            public const string AverageDaysPerPeriod = "SGNOM.General.AverageDaysPerPeriod"; // 24
            public const string NormalWorkRegime = "SGNOM.General.DefaultWorkRegime"; //5*2-6*1
            public const string Bank = "SGNOM.General.Bank"; //BPA
            public const string PaymentSystem = "SGNOM.General.PaymentSystem"; // ByTime
            public const string EmployeeSalaryForm = "SGNOM.General.EmployeeSalaryForm"; //Royal
        }

        public static class PaymentDefinition
        {
            public const string ForWorkingTime = "SGNOM.PaymentDefinition.ForWorkingTime"; //504
            public const string ForCrazyWorkingTime = "SGNOM.PaymentDefinition.ForCrazyWorkingTime"; //509
            public const string ForExtraHours = "SGNOM.PaymentDefinition.ForExtraHours"; //509
            public const string ForHolidayTime = "SGNOM.PaymentDefinition.ForHolidayTime"; //505
            public const string ForSpecialLeavePermissionTime = "SGNOM.PaymentDefinition.ForSpecialLeavePermissionTime"; //506 (Licencia para madres que no tienen que le cuiden sus hijos)
            //----------------------------
            public const string ForNormalBreakTime = "SGNOM.PaymentDefinition.ForNormalBreakTime"; //88
            public const string ForNormalNationalCelebrationDayTime = "SGNOM.PaymentDefinition.ForNormalNationalCelebrationDayTime"; //508
            public const string ForNormalNationalHolidayTime = "SGNOM.PaymentDefinition.ForNormalNationalHolidayTime"; //508
            public const string ForSpecialBreakTime = "SGNOM.PaymentDefinition.ForSpecialBreakTime"; //504
            public const string ForSpecialNationalCelebrationDayTime = "SGNOM.PaymentDefinition.ForSpecialNationalCelebrationDayTime"; //581
            public const string ForSpecialNationalHolidayTime = "SGNOM.PaymentDefinition.ForSpecialNationalHolidayTime"; //581
            //----------------------------
            public const string ForEarlyNightTime = "SGNOM.PaymentDefinition.ForEarlyNightTime"; //131
            public const string ForLateNightTime = "SGNOM.PaymentDefinition.ForLateNightTime"; //132
        }

        public static class AccountingInformation
        {
            public const string FunctionForWorkingTime = "SGNOM.AccountingInformation.FunctionForWorkingTime"; //C_TP504
            public const string FunctionForHolidayTime = "SGNOM.AccountingInformation.FunctionForHolidayTime"; //C_PVAC
            public const string FunctionForPensionPayments = "SGNOM.AccountingInformation.FunctionForPensionPayments"; //C_PPEN
            public const string FunctionForIncomeTaxes = "SGNOM.AccountingInformation.FunctionForIncomeTaxes"; //C_IMP_ING
            public const string FunctionForSocialSecurityTaxes = "SGNOM.AccountingInformation.FunctionForSocialSecurityTaxes"; //C_IMP_CSS
            public const string FunctionForClaims = "SGNOM.AccountingInformation.FunctionForClaims"; //C_RECLAM
        }

        public static class Taxes
        {
            public const string ForSocialSecurity = "SGNOM.Taxes.SocialSecurityTaxes";
            public const string ForIncome = "SGNOM.Taxes.IncomeTaxes";
            public const string ForCompanyWorkforce = "SGNOM.Taxes.CompanyWorkforceTaxes";
            public const string ForCompanySocialSecurity = "SGNOM.Taxes.CompanySocialSecurityTaxes";
        }

        public static class Holidays
        {
            public const string HolidaysContribution = "SGNOM.Holidays.HolidaysContribution"; //0.0909
        }

        public static class Claims
        {
        }

        public static class Retentions
        {
            public const string MaxPercent = "SGNOM.Retentions.MaxPercent";
            public const string ForDemand = "SGNOM.Retentions.ForDemand"; //Demandas
            public const string ForJudicialAttachment = "SGNOM.Retentions.ForJudicialAttachment"; //Embargo Judicial
            public const string ForTransportationDiscount = "SGNOM.Retentions.ForTransportationDiscount"; //Descuento del transporte
        }

        public static class SocialSecurity
        {
            public const string WorkShift = "SGNOM.SocialSecurity.WorkShift"; //Normal - Régimen para el cálculo para la seguridad social
        }

        public static class WorkShift
        {
            public const string Default = "SGNOM.WorkShift.Default"; //N
        }

        public static class Employment
        {
            public const string IndexRanges = "SGNOM.Employment.IndexRanges";
            public const string AllowEmploymentsOutOfTemplate = "SGNOM.Employment.AllowEmploymentsOutOfTemplate"; //Options are 'Strict', 'Flexible' & 'NotAllowed'
        }

        public static class SingingDocument
        {
        }
    }
}

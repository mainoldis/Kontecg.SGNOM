using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kontecg.Json;
using Kontecg.Organizations;
using Kontecg.Salary;
using Kontecg.Taxes;
using Microsoft.Extensions.Configuration;

namespace Kontecg.Configuration
{
    /// <summary>
    ///     Defines settings for the application.
    ///     See <see cref="SGNOMSettings" /> for setting names.
    /// </summary>
    public class SGNOMSettingProvider : SettingProvider
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly VisibleSettingClientVisibilityProvider _visibleSettingClientVisibilityProvider;

        public SGNOMSettingProvider(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
            _visibleSettingClientVisibilityProvider = new VisibleSettingClientVisibilityProvider();
        }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return GetGeneralSettings()
                .Union(GetPaymentDefinitionSettings())
                .Union(GetAccountingInformationSettings())
                .Union(GetTaxesSettings())
                .Union(GetHolidaysSettings())
                .Union(GetClaimsSettings())
                .Union(GetRetentionsSettings())
                .Union(GetSocialSecuritySettings())
                .Union(GetWorkShiftSettings())
                .Union(GetEmploymentSettings())
                .Union(GetSingingDocumentSettings());
        }

        private IEnumerable<SettingDefinition> GetGeneralSettings()
        {
            return
            [
                new SettingDefinition(SGNOMSettings.General.MinimumWageAmount,
                    GetFromAppSettings(SGNOMSettings.General.MinimumWageAmount, SGNOMConsts.MinimumWageAmount.ToString("0:C2")),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.General.AverageWorkingHoursPerPeriod,
                    GetFromAppSettings(SGNOMSettings.General.AverageWorkingHoursPerPeriod, SGNOMConsts.DefaultAverageWorkingHoursPerPeriod.ToString("0:F2")),
                clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.General.AverageDaysPerPeriod,
                    GetFromAppSettings(SGNOMSettings.General.AverageDaysPerPeriod, SGNOMConsts.DefaultAverageDaysPerPeriod.ToString()),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.General.NormalWorkRegime,
                    GetFromAppSettings(SGNOMSettings.General.NormalWorkRegime, "5*2-6*1"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.General.Bank,
                    GetFromAppSettings(SGNOMSettings.General.Bank, "BPA"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.General.PaymentSystem,
                    GetFromAppSettings(SGNOMSettings.General.PaymentSystem,  nameof(PaymentSystem.ByTime)),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.General.EmployeeSalaryForm,
                    GetFromAppSettings(SGNOMSettings.General.EmployeeSalaryForm, nameof(EmployeeSalaryForm.Royal)),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company)
            ];
        }

        private IEnumerable<SettingDefinition> GetPaymentDefinitionSettings()
        {
            return
            [
                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForWorkingTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForWorkingTime, "504"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForCrazyWorkingTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForCrazyWorkingTime, "513"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForExtraHours,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForExtraHours, "509"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForHolidayTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForHolidayTime, "505"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForSpecialLeavePermissionTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForSpecialLeavePermissionTime, "506"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForNormalBreakTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForNormalBreakTime, "88"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForNormalNationalCelebrationDayTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForNormalNationalCelebrationDayTime, "508"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForNormalNationalHolidayTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForNormalNationalHolidayTime, "508"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForSpecialBreakTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForSpecialBreakTime, "504"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForSpecialNationalCelebrationDayTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForSpecialNationalCelebrationDayTime, "581"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForSpecialNationalHolidayTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForSpecialNationalHolidayTime, "581"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForEarlyNightTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForEarlyNightTime, "131"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.PaymentDefinition.ForLateNightTime,
                    GetFromAppSettings(SGNOMSettings.PaymentDefinition.ForLateNightTime, "132"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company)
            ];
        }

        private IEnumerable<SettingDefinition> GetAccountingInformationSettings()
        {
            return
            [
                new SettingDefinition(SGNOMSettings.AccountingInformation.FunctionForWorkingTime,
                    GetFromAppSettings(SGNOMSettings.AccountingInformation.FunctionForWorkingTime, "C_TP504"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.AccountingInformation.FunctionForHolidayTime,
                    GetFromAppSettings(SGNOMSettings.AccountingInformation.FunctionForHolidayTime, "C_PVAC"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.AccountingInformation.FunctionForPensionPayments,
                    GetFromAppSettings(SGNOMSettings.AccountingInformation.FunctionForPensionPayments, "C_PPEN"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.AccountingInformation.FunctionForIncomeTaxes,
                    GetFromAppSettings(SGNOMSettings.AccountingInformation.FunctionForIncomeTaxes, "C_IMP_ING"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.AccountingInformation.FunctionForSocialSecurityTaxes,
                    GetFromAppSettings(SGNOMSettings.AccountingInformation.FunctionForSocialSecurityTaxes, "C_IMP_CSS"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.AccountingInformation.FunctionForClaims,
                    GetFromAppSettings(SGNOMSettings.AccountingInformation.FunctionForClaims, "C_RECLAM"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),
            ];
        }

        private IEnumerable<SettingDefinition> GetTaxesSettings()
        {
            var forSocialSecurity = new TaxInfo(TaxType.SocialSecurity, 100)
            {
                Ranges = new List<TaxRangeRecord>()
                {
                    new(decimal.Zero, 15000M, 5M),
                    new(15000M, decimal.MaxValue, 10M)
                },
                Reference = "C_IMP_CSS"
            };

            var forIncome = new TaxInfo(TaxType.Income, 100)
            {
                Ranges = new List<TaxRangeRecord>()
                {
                    new(decimal.Zero, 3260M, decimal.Zero),
                    new(3260M, 9510M, 3M),
                    new(9510M, 15000M, 5M),
                    new(15000M, 20000M, 7.5M),
                    new(20000M, 25000M, 10M),
                    new(25000M, 30000M, 15M),
                    new(30000M, decimal.MaxValue, 20M)
                },
                Reference = "C_IMP_ING"
            };

            var forCompanySocialSecurity = new TaxInfo(TaxType.CompanySocialSecurity, 12.5M);
            var forCompanyWorkforce = new TaxInfo(TaxType.CompanyWorkforce, 5M);
            
            return
            [
                new SettingDefinition(SGNOMSettings.Taxes.ForSocialSecurity, GetFromAppSettings(SGNOMSettings.Taxes.ForSocialSecurity, forSocialSecurity.ToJsonString()), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(SGNOMSettings.Taxes.ForIncome, GetFromAppSettings(SGNOMSettings.Taxes.ForIncome, forIncome.ToJsonString()), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(SGNOMSettings.Taxes.ForCompanySocialSecurity, GetFromAppSettings(SGNOMSettings.Taxes.ForCompanySocialSecurity, forCompanySocialSecurity.ToJsonString()), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(SGNOMSettings.Taxes.ForCompanyWorkforce, GetFromAppSettings(SGNOMSettings.Taxes.ForCompanyWorkforce, forCompanyWorkforce.ToJsonString()), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company)
            ];
        }

        private IEnumerable<SettingDefinition> GetHolidaysSettings()
        {
            return
            [
                new SettingDefinition(SGNOMSettings.Holidays.HolidaysContribution, GetFromAppSettings(SGNOMSettings.Holidays.HolidaysContribution,
                        SGNOMConsts.DefaultHolidayContribution.ToString(CultureInfo.InvariantCulture)), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company)
            ];
        }

        private IEnumerable<SettingDefinition> GetClaimsSettings()
        {
            return Array.Empty<SettingDefinition>();
        }

        private IEnumerable<SettingDefinition> GetRetentionsSettings()
        {
            return
            [
                new SettingDefinition(SGNOMSettings.Retentions.MaxPercent, GetFromAppSettings(SGNOMSettings.Retentions.MaxPercent, "30"), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(SGNOMSettings.Retentions.ForDemand, GetFromAppSettings(SGNOMSettings.Retentions.ForDemand, "113"), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(SGNOMSettings.Retentions.ForJudicialAttachment, GetFromAppSettings(SGNOMSettings.Retentions.ForJudicialAttachment, "88"), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(SGNOMSettings.Retentions.ForTransportationDiscount, GetFromAppSettings(SGNOMSettings.Retentions.ForTransportationDiscount, "86"), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),
            ];
        }

        private IEnumerable<SettingDefinition> GetSocialSecuritySettings()
        {
            return
            [
                new SettingDefinition(SGNOMSettings.SocialSecurity.WorkShift, GetFromAppSettings(SGNOMSettings.SocialSecurity.WorkShift, "Normal"), clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),
            ];
        }

        private IEnumerable<SettingDefinition> GetWorkShiftSettings()
        {
            return
            [
                new SettingDefinition(SGNOMSettings.WorkShift.Default,
                    GetFromAppSettings(SGNOMSettings.WorkShift.Default, "N"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company)
            ];
        }

        private IEnumerable<SettingDefinition> GetEmploymentSettings()
        {
            return
            [
                new SettingDefinition(SGNOMSettings.Employment.IndexRanges,
                    GetFromAppSettings(SGNOMSettings.Employment.IndexRanges,
                        "{\"StartIndexForLifeTimeContracts\":1,\"EndIndexForLifeTimeContracts\":47999,\"StartIndexForInitialLifeTimeContracts\":60000,\"EndIndexForInitialLifeTimeContracts\":69999,\"StartIndexForShortContracts\":48000,\"EndIndexForShortContracts\":59999,\"StartIndexForTemporallyContracts\":70000,\"EndIndexForTemporallyContracts\":79999}"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(SGNOMSettings.Employment.AllowEmploymentsOutOfTemplate,
                    GetFromAppSettings(SGNOMSettings.Employment.AllowEmploymentsOutOfTemplate,
                        AllowEmploymentsOutOfTemplateOptions.NotAllowed.ToString()),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company)
            ];
        }

        private IEnumerable<SettingDefinition> GetSingingDocumentSettings()
        {
            return Array.Empty<SettingDefinition>();
        }

        private string GetFromAppSettings(string name, string defaultValue = null)
        {
            return GetFromSettings("App:" + name, defaultValue);
        }

        private string GetFromSettings(string name, string defaultValue = null)
        {
            return _appConfiguration[name] ?? defaultValue;
        }
    }
}

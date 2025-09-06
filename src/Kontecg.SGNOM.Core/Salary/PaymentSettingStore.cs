using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.Salary
{
    /// <summary>
    /// Implements <see cref="IPaymentSettingStore"/> to get settings from <see cref="ISettingManager"/>.
    /// </summary>
    public class PaymentSettingStore : IPaymentSettingStore, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentSettingStore"/> class.
        /// </summary>
        public PaymentSettingStore(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }
        
        public PaymentSettingRecord GetPaymentDefinition(int? companyId)
        {
            if (companyId.HasValue)
                return new PaymentSettingRecord(
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForWorkingTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForCrazyWorkingTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForExtraHours, companyId.Value), _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForHolidayTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForSpecialLeavePermissionTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForNormalBreakTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForNormalNationalCelebrationDayTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForNormalNationalHolidayTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForSpecialBreakTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForSpecialNationalCelebrationDayTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForSpecialNationalHolidayTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForEarlyNightTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForLateNightTime, companyId.Value),
                    _settingManager.GetSettingValueForCompany<EmployeeSalaryForm>(SGNOMSettings.General.EmployeeSalaryForm, companyId.Value),
                    _settingManager.GetSettingValueForCompany<PaymentSystem>(SGNOMSettings.General.PaymentSystem, companyId.Value));

            return new PaymentSettingRecord(
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForWorkingTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForCrazyWorkingTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForExtraHours), _settingManager.GetSettingValueForCompany(SGNOMSettings.PaymentDefinition.ForHolidayTime, companyId.Value),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForSpecialLeavePermissionTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForNormalBreakTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForNormalNationalCelebrationDayTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForNormalNationalHolidayTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForSpecialBreakTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForSpecialNationalCelebrationDayTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForSpecialNationalHolidayTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForEarlyNightTime),
                _settingManager.GetSettingValueForApplication(SGNOMSettings.PaymentDefinition.ForLateNightTime),
                _settingManager.GetSettingValueForApplication<EmployeeSalaryForm>(SGNOMSettings.General.EmployeeSalaryForm),
                _settingManager.GetSettingValueForApplication<PaymentSystem>(SGNOMSettings.General.PaymentSystem));
        }

        public async Task<PaymentSettingRecord> GetPaymentDefinitionAsync(int? companyId)
        {
            if (companyId.HasValue)
                return new PaymentSettingRecord(
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForWorkingTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForCrazyWorkingTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForExtraHours, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForHolidayTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForSpecialLeavePermissionTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForNormalBreakTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForNormalNationalCelebrationDayTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForNormalNationalHolidayTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForSpecialBreakTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForSpecialNationalCelebrationDayTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForSpecialNationalHolidayTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForEarlyNightTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.PaymentDefinition.ForLateNightTime, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync<EmployeeSalaryForm>(SGNOMSettings.General.EmployeeSalaryForm, companyId.Value),
                    await _settingManager.GetSettingValueForCompanyAsync<PaymentSystem>(SGNOMSettings.General.PaymentSystem, companyId.Value));

            return new PaymentSettingRecord(
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForWorkingTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForCrazyWorkingTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForExtraHours),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForHolidayTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForSpecialLeavePermissionTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForNormalBreakTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForNormalNationalCelebrationDayTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForNormalNationalHolidayTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForSpecialBreakTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForSpecialNationalCelebrationDayTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForSpecialNationalHolidayTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForEarlyNightTime),
                await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.PaymentDefinition.ForLateNightTime),
                await _settingManager.GetSettingValueForApplicationAsync<EmployeeSalaryForm>(SGNOMSettings.General.EmployeeSalaryForm),
                await _settingManager.GetSettingValueForApplicationAsync<PaymentSystem>(SGNOMSettings.General.PaymentSystem));
        }

        public string GetNormalWorkShiftName(int? companyId)
        {
            return companyId.HasValue
                ? _settingManager.GetSettingValueForCompany(SGNOMSettings.WorkShift.Default, companyId.Value)
                : _settingManager.GetSettingValueForApplication(SGNOMSettings.WorkShift.Default);
        }

        public async Task<string> GetNormalWorkShiftNameAsync(int? companyId)
        {
            return companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.WorkShift.Default, companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.WorkShift.Default);
        }
    }
}

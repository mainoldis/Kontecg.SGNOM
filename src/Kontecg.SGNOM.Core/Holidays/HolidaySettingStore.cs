using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.Holidays
{
    public class HolidaySettingStore : IHolidaySettingStore, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidaySettingStore"/> class.
        /// </summary>
        public HolidaySettingStore(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public int GetMaxAllowedDays(int? companyId)
        {
            return companyId.HasValue
                ? _settingManager.GetSettingValueForCompany<int>(SGNOMSettings.General.AverageDaysPerPeriod, companyId.Value)
                : _settingManager.GetSettingValueForApplication<int>(SGNOMSettings.General.AverageDaysPerPeriod);
        }

        public async Task<int> GetMaxAllowedDaysAsync(int? companyId)
        {
            return companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync<int>(SGNOMSettings.General.AverageDaysPerPeriod, companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync<int>(SGNOMSettings.General.AverageDaysPerPeriod);
        }
    }
}

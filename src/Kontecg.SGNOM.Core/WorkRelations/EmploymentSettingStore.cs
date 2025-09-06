using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Json;
using Kontecg.Organizations;

namespace Kontecg.WorkRelations
{
    public class EmploymentSettingStore : IEmploymentSettingStore, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmploymentSettingStore"/> class.
        /// </summary>
        public EmploymentSettingStore(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public EmploymentIndexSettingRecord GetIndexRanges(int? companyId)
        {
            var setting = companyId.HasValue
                ? _settingManager.GetSettingValueForCompany(SGNOMSettings.Employment.IndexRanges, companyId.Value)
                : _settingManager.GetSettingValueForApplication(SGNOMSettings.Employment.IndexRanges);

            return setting.FromJsonString<EmploymentIndexSettingRecord>();
        }

        public async Task<EmploymentIndexSettingRecord> GetIndexRangesAsync(int? companyId)
        {
            var setting = companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.Employment.IndexRanges,
                    companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.Employment.IndexRanges);

            return setting.FromJsonString<EmploymentIndexSettingRecord>();
        }

        public AllowEmploymentsOutOfTemplateOptions GetAllowEmploymentsOutOfTemplateOptions(int? companyId)
        {
            return companyId.HasValue
                ? _settingManager.GetSettingValueForCompany<AllowEmploymentsOutOfTemplateOptions>(
                    SGNOMSettings.Employment.AllowEmploymentsOutOfTemplate, companyId.Value)
                : _settingManager.GetSettingValueForApplication<AllowEmploymentsOutOfTemplateOptions>(
                    SGNOMSettings.Employment.AllowEmploymentsOutOfTemplate);
        }

        public async Task<AllowEmploymentsOutOfTemplateOptions> GetAllowEmploymentsOutOfTemplateOptionsAsync(int? companyId)
        {
            return companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync<AllowEmploymentsOutOfTemplateOptions>(
                    SGNOMSettings.Employment.AllowEmploymentsOutOfTemplate, companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync<AllowEmploymentsOutOfTemplateOptions>(
                    SGNOMSettings.Employment.AllowEmploymentsOutOfTemplate);
        }
    }
}

using System.Threading.Tasks;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Localization;
using Kontecg.MultiCompany.Dto;
using Kontecg.Notifications;
using Kontecg.UI;

namespace Kontecg.MultiCompany
{
    public class CompanyRegistrationAppService : KontecgAppServiceBase, ICompanyRegistrationAppService
    {
        private readonly IAppNotifier _appNotifier;
        private readonly ILocalizationContext _localizationContext;
        private readonly IMultiCompanyConfig _multiCompanyConfig;
        private readonly CompanyManager _companyManager;

        public CompanyRegistrationAppService(
            IMultiCompanyConfig multiCompanyConfig,
            IAppNotifier appNotifier,
            ILocalizationContext localizationContext,
            CompanyManager companyManager)
        {
            _multiCompanyConfig = multiCompanyConfig;
            _appNotifier = appNotifier;
            _localizationContext = localizationContext;
            _companyManager = companyManager;
        }

        public async Task<RegisterCompanyOutput> RegisterCompanyAsync(RegisterCompanyInput input)
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                CheckCompanyRegistrationIsEnabled();

                //Getting host-specific settings
                var isActive = await IsNewRegisteredCompanyActiveByDefaultAsync();
                var isEmailConfirmationRequired = await SettingManager.GetSettingValueForApplicationAsync<bool>(
                    KontecgBaselineSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin
                );

                var companyId = await _companyManager.CreateWithAdminUserAsync(
                    input.CompanyName,
                    input.Name,
                    input.Reup,
                    input.Organism,
                    input.Address,
                    input.AdminPassword,
                    input.AdminEmailAddress,
                    null,
                    isActive,
                    false
                );

                var company = await CompanyManager.GetByIdAsync(companyId);
                await _appNotifier.NewCompanyRegisteredAsync(company);

                return new RegisterCompanyOutput
                {
                    CompanyId = company.Id,
                    CompanyName = input.CompanyName,
                    Name = input.Name,
                    UserName = KontecgUserBase.AdminUserName,
                    EmailAddress = input.AdminEmailAddress,
                    IsActive = company.IsActive,
                    IsEmailConfirmationRequired = isEmailConfirmationRequired,
                    IsCompanyActive = company.IsActive
                };
            }
        }

        private async Task<bool> IsNewRegisteredCompanyActiveByDefaultAsync()
        {
            return await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.CompanyManagement
                .IsNewRegisteredCompanyActiveByDefault);
        }

        private void CheckCompanyRegistrationIsEnabled()
        {
            if (!IsSelfRegistrationEnabled())
                throw new UserFriendlyException(L("SelfCompanyRegistrationIsDisabledMessage_Detail"));

            if (!_multiCompanyConfig.IsEnabled) throw new UserFriendlyException(L("MultiCompanyIsNotEnabled"));
        }

        private bool IsSelfRegistrationEnabled()
        {
            return SettingManager.GetSettingValueForApplication<bool>(
                AppSettings.CompanyManagement.AllowSelfRegistration);
        }
    }
}

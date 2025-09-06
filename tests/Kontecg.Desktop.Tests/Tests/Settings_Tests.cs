using Kontecg.Configuration;
using Kontecg.Configuration.Companies;
using Kontecg.Configuration.Host;
using Kontecg.Localization;
using Kontecg.Runtime.Session;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Settings_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public void Get_unauthenticated_users_config_Test()
        {
            var configurationBuilder = LocalIocManager.Resolve<KontecgUserConfigurationBuilder>();
            var allConfig = configurationBuilder.GetAll();
        }

        [Fact]
        public void Get_authenticated_users_config_Test()
        {
            KontecgSession.Use(1, 2);
            var configurationBuilder = LocalIocManager.Resolve<KontecgUserConfigurationBuilder>();
            var allConfig = configurationBuilder.GetAll();
        }

        [Fact]
        public async void Get_authenticated_host_settings_Test()
        {
            KontecgSession.Use(null, 1);
            var hostSettingsAppService = LocalIocManager.Resolve<IHostSettingsAppService>();
            var allConfig = await hostSettingsAppService.GetAllSettingsAsync();
        }

        [Fact]
        public async void Change_some_host_settings_Test()
        {
            KontecgSession.Use(null, 1);

            await WithUnitOfWorkAsync(async () => 
            {
                var hostSettingsAppService = LocalIocManager.Resolve<IHostSettingsAppService>();
                var hostSettings = await hostSettingsAppService.GetAllSettingsAsync();

                hostSettings.General.Currency = "CUP";

                hostSettings.Billing.Country = "CUBA";
                hostSettings.Billing.State = "HOLGUIN";
                hostSettings.Billing.City = "MOA";

                await hostSettingsAppService.UpdateAllSettingsAsync(hostSettings);
            });
        }
        [Fact]
        public async void Change_some_company_settings_Test()
        {
            KontecgSession.Use(1, 2);

            await WithUnitOfWorkAsync(async () =>
            {
                var companySettingsAppService = LocalIocManager.Resolve<ICompanySettingsAppService>();
                var companySettings = await companySettingsAppService.GetAllSettingsAsync();

                companySettings.Billing.LegalName = "EMPRESA DEL NIQUEL \"COMANDANTE ERNESTO CHE GUEVARA\"";
                companySettings.Billing.Address = "CARRETERA MOA-BARACOA KM.5";

                companySettings.Security.UserLockOut.IsEnabled = true;
                companySettings.Security.UserLockOut.DefaultAccountLockoutSeconds = 300;

                companySettings.UserManagement.SessionTimeOutSettings.IsEnabled = true;
                companySettings.UserManagement.SessionTimeOutSettings.ShowLockScreenWhenTimedOut = true;
                companySettings.UserManagement.SessionTimeOutSettings.TimeOutSecond = 360;

                companySettings.Ldap.IsEnabled = true;
                companySettings.Ldap.Domain = "172.22.16.1";
                companySettings.Ldap.UserName = @"ecg\adminsql1";
                companySettings.Ldap.Password = "SystemManagerSql!463";

                await companySettingsAppService.UpdateAllSettingsAsync(companySettings);
            });
        }

        [Fact]
        public async void Change_user_especific_setting_Test()
        {
            KontecgSession.Use(1, 2);

            await WithUnitOfWorkAsync(async () =>
            {
                var settingManager = LocalIocManager.Resolve<ISettingManager>();

                await settingManager.ChangeSettingForUserAsync(KontecgSession.ToUserIdentifier(), LocalizationSettingNames.DefaultLanguage, "en");
            });
        }
    }
}

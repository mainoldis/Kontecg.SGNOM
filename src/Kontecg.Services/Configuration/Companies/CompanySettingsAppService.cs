using System.Globalization;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Baseline.Configuration;
using Kontecg.Baseline.Ldap.Configuration;
//using Kontecg.Baseline.ServicioAdmin.Configuration;
using Kontecg.Configuration.Companies.Dto;
using Kontecg.Configuration.Host.Dto;
using Kontecg.Configuration.Startup;
using Kontecg.Extensions;
using Kontecg.Net.Mail;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Timing;
using Kontecg.UI;

namespace Kontecg.Configuration.Companies
{
    [KontecgAuthorize(PermissionNames.AdministrationCompanySettings)]
    public class CompanySettingsAppService : SettingsAppServiceBase, ICompanySettingsAppService
    {
        private readonly IMultiCompanyConfig _multiCompanyConfig;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IKontecgLdapModuleConfig _ldapModuleConfig;
        //private readonly IKontecgServicioAdminModuleConfig _servicioAdminModuleConfig;

        public CompanySettingsAppService(
            IEmailSender emailSender,
            IMultiCompanyConfig multiCompanyConfig,
            ITimeZoneService timeZoneService,
            IBinaryObjectManager binaryObjectManager,
            IKontecgLdapModuleConfig ldapModuleConfig,
            //IKontecgServicioAdminModuleConfig servicioAdminModuleConfig,
            IAppConfigurationAccessor configurationAccessor
            )
            : base(emailSender, configurationAccessor)
        {
            _multiCompanyConfig = multiCompanyConfig;
            _ldapModuleConfig = ldapModuleConfig;
            //_servicioAdminModuleConfig = servicioAdminModuleConfig;
            _timeZoneService = timeZoneService;
            _binaryObjectManager = binaryObjectManager;
        }

        #region Get Settings

        public async Task<CompanySettingsEditDto> GetAllSettingsAsync()
        {
            var settings = new CompanySettingsEditDto
            {
                UserManagement = await GetUserManagementSettingsAsync(),
                Email = await GetEmailSettingsAsync(),
                Security = await GetSecuritySettingsAsync(),
                Billing = await GetBillingSettingsAsync(),
                OtherSettings = await GetOtherSettingsAsync(),
            };

            if (!_multiCompanyConfig.IsEnabled || Clock.SupportsMultipleTimezone)
            {
                settings.General = await GetGeneralSettingsAsync();
            }

            if (_ldapModuleConfig.IsEnabled)
            {
                settings.Ldap = await GetLdapSettingsAsync();
            }
            else
            {
                settings.Ldap = new LdapSettingsEditDto {IsModuleEnabled = false};
            }
            /*
            if (_servicioAdminModuleConfig.IsEnabled)
            {
                settings.ServicioAdmin = await GetServicioAdminSettingsAsync();
            }
            else
            {
                settings.ServicioAdmin = new ServicioAdminSettingsEditDto {IsModuleEnabled = false};
            }
            */
            return settings;
        }

        private async Task<CompanyUserManagementSettingsEditDto> GetUserManagementSettingsAsync()
        {
            return new CompanyUserManagementSettingsEditDto
            {
                AllowSelfRegistration = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.AllowSelfRegistration),
                IsNewRegisteredUserActiveByDefault = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault),
                IsEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin),
                SessionTimeOutSettings = new SessionTimeOutSettingsEditDto()
                {
                    IsEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.SessionTimeOut.IsEnabled),
                    TimeOutSecond = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.SessionTimeOut.TimeOutSecond),
                    ShowTimeOutNotificationSecond = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond),
                    ShowLockScreenWhenTimedOut = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut)
                },
                UserPasswordSettings = new UserPasswordSettingsEditDto()
                {
                    EnableCheckingLastXPasswordWhenPasswordChange = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange),
                    CheckingLastXPasswordCount = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.Password.CheckingLastXPasswordCount),
                    EnablePasswordExpiration = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.Password.EnablePasswordExpiration),
                    PasswordExpirationDayCount = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.Password.PasswordExpirationDayCount),
                    PasswordResetCodeExpirationHours = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.Password.PasswordResetCodeExpirationHours),
                }
            };
        }

        private async Task<SecuritySettingsEditDto> GetSecuritySettingsAsync()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit),
                RequireLowercase = await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await SettingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength)
            };

            var defaultPasswordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = await SettingManager.GetSettingValueForApplicationAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit),
                RequireLowercase = await SettingManager.GetSettingValueForApplicationAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await SettingManager.GetSettingValueForApplicationAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await SettingManager.GetSettingValueForApplicationAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await SettingManager.GetSettingValueForApplicationAsync<int>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength)
            };

            return new SecuritySettingsEditDto
            {
                UseDefaultPasswordComplexitySettings = passwordComplexitySetting.Equals(defaultPasswordComplexitySetting),
                PasswordComplexity = passwordComplexitySetting,
                DefaultPasswordComplexity = defaultPasswordComplexitySetting,
                UserLockOut = await GetUserLockOutSettingsAsync(),
                AllowOneConcurrentLoginPerUser = await GetOneConcurrentLoginPerUserSettingAsync()
            };
        }

        private async Task<UserLockOutSettingsEditDto> GetUserLockOutSettingsAsync()
        {
            return new UserLockOutSettingsEditDto
            {
                IsEnabled = await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.UserLockOut.IsEnabled),
                MaxFailedAccessAttemptsBeforeLockout = await SettingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout),
                DefaultAccountLockoutSeconds = await SettingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds)
            };
        }

        private async Task<bool> GetOneConcurrentLoginPerUserSettingAsync()
        {
            return await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser);
        }

        private async Task<GeneralSettingsEditDto> GetGeneralSettingsAsync()
        {
            var settings = new GeneralSettingsEditDto();

            if (Clock.SupportsMultipleTimezone)
            {
                var timezone = await SettingManager.GetSettingValueForCompanyAsync(TimingSettingNames.TimeZone, KontecgSession.GetCompanyId());

                settings.Timezone = timezone;
                settings.TimezoneForComparison = timezone;
            }

            var defaultTimeZoneId = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Company, KontecgSession.CompanyId);

            if (settings.Timezone == defaultTimeZoneId)
            {
                settings.Timezone = string.Empty;
            }

            var currency =
                await SettingManager.GetSettingValueForCompanyAsync(AppSettings.CurrencyManagement.BaseCurrency, KontecgSession.GetCompanyId());

            settings.Currency = currency;

            return settings;
        }

        private async Task<CompanyEmailSettingsEditDto> GetEmailSettingsAsync()
        {
            var useHostDefaultEmailSettings = await SettingManager.GetSettingValueForCompanyAsync<bool>(AppSettings.Email.UseHostDefaultEmailSettings, KontecgSession.GetCompanyId());

            if (useHostDefaultEmailSettings)
            {
                return new CompanyEmailSettingsEditDto
                {
                    UseHostDefaultEmailSettings = true
                };
            }

            var smtpPassword = await SettingManager.GetSettingValueForCompanyAsync(EmailSettingNames.Smtp.Password, KontecgSession.GetCompanyId());

            return new CompanyEmailSettingsEditDto
            {
                UseHostDefaultEmailSettings = false,
                DefaultFromAddress = await SettingManager.GetSettingValueForCompanyAsync(EmailSettingNames.DefaultFromAddress, KontecgSession.GetCompanyId()),
                DefaultFromDisplayName = await SettingManager.GetSettingValueForCompanyAsync(EmailSettingNames.DefaultFromDisplayName, KontecgSession.GetCompanyId()),
                SmtpHost = await SettingManager.GetSettingValueForCompanyAsync(EmailSettingNames.Smtp.Host, KontecgSession.GetCompanyId()),
                SmtpPort = await SettingManager.GetSettingValueForCompanyAsync<int>(EmailSettingNames.Smtp.Port, KontecgSession.GetCompanyId()),
                SmtpUserName = await SettingManager.GetSettingValueForCompanyAsync(EmailSettingNames.Smtp.UserName, KontecgSession.GetCompanyId()),
                SmtpPassword = SimpleStringCipher.Instance.Decrypt(smtpPassword),
                SmtpDomain = await SettingManager.GetSettingValueForCompanyAsync(EmailSettingNames.Smtp.Domain, KontecgSession.GetCompanyId()),
                SmtpEnableSsl = await SettingManager.GetSettingValueForCompanyAsync<bool>(EmailSettingNames.Smtp.EnableSsl, KontecgSession.GetCompanyId()),
                SmtpUseDefaultCredentials = await SettingManager.GetSettingValueForCompanyAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials, KontecgSession.GetCompanyId())
            };
        }

        private async Task<LdapSettingsEditDto> GetLdapSettingsAsync()
        {
            return new LdapSettingsEditDto
            {
                IsModuleEnabled = true,
                IsEnabled = await SettingManager.GetSettingValueForCompanyAsync<bool>(LdapSettingNames.IsEnabled, KontecgSession.GetCompanyId()),
                Domain = await SettingManager.GetSettingValueForCompanyAsync(LdapSettingNames.Domain, KontecgSession.GetCompanyId()),
                UserName = await SettingManager.GetSettingValueForCompanyAsync(LdapSettingNames.UserName, KontecgSession.GetCompanyId()),
                Password = await SettingManager.GetSettingValueForCompanyAsync(LdapSettingNames.Password, KontecgSession.GetCompanyId()),
                UseSsl = await SettingManager.GetSettingValueForCompanyAsync<bool>(LdapSettingNames.UseSsl, KontecgSession.GetCompanyId()),
            };
        }

        private async Task<ServicioAdminSettingsEditDto> GetServicioAdminSettingsAsync()
        {
            return new ServicioAdminSettingsEditDto
            {
                IsModuleEnabled = true
            };
        }

        private async Task<CompanyBillingSettingsEditDto> GetBillingSettingsAsync()
        {
            return new CompanyBillingSettingsEditDto()
            {
                LegalName = await SettingManager.GetSettingValueAsync(AppSettings.CompanyManagement.BillingLegalName),
                Address = await SettingManager.GetSettingValueAsync(AppSettings.CompanyManagement.BillingAddress),
            };
        }

        private async Task<CompanyOtherSettingsEditDto> GetOtherSettingsAsync()
        {
            return new CompanyOtherSettingsEditDto();
        }

        #endregion

        #region Update Settings

        public async Task UpdateAllSettingsAsync(CompanySettingsEditDto input)
        {
            await UpdateUserManagementSettingsAsync(input.UserManagement);
            await UpdateSecuritySettingsAsync(input.Security);
            await UpdateBillingSettingsAsync(input.Billing);
            await UpdateEmailSettingsAsync(input.Email);

            if (!_multiCompanyConfig.IsEnabled || Clock.SupportsMultipleTimezone)
            {
                if (input.General.Timezone.IsNullOrEmpty())
                {
                    var defaultValue = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Company, KontecgSession.CompanyId);
                    await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), TimingSettingNames.TimeZone, input.General.Timezone);
                }

                await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                    AppSettings.CurrencyManagement.BaseCurrency, input.General.Currency);
            }

            if (!_multiCompanyConfig.IsEnabled)
            {
                await UpdateOtherSettingsAsync(input.OtherSettings);

                input.ValidateHostSettings();
            }

            if (_ldapModuleConfig.IsEnabled)
            {
                await UpdateLdapSettingsAsync(input.Ldap);
            }
            /*
            if (_servicioAdminModuleConfig.IsEnabled)
            {
                await UpdateServicioAdminSettingsAsync(input.ServicioAdmin);
            }
            */

        }

        private async Task UpdateUserManagementSettingsAsync(CompanyUserManagementSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForCompanyAsync(
                KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.AllowSelfRegistration,
                input.AllowSelfRegistration.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForCompanyAsync(
                KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault,
                input.IsNewRegisteredUserActiveByDefault.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForCompanyAsync(
                KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                input.IsEmailConfirmationRequiredForLogin.ToString().ToLowerInvariant()
            );

            await UpdateUserManagementSessionTimeOutSettingsAsync(input.SessionTimeOutSettings);
            await UpdateUserManagementPasswordSettingsAsync(input.UserPasswordSettings);
        }

        private async Task UpdateUserManagementSessionTimeOutSettingsAsync(SessionTimeOutSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForCompanyAsync(
                KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.SessionTimeOut.IsEnabled,
                input.IsEnabled.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForCompanyAsync(
                KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.SessionTimeOut.TimeOutSecond,
                input.TimeOutSecond.ToString()
            );
            await SettingManager.ChangeSettingForCompanyAsync(
                KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond,
                input.ShowTimeOutNotificationSecond.ToString()
            );
            await SettingManager.ChangeSettingForCompanyAsync(
                KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut,
                input.ShowLockScreenWhenTimedOut.ToString().ToLowerInvariant()
            );
        }

        private async Task UpdateUserManagementPasswordSettingsAsync(UserPasswordSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange,
                input.EnableCheckingLastXPasswordWhenPasswordChange.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.Password.CheckingLastXPasswordCount,
                input.CheckingLastXPasswordCount.ToString()
            );

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.Password.EnablePasswordExpiration,
                input.EnablePasswordExpiration.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.Password.PasswordExpirationDayCount,
                input.PasswordExpirationDayCount.ToString()
            );

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                AppSettings.UserManagement.Password.PasswordResetCodeExpirationHours,
                input.PasswordResetCodeExpirationHours.ToString()
            );
        }

        private async Task UpdateSecuritySettingsAsync(SecuritySettingsEditDto settings)
        {
            if (settings.UseDefaultPasswordComplexitySettings)
            {
                await UpdatePasswordComplexitySettingsAsync(settings.DefaultPasswordComplexity);
            }
            else
            {
                if (settings.PasswordComplexity.RequiredLength < settings.PasswordComplexity.AllowedMinimumLength)
                {
                    throw new UserFriendlyException(L("AllowedMinimumLength", settings.PasswordComplexity.AllowedMinimumLength));
                }
                await UpdatePasswordComplexitySettingsAsync(settings.PasswordComplexity);
            }

            await UpdateUserLockOutSettingsAsync(settings.UserLockOut);
            await UpdateOneConcurrentLoginPerUserSettingAsync(settings.AllowOneConcurrentLoginPerUser);
        }

        private async Task UpdatePasswordComplexitySettingsAsync(PasswordComplexitySetting input)
        {
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                input.RequireDigit.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                input.RequireLowercase.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                input.RequireNonAlphanumeric.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                input.RequireUppercase.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                input.RequiredLength.ToString()
            );
        }

        private async Task UpdateUserLockOutSettingsAsync(UserLockOutSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.UserLockOut.IsEnabled,
                input.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                input.DefaultAccountLockoutSeconds.ToString());
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(),
                KontecgBaselineSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout,
                input.MaxFailedAccessAttemptsBeforeLockout.ToString());
        }

        private async Task UpdateOneConcurrentLoginPerUserSettingAsync(bool allowOneConcurrentLoginPerUser)
        {
            if (_multiCompanyConfig.IsEnabled)
                return;
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser, allowOneConcurrentLoginPerUser.ToString().ToLowerInvariant());
        }

        private async Task UpdateBillingSettingsAsync(CompanyBillingSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), AppSettings.CompanyManagement.BillingLegalName, input.LegalName);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), AppSettings.CompanyManagement.BillingAddress, input.Address);
        }

        private async Task UpdateEmailSettingsAsync(CompanyEmailSettingsEditDto input)
        {
            if (_multiCompanyConfig.IsEnabled && !KontecgCoreConsts.AllowCompaniesToChangeEmailSettings)
            {
                return;
            }

            var useHostDefaultEmailSettings = _multiCompanyConfig.IsEnabled && input.UseHostDefaultEmailSettings;

            if (useHostDefaultEmailSettings)
            {
                var smtpPassword = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password);

                input = new CompanyEmailSettingsEditDto
                {
                    UseHostDefaultEmailSettings = true,
                    DefaultFromAddress = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress),
                    DefaultFromDisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                    SmtpHost = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                    SmtpPort = await SettingManager.GetSettingValueForApplicationAsync<int>(EmailSettingNames.Smtp.Port),
                    SmtpUserName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                    SmtpPassword = SimpleStringCipher.Instance.Decrypt(smtpPassword),
                    SmtpDomain = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Domain),
                    SmtpEnableSsl = await SettingManager.GetSettingValueForApplicationAsync<bool>(EmailSettingNames.Smtp.EnableSsl),
                    SmtpUseDefaultCredentials = await SettingManager.GetSettingValueForApplicationAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials)
                };
            }

            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), AppSettings.Email.UseHostDefaultEmailSettings, useHostDefaultEmailSettings.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.DefaultFromAddress, input.DefaultFromAddress);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.DefaultFromDisplayName, input.DefaultFromDisplayName);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.Smtp.Host, input.SmtpHost);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.Smtp.Port, input.SmtpPort.ToString(CultureInfo.InvariantCulture));
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.Smtp.UserName, input.SmtpUserName);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.Smtp.Password, SimpleStringCipher.Instance.Encrypt(input.SmtpPassword));
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.Smtp.Domain, input.SmtpDomain);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.Smtp.EnableSsl, input.SmtpEnableSsl.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), EmailSettingNames.Smtp.UseDefaultCredentials, input.SmtpUseDefaultCredentials.ToString().ToLowerInvariant());
        }

        private async Task UpdateLdapSettingsAsync(LdapSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), LdapSettingNames.IsEnabled, input.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), LdapSettingNames.Domain, input.Domain.IsNullOrWhiteSpace() ? null : input.Domain);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), LdapSettingNames.UserName, input.UserName.IsNullOrWhiteSpace() ? null : input.UserName);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), LdapSettingNames.Password, input.Password.IsNullOrWhiteSpace() ? null : input.Password);
            await SettingManager.ChangeSettingForCompanyAsync(KontecgSession.GetCompanyId(), LdapSettingNames.UseSsl, input.UseSsl.ToString().ToLowerInvariant());
        }

        private async Task UpdateServicioAdminSettingsAsync(ServicioAdminSettingsEditDto input)
        {
        }

        private async Task UpdateOtherSettingsAsync(CompanyOtherSettingsEditDto input)
        {
        }

        #endregion

        #region Others

        public async Task ClearLogoAsync()
        {
            var company = await GetCurrentCompanyAsync();

            if (!company.HasLogo())
            {
                return;
            }

            var logoObject = await _binaryObjectManager.GetOrNullAsync(company.LogoId.Value);
            if (logoObject != null)
            {
                await _binaryObjectManager.DeleteAsync(company.LogoId.Value);
            }

            company.ClearLogo();
        }

        #endregion
    }
}

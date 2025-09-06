using System;
using System.Globalization;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration.Dto;
using Kontecg.Configuration.Host.Dto;
using Kontecg.Extensions;
using Kontecg.Net.Mail;
using Kontecg.Runtime.Security;
using Kontecg.Timing;
using Kontecg.UI;

namespace Kontecg.Configuration.Host
{
    [KontecgAuthorize(PermissionNames.AdministrationHostSettings)]
    public class HostSettingsAppService : SettingsAppServiceBase, IHostSettingsAppService
    {
        private readonly ITimeZoneService _timeZoneService;
        readonly ISettingDefinitionManager _settingDefinitionManager;

        public HostSettingsAppService(
            IEmailSender emailSender,
            ITimeZoneService timeZoneService,
            ISettingDefinitionManager settingDefinitionManager,
            IAppConfigurationAccessor configurationAccessor)
            : base(emailSender, configurationAccessor)
        {
            _timeZoneService = timeZoneService;
            _settingDefinitionManager = settingDefinitionManager;
        }

        #region Get settings

        public async Task<HostSettingsEditDto> GetAllSettingsAsync()
        {
            return new HostSettingsEditDto
            {
                General = await GetGeneralSettingsAsync(),
                UserManagement = await GetUserManagementAsync(),
                Email = await GetEmailSettingsAsync(),
                Security = await GetSecuritySettingsAsync(),
                Billing = await GetBillingSettingsAsync(),
                OtherSettings = await GetOtherSettingsAsync(),
            };
        }

        private async Task<GeneralSettingsEditDto> GetGeneralSettingsAsync()
        {
            var timezone = await SettingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);
            var settings = new GeneralSettingsEditDto
            {
                Timezone = timezone,
                TimezoneForComparison = timezone
            };
            var defaultTimeZoneId =
                await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, KontecgSession.CompanyId);
            if (settings.Timezone == defaultTimeZoneId)
            {
                settings.Timezone = string.Empty;
            }

            var currency =
                await SettingManager.GetSettingValueForApplicationAsync(AppSettings.CurrencyManagement.BaseCurrency);

            settings.Currency = currency;

            return settings;
        }

        private async Task<HostUserManagementSettingsEditDto> GetUserManagementAsync()
        {
            return new HostUserManagementSettingsEditDto
            {
                SessionTimeOutSettings = new SessionTimeOutSettingsEditDto
                {
                    IsEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement
                        .SessionTimeOut.IsEnabled),
                    TimeOutSecond =
                        await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.SessionTimeOut
                            .TimeOutSecond),
                    ShowTimeOutNotificationSecond =
                        await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.SessionTimeOut
                            .ShowTimeOutNotificationSecond),
                    ShowLockScreenWhenTimedOut =
                        await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.SessionTimeOut
                            .ShowLockScreenWhenTimedOut)
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

        private async Task<EmailSettingsEditDto> GetEmailSettingsAsync()
        {
            var smtpPassword = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password);

            return new EmailSettingsEditDto
            {
                DefaultFromAddress = await SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromAddress),
                DefaultFromDisplayName =
                    await SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromDisplayName),
                SmtpHost = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Host),
                SmtpPort = await SettingManager.GetSettingValueAsync<int>(EmailSettingNames.Smtp.Port),
                SmtpUserName = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.UserName),
                SmtpPassword = SimpleStringCipher.Instance.Decrypt(smtpPassword),
                SmtpDomain = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Domain),
                SmtpEnableSsl = await SettingManager.GetSettingValueAsync<bool>(EmailSettingNames.Smtp.EnableSsl),
                SmtpUseDefaultCredentials =
                    await SettingManager.GetSettingValueAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials)
            };
        }

        private async Task<SecuritySettingsEditDto> GetSecuritySettingsAsync()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit =
                    await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireDigit),
                RequireLowercase =
                    await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric =
                    await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase =
                    await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireUppercase),
                RequiredLength =
                    await SettingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity
                        .RequiredLength)
            };

            var defaultPasswordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = Convert.ToBoolean(_settingDefinitionManager
                    .GetSettingDefinition(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit)
                    .DefaultValue),
                RequireLowercase = Convert.ToBoolean(_settingDefinitionManager
                    .GetSettingDefinition(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase)
                    .DefaultValue),
                RequireNonAlphanumeric = Convert.ToBoolean(_settingDefinitionManager
                    .GetSettingDefinition(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric)
                    .DefaultValue),
                RequireUppercase = Convert.ToBoolean(_settingDefinitionManager
                    .GetSettingDefinition(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase)
                    .DefaultValue),
                RequiredLength = Convert.ToInt32(_settingDefinitionManager
                    .GetSettingDefinition(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength)
                    .DefaultValue)
            };

            return new SecuritySettingsEditDto
            {
                UseDefaultPasswordComplexitySettings =
                    passwordComplexitySetting.Equals(defaultPasswordComplexitySetting),
                PasswordComplexity = passwordComplexitySetting,
                DefaultPasswordComplexity = defaultPasswordComplexitySetting,
                UserLockOut = await GetUserLockOutSettingsAsync(),
                AllowOneConcurrentLoginPerUser = await GetOneConcurrentLoginPerUserSettingAsync()
            };
        }

        private async Task<HostBillingSettingsEditDto> GetBillingSettingsAsync()
        {
            return new HostBillingSettingsEditDto
            {
                Country = await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.DefaultCountry),
                State = await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.DefaultState),
                City = await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.DefaultCity),
            };
        }

        private async Task<OtherSettingsEditDto> GetOtherSettingsAsync()
        {
            return new OtherSettingsEditDto();
        }

        private async Task<UserLockOutSettingsEditDto> GetUserLockOutSettingsAsync()
        {
            return new UserLockOutSettingsEditDto
            {
                IsEnabled = await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                    .UserLockOut.IsEnabled),
                MaxFailedAccessAttemptsBeforeLockout =
                    await SettingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames.UserManagement.UserLockOut
                        .MaxFailedAccessAttemptsBeforeLockout),
                DefaultAccountLockoutSeconds =
                    await SettingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames.UserManagement.UserLockOut
                        .DefaultAccountLockoutSeconds)
            };
        }

        private async Task<bool> GetOneConcurrentLoginPerUserSettingAsync()
        {
            return await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement
                .AllowOneConcurrentLoginPerUser);
        }

        #endregion

        #region Update Settings

        public async Task UpdateAllSettingsAsync(HostSettingsEditDto input)
        {
            await UpdateGeneralSettingsAsync(input.General);
            await UpdateUserManagementSettingsAsync(input.UserManagement);
            await UpdateSecuritySettingsAsync(input.Security);
            await UpdateEmailSettingsAsync(input.Email);
            await UpdateBillingSettingsAsync(input.Billing);
            await UpdateOtherSettingsAsync(input.OtherSettings);
        }

        private async Task UpdateGeneralSettingsAsync(GeneralSettingsEditDto input)
        {
            if (Clock.SupportsMultipleTimezone)
            {
                if (input.Timezone.IsNullOrEmpty())
                {
                    var defaultValue =
                        await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, KontecgSession.CompanyId);
                    await SettingManager.ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone,
                        input.Timezone);
                }
            }

            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.CurrencyManagement.BaseCurrency,
                input.Currency);
        }

        private async Task UpdateUserManagementSettingsAsync(HostUserManagementSettingsEditDto input)
        {
            await UpdateUserManagementSessionTimeOutSettingsAsync(input.SessionTimeOutSettings);
            await UpdateUserManagementPasswordSettingsAsync(input.UserPasswordSettings);
        }

        private async Task UpdateUserManagementSessionTimeOutSettingsAsync(SessionTimeOutSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.SessionTimeOut.IsEnabled,
                input.IsEnabled.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.SessionTimeOut.TimeOutSecond,
                input.TimeOutSecond.ToString()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond,
                input.ShowTimeOutNotificationSecond.ToString()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut,
                input.ShowLockScreenWhenTimedOut.ToString().ToLowerInvariant()
            );
        }

        private async Task UpdateUserManagementPasswordSettingsAsync(UserPasswordSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange,
                input.EnableCheckingLastXPasswordWhenPasswordChange.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.Password.CheckingLastXPasswordCount,
                input.CheckingLastXPasswordCount.ToString()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.Password.EnablePasswordExpiration,
                input.EnablePasswordExpiration.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.Password.PasswordExpirationDayCount,
                input.PasswordExpirationDayCount.ToString()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.Password.PasswordResetCodeExpirationHours,
                input.PasswordResetCodeExpirationHours.ToString()
            );
        }

        private async Task UpdateSecuritySettingsAsync(SecuritySettingsEditDto input)
        {
            if (input.UseDefaultPasswordComplexitySettings)
            {
                await UpdatePasswordComplexitySettingsAsync(input.DefaultPasswordComplexity);
            }
            else
            {
                if (input.PasswordComplexity.RequiredLength < input.PasswordComplexity.AllowedMinimumLength)
                {
                    throw new UserFriendlyException(L("AllowedMinimumLength", input.PasswordComplexity.AllowedMinimumLength));
                }

                await UpdatePasswordComplexitySettingsAsync(input.PasswordComplexity);
            }

            await UpdateUserLockOutSettingsAsync(input.UserLockOut);
            await UpdateOneConcurrentLoginPerUserSettingAsync(input.AllowOneConcurrentLoginPerUser);
        }

        private async Task UpdatePasswordComplexitySettingsAsync(PasswordComplexitySetting input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                input.RequireDigit.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                input.RequireLowercase.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                input.RequireNonAlphanumeric.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                input.RequireUppercase.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                input.RequiredLength.ToString()
            );
        }

        private async Task UpdateUserLockOutSettingsAsync(UserLockOutSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(
                KontecgBaselineSettingNames.UserManagement.UserLockOut.IsEnabled,
                input.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(
                KontecgBaselineSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                input.DefaultAccountLockoutSeconds.ToString());
            await SettingManager.ChangeSettingForApplicationAsync(
                KontecgBaselineSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout,
                input.MaxFailedAccessAttemptsBeforeLockout.ToString());
        }

        private async Task UpdateOneConcurrentLoginPerUserSettingAsync(bool allowOneConcurrentLoginPerUser)
        {
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.AllowOneConcurrentLoginPerUser, allowOneConcurrentLoginPerUser.ToString().ToLowerInvariant());
        }

        private async Task UpdateEmailSettingsAsync(EmailSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromAddress,
                input.DefaultFromAddress);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromDisplayName,
                input.DefaultFromDisplayName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Host, input.SmtpHost);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Port,
                input.SmtpPort.ToString(CultureInfo.InvariantCulture));
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UserName,
                input.SmtpUserName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Password,
                SimpleStringCipher.Instance.Encrypt(input.SmtpPassword));
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Domain, input.SmtpDomain);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.EnableSsl,
                input.SmtpEnableSsl.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials,
                input.SmtpUseDefaultCredentials.ToString().ToLowerInvariant());
        }

        private async Task UpdateBillingSettingsAsync(HostBillingSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.HostManagement.DefaultCountry,
                input.Country);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.HostManagement.DefaultState,
                input.State);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.HostManagement.DefaultCity,
                input.City);
        }

        private async Task UpdateOtherSettingsAsync(OtherSettingsEditDto input)
        {
        }

        #endregion
    }
}

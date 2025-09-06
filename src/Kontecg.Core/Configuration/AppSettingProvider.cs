using System.Collections.Generic;
using System.Linq;
using Kontecg.Domain;
using Kontecg.Json;
using Kontecg.Net.Mail;
using Kontecg.Timing;
using Microsoft.Extensions.Configuration;
using NMoneys;

namespace Kontecg.Configuration
{
    /// <summary>
    ///     Defines settings for the application.
    ///     See <see cref="AppSettings" /> for setting names.
    /// </summary>
    public class AppSettingProvider : SettingProvider
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly VisibleSettingClientVisibilityProvider _visibleSettingClientVisibilityProvider;

        public AppSettingProvider(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
            _visibleSettingClientVisibilityProvider = new VisibleSettingClientVisibilityProvider();
        }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            // Change scope of Email settings
            ChangeEmailSettingScopes(context);

            return GetHostSettings()
                .Union(GetCompanySettings())
                .Union(GetSharedSettings());
        }

        private void ChangeEmailSettingScopes(SettingDefinitionProviderContext context)
        {
            if (!KontecgCoreConsts.AllowCompaniesToChangeEmailSettings)
            {
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.Host).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.Port).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.UserName).Scopes =
                    SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.Password).Scopes =
                    SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.Domain).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.EnableSsl).Scopes =
                    SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials).Scopes =
                    SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.DefaultFromAddress).Scopes =
                    SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.DefaultFromDisplayName).Scopes =
                    SettingScopes.Application;
            }
        }

        private IEnumerable<SettingDefinition> GetHostSettings()
        {
            return
            [
                new SettingDefinition(AppSettings.CompanyManagement.AllowSelfRegistration,
                    GetFromAppSettings(AppSettings.CompanyManagement.AllowSelfRegistration, "true"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),
                new SettingDefinition(AppSettings.CompanyManagement.IsNewRegisteredCompanyActiveByDefault,
                    GetFromAppSettings(AppSettings.CompanyManagement.IsNewRegisteredCompanyActiveByDefault, "true")),
                new SettingDefinition(AppSettings.HostManagement.DefaultCountry,
                    GetFromAppSettings(AppSettings.HostManagement.DefaultCountry, "Cuba")),
                new SettingDefinition(AppSettings.HostManagement.DefaultState,
                    GetFromAppSettings(AppSettings.HostManagement.DefaultState, "")),
                new SettingDefinition(AppSettings.HostManagement.DefaultCity,
                    GetFromAppSettings(AppSettings.HostManagement.DefaultCity, "")),

                new SettingDefinition(AppSettings.UserManagement.UserExpirationDayCount,
                    GetFromAppSettings(AppSettings.UserManagement.UserExpirationDayCount, "30"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),

                new SettingDefinition(AppSettings.UserManagement.IsPersonRequiredForLogin,
                    GetFromAppSettings(AppSettings.UserManagement.IsPersonRequiredForLogin, "true"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),

                new SettingDefinition(AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange,
                    GetFromAppSettings(AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange, "false"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),

                new SettingDefinition(AppSettings.UserManagement.Password.CheckingLastXPasswordCount,
                    GetFromAppSettings(AppSettings.UserManagement.Password.CheckingLastXPasswordCount, "3"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),

                new SettingDefinition(AppSettings.UserManagement.Password.EnablePasswordExpiration,
                    GetFromAppSettings(AppSettings.UserManagement.Password.EnablePasswordExpiration, "false"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),

                new SettingDefinition(AppSettings.UserManagement.Password.PasswordExpirationDayCount,
                    GetFromAppSettings(AppSettings.UserManagement.Password.PasswordExpirationDayCount, "45"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),

                new SettingDefinition(AppSettings.UserManagement.Password.PasswordResetCodeExpirationHours,
                    GetFromAppSettings(AppSettings.UserManagement.Password.PasswordResetCodeExpirationHours, "24"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider)
            ];
        }

        private IEnumerable<SettingDefinition> GetCompanySettings()
        {
            return
            [
                new SettingDefinition(AppSettings.UserManagement.AllowSelfRegistration,
                    GetFromAppSettings(AppSettings.UserManagement.AllowSelfRegistration, "true"),
                    scopes: SettingScopes.Company, clientVisibilityProvider: _visibleSettingClientVisibilityProvider),
                new SettingDefinition(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault,
                    GetFromAppSettings(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault, "true"),
                    scopes: SettingScopes.Company),
                new SettingDefinition(AppSettings.CompanyManagement.BillingLegalName,
                    GetFromAppSettings(AppSettings.CompanyManagement.BillingLegalName, ""),
                    scopes: SettingScopes.Company),
                new SettingDefinition(AppSettings.CompanyManagement.BillingAddress,
                    GetFromAppSettings(AppSettings.CompanyManagement.BillingAddress, ""), scopes: SettingScopes.Company),
                new SettingDefinition(AppSettings.Email.UseHostDefaultEmailSettings,
                    GetFromAppSettings(AppSettings.Email.UseHostDefaultEmailSettings,
                        KontecgCoreConsts.MultiCompanyEnabled ? "true" : "false"), scopes: SettingScopes.Company)
            ];
        }

        private IEnumerable<SettingDefinition> GetSharedSettings()
        {
            return new[]
            {
                new SettingDefinition(AppSettings.UserManagement.SessionTimeOut.IsEnabled,
                    GetFromAppSettings(AppSettings.UserManagement.SessionTimeOut.IsEnabled, "false"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(AppSettings.UserManagement.SessionTimeOut.TimeOutSecond,
                    GetFromAppSettings(AppSettings.UserManagement.SessionTimeOut.TimeOutSecond, "30"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(AppSettings.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond,
                    GetFromAppSettings(AppSettings.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond, "30"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(AppSettings.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut,
                    GetFromAppSettings(AppSettings.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut, "false"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser,
                    GetFromAppSettings(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser, "false"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(AppSettings.UserManagement.UseBlobStorageProfilePicture,
                    GetFromAppSettings(AppSettings.UserManagement.UseBlobStorageProfilePicture, "false"),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(
                    AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate + "." + CurrencyIsoCode.USD,
                    GetFromAppSettings(
                        AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate + "." + CurrencyIsoCode.USD,
                        ScopeData.Company.ToString()),
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(AppSettings.PersonManagement.MustHavePhoto, "true",
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),
                new SettingDefinition(AppSettings.PersonManagement.MustHaveAddress, "false",
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),
                new SettingDefinition(AppSettings.PersonManagement.MustHaveEtnia, "false",
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),
                new SettingDefinition(AppSettings.PersonManagement.MustHaveClothingSizes, "false",
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),

                new SettingDefinition(AppSettings.Timing.OpenModeManagement, PeriodOpenMode.LastMonth.ToString(),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider),

            }.Union(GetCurrenciesSettings());
        }

        private IEnumerable<SettingDefinition> GetCurrenciesSettings()
        {
            var currenciesScopedSettings = new List<SettingDefinition>();
            var keys = new[] { KontecgCoreConsts.DefaultCurrency, "USD", "CAD", "CHF", "EUR", "GBP", "JPY", "MXN" };

            currenciesScopedSettings.Add(new SettingDefinition(AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate,
                GetFromAppSettings(AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate, ScopeData.Company.ToString()),
                clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company));

            currenciesScopedSettings.Add(new SettingDefinition(AppSettings.CurrencyManagement.BaseCurrency,
                GetFromAppSettings(AppSettings.CurrencyManagement.BaseCurrency, KontecgCoreConsts.DefaultCurrency),
                clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company));

            currenciesScopedSettings.Add(new SettingDefinition(AppSettings.CurrencyManagement.AllowedCurrencies,
                GetFromAppSettings(AppSettings.CurrencyManagement.AllowedCurrencies, keys.ToJsonString()),
                clientVisibilityProvider: _visibleSettingClientVisibilityProvider, scopes: SettingScopes.Application | SettingScopes.Company));

            currenciesScopedSettings.AddRange(keys.Select(key => new SettingDefinition(
                AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate + "." + key.ToUpper(),
                GetFromAppSettings(AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate + "." + key.ToUpper(),
                    ScopeData.Company.ToString()), clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                scopes: SettingScopes.Application | SettingScopes.Company)));

            return currenciesScopedSettings;
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

using System.Linq;
using Castle.Core.Logging;
using Kontecg.Baseline.Ldap.Configuration;
using Kontecg.Configuration;
using Kontecg.Domain;
using Kontecg.EFCore;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Net.Http;
using Kontecg.Net.Mail;
using Kontecg.Runtime.Security;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Migrations.Seed.Host
{
    public class DefaultSettingsBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly ILogger _logger;

        public DefaultSettingsBuilder(KontecgCoreDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Create()
        {
            int? companyId = null;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!KontecgCoreConsts.MultiCompanyEnabled)
#pragma warning disable 162
            {
                companyId = MultiCompanyConsts.DefaultCompanyId;
            }
#pragma warning restore 162

            //Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admSystem@ecg.moa.minem.cu", companyId);
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "Sistema de Notificación de Aplicaciones", companyId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.UseDefaultCredentials, false.ToString().ToLower(), companyId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.Domain, "ecg", companyId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.UserName, "ServiceChenet", companyId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.Password, SimpleStringCipher.Instance.Encrypt("Caribe2014**"), companyId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.Host,"172.22.16.129", companyId);

            //Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "es", companyId);

            //Ldap
            AddSettingIfNotExists(LdapSettingNames.IsEnabled, true.ToString().ToLower(), MultiCompanyConsts.DefaultCompanyId);
            AddSettingIfNotExists(LdapSettingNames.Domain, "172.22.16.8", MultiCompanyConsts.DefaultCompanyId);
            AddSettingIfNotExists(LdapSettingNames.UserName, @"ecg\adminsql1", MultiCompanyConsts.DefaultCompanyId);
            AddSettingIfNotExists(LdapSettingNames.Password,SimpleStringCipher.Instance.Encrypt("SystemManagerSql!464"), MultiCompanyConsts.DefaultCompanyId);
            //ServicioAdmin
            //AddSettingIfNotExists(ServicioAdminSettingNames.IsEnabled, false.ToString().ToLower(), MultiCompanyConsts.DefaultCompanyId);
            //Proxy
            AddSettingIfNotExists(HttpSettingNames.UseDefaultProxy, false.ToString().ToLower(), null);
            AddSettingIfNotExists(HttpSettingNames.UseDefaultProxy, true.ToString().ToLower(), companyId);
            AddSettingIfNotExists(HttpSettingNames.Proxy.Uri, "http://172.22.16.145:8080", companyId);
            AddSettingIfNotExists(HttpSettingNames.Proxy.Domain, "ecg", companyId);
            AddSettingIfNotExists(HttpSettingNames.Proxy.UserName,"mfsuarez", companyId);
            AddSettingIfNotExists(HttpSettingNames.Proxy.Password, SimpleStringCipher.Instance.Encrypt("Caribe2024*"), companyId);
            AddSettingIfNotExists(HttpSettingNames.Proxy.BypassOnLocal, true.ToString().ToLower(), companyId);
            AddSettingIfNotExists(HttpSettingNames.Proxy.BypassList, "localhost;127.0.0.1;.ecg.moa.minbas.cu;172.22.0.0/16;192.168.0.0/16", companyId);
            //Currencies & Exchange Rates
            AddSettingIfNotExists(AppSettings.CurrencyManagement.BaseCurrency, KontecgCoreConsts.DefaultCurrency, companyId);
            AddSettingIfNotExists(AppSettings.CurrencyManagement.AllowedCurrencies, $"[\"{KontecgCoreConsts.DefaultCurrency}\",\"USD\",\"CAD\",\"CHF\",\"EUR\",\"GBP\",\"JPY\",\"MXN\"]", companyId);
            AddSettingIfNotExists(AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate + ".USD", ScopeData.Personal.ToString(), companyId);

            //Login
            AddSettingIfNotExists(AppSettings.UserManagement.IsPersonRequiredForLogin, true.ToString().ToLower(), companyId);

            //Person management
            AddSettingIfNotExists(AppSettings.PersonManagement.MustHavePhoto, true.ToString().ToLower(), companyId);
            AddSettingIfNotExists(AppSettings.PersonManagement.MustHaveAddress, true.ToString().ToLower(), companyId);
            AddSettingIfNotExists(AppSettings.PersonManagement.MustHaveEtnia, true.ToString().ToLower(), companyId);
            AddSettingIfNotExists(AppSettings.PersonManagement.MustHaveClothingSizes, true.ToString().ToLower(), companyId);

            //Theme
            AddSettingIfNotExists("WinForms.Theme.AllowChangeTheme", true.ToString().ToLower(), companyId);
            AddSettingIfNotExists("WinForms.Theme.SkinName", "WXI", companyId);
            AddSettingIfNotExists("WinForms.Theme.PaletteName", "Darkness", companyId);
            AddSettingIfNotExists("WinForms.Theme.ForceCompactUIMode", true.ToString().ToLower(), companyId);
            AddSettingIfNotExists("WinForms.AuthManagement.RememberLastLogin", true.ToString().ToLower(), companyId);
            
            _logger.Info("Settings created.");
        }

        private void AddSettingIfNotExists(string name, string value, int? companyId = null)
        {
            if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.CompanyId == companyId && s.UserId == null))
                return;

            _context.Settings.Add(new Setting(companyId, null, name, value));
            _context.SaveChanges();
        }
    }
}

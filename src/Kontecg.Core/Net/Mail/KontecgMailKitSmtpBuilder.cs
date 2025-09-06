using Kontecg.Localization;
using Kontecg.MailKit;
using Kontecg.Net.Mail.Smtp;
using Kontecg.UI;
using MailKit.Net.Smtp;

namespace Kontecg.Net.Mail
{
    public class KontecgMailKitSmtpBuilder : DefaultMailKitSmtpBuilder
    {
        private readonly ILocalizationManager _localizationManager;
        private readonly IEmailSettingsChecker _emailSettingsChecker;

        public KontecgMailKitSmtpBuilder(
            ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
            IKontecgMailKitConfiguration kontecgMailKitConfiguration,
            ILocalizationManager localizationManager,
            IEmailSettingsChecker emailSettingsChecker)
            : base(smtpEmailSenderConfiguration,
            kontecgMailKitConfiguration)
        {
            _localizationManager = localizationManager;
            _emailSettingsChecker = emailSettingsChecker;
        }

        protected override void ConfigureClient(SmtpClient client)
        {
            if (!_emailSettingsChecker.EmailSettingsValid())
            {
                throw new UserFriendlyException(L("SMTPSettingsNotProvidedWarningText"));
            }

            client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            base.ConfigureClient(client);
        }

        private string L(string name)
        {
            return _localizationManager.GetString(KontecgCoreConsts.LocalizationSourceName, name);
        }
    }
}

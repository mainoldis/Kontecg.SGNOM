using Kontecg.Configuration;
using Kontecg.Net.Mail.Smtp;
using Kontecg.Runtime.Security;

namespace Kontecg.Net.Mail
{
    public class KontecgSmtpEmailSenderConfiguration : SmtpEmailSenderConfiguration
    {
        public KontecgSmtpEmailSenderConfiguration(ISettingManager settingManager) : base(settingManager)
        {
        }

        public override string Password =>
            SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Password));
    }
}

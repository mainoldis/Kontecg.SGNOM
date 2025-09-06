using Kontecg.Configuration.Host.Dto;
using System.Threading.Tasks;
using System;
using Kontecg.Net.Mail;
using Kontecg.UI;

namespace Kontecg.Configuration
{
    public abstract class SettingsAppServiceBase : KontecgAppServiceBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IAppConfigurationAccessor _configurationAccessor;

        protected SettingsAppServiceBase(
            IEmailSender emailSender,
            IAppConfigurationAccessor configurationAccessor)
        {
            _emailSender = emailSender;
            _configurationAccessor = configurationAccessor;
        }

        public async Task SendTestEmailAsync(SendTestEmailInput input)
        {
            try
            {
                await _emailSender.SendAsync(
                    input.EmailAddress,
                    L("TestEmail_Subject"),
                    L("TestEmail_Body")
                );
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("An error was encountered while sending an email. " + e.Message, e);
            }
        }
    }
}

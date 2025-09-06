using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kontecg.Configuration;
using Kontecg.Debugging;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Kontecg.Net.Mail;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Session;
using Kontecg.UI;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Used to send email to users.
    /// </summary>
    public class UserMailer : KontecgCoreServiceBase, IUserMailer, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IKontecgSession _kontecgSession;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly UserManager _userManager;
        private readonly string _emailButtonColor = "#00bb77";

        // used for styling action links on email messages.
        private readonly string _emailButtonStyle =
            "padding-left: 30px; padding-right: 30px; padding-top: 12px; padding-bottom: 12px; color: #ffffff; background-color: #00bb77; font-size: 14pt; text-decoration: none;";

        public UserMailer(
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<Company> companyRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            UserManager userManager,
            IKontecgSession kontecgSession)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _companyRepository = companyRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
            _userManager = userManager;
            _kontecgSession = kontecgSession;
        }

        /// <summary>
        ///     Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        ///     Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link = null, string plainPassword = null)
        {
            await CheckMailSettingsEmptyOrNullAsync();

            if (user.EmailConfirmationCode.IsNullOrEmpty())
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.CompanyId.HasValue) link = link.Replace("{companyId}", user.CompanyId.ToString());

            link = EncryptQueryParameters(link);

            var companyName = GetCompanyNameOrNull(user.CompanyId);
            var emailTemplate =
                GetTitleAndSubTitle(user.CompanyId, L("EmailActivation_Title"), L("EmailActivation_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!companyName.IsNullOrEmpty())
                mailMessage.AppendLine("<b>" + L("CompanyName") + "</b>: " + companyName + "<br />");

            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");

            if (!plainPassword.IsNullOrEmpty())
                mailMessage.AppendLine("<b>" + L("Password") + "</b>: " + plainPassword + "<br />");

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor +
                                   "\" href=\"" + link + "\">" + L("Verify") + "</a>");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" +
                                   L("EmailMessage_CopyTheLinkBelowToYourBrowser") + "</span><br />");
            mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");

            await ReplaceBodyAndSendAsync(user.EmailAddress, L("EmailActivation_Subject"), emailTemplate, mailMessage);
        }

        /// <summary>
        ///     Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Reset link</param>
        public async Task SendPasswordResetLinkAsync(User user, string link = null)
        {
            await CheckMailSettingsEmptyOrNullAsync();

            if (user.PasswordResetCode.IsNullOrEmpty())
                throw new Exception("PasswordResetCode should be set in order to send password reset link.");

            var companyName = GetCompanyNameOrNull(user.CompanyId);
            var emailTemplate = GetTitleAndSubTitle(user.CompanyId, L("PasswordResetEmail_Title"),
                L("PasswordResetEmail_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!companyName.IsNullOrEmpty())
                mailMessage.AppendLine("<b>" + L("CompanyName") + "</b>: " + companyName + "<br />");

            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<b>" + L("ResetCode") + "</b>: " + user.PasswordResetCode + "<br />");

            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

                if (user.CompanyId.HasValue) link = link.Replace("{companyId}", user.CompanyId.ToString());

                link = EncryptQueryParameters(link);

                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
                mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor +
                                       "\" href=\"" + link + "\">" + L("Reset") + "</a>");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" +
                                       L("EmailMessage_CopyTheLinkBelowToYourBrowser") + "</span><br />");
                mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");
            }

            await ReplaceBodyAndSendAsync(user.EmailAddress, L("PasswordResetEmail_Subject"), emailTemplate, mailMessage);
        }

        private string GetCompanyNameOrNull(int? companyId)
        {
            if (companyId == null) return null;

            using (_unitOfWorkProvider.Current.SetCompanyId(null))
            {
                return _companyRepository.Get(companyId.Value).CompanyName;
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? companyId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(companyId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private async Task ReplaceBodyAndSendAsync(string emailAddress, string subject, StringBuilder emailTemplate,
            StringBuilder mailMessage)
        {
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());
            await _emailSender.SendAsync(new MailMessage
            {
                To = {emailAddress},
                Subject = subject,
                Body = emailTemplate.ToString(),
                IsBodyHtml = true
            });
        }

        /// <summary>
        ///     Returns link with encrypted parameters
        /// </summary>
        /// <param name="link"></param>
        /// <param name="encryptedParameterName"></param>
        /// <returns></returns>
        private string EncryptQueryParameters(string link, string encryptedParameterName = "c")
        {
            if (!link.Contains("?")) return link;

            var basePath = link.Substring(0, link.IndexOf('?'));
            var query = link.Substring(link.IndexOf('?')).TrimStart('?');

            return basePath + "?" + encryptedParameterName + "=" +
                   HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(query));
        }

        private async Task CheckMailSettingsEmptyOrNullAsync()
        {
            if (DebugHelper.IsDebug) return;

            if (
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromAddress)).IsNullOrEmpty() ||
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Host)).IsNullOrEmpty()
            )
                throw new UserFriendlyException(L("SMTPSettingsNotProvidedWarningText"));

            if (await _settingManager.GetSettingValueAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials)) return;

            if (
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.UserName)).IsNullOrEmpty() ||
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password)).IsNullOrEmpty()
            )
                throw new UserFriendlyException(L("SMTPSettingsNotProvidedWarningText"));
        }
    }
}

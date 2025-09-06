using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Kontecg.Authorization.Users;
using Kontecg.Configuration;
using Kontecg.Debugging;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.IO;
using Kontecg.Net.Mail;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Threading;
using Kontecg.UI;

namespace Kontecg.ExceptionHandling
{
    public class ExceptionMailer : KontecgCoreServiceBase, IExceptionMailer, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly UserManager _userManager;
        private readonly IContentFolders _appFolders;

        public ExceptionMailer(
            IEmailSender emailSender,
            IEmailTemplateProvider emailTemplateProvider,
            IErrorInfoBuilder errorInfoBuilder,
            UserManager userManager,
            IContentFolders appFolders)
        {
            _emailSender = emailSender;
            _emailTemplateProvider = emailTemplateProvider;
            _errorInfoBuilder = errorInfoBuilder;
            _userManager = userManager;
            _appFolders = appFolders;
        }

        /// <inheritdoc />
        public void Send(IKontecgSession kontecgSession, Exception exception, string message, TempFileInfo attachment = null)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                CheckMailSettingsEmptyOrNull();
                User supportUser = AsyncHelper.RunSync(() => _userManager.FindByNameAsync(KontecgUserBase.AdminUserName));
                User replyToUser = kontecgSession.UserId.HasValue
                    ? _userManager.GetUserOrNull(kontecgSession.ToUserIdentifier())
                    : null;

                var emailTemplate = GetTitleAndSubTitle(supportUser.CompanyId, L("EmailError_Title"), L("EmailError_SubTitle"));
                var mailMessage = new StringBuilder();

                if (replyToUser != null)
                {
                    mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + replyToUser.Name + " " + replyToUser.Surname + "<br />");
                    mailMessage.AppendLine("<b>" + L("EmailAddress") + "</b>: " + replyToUser.EmailAddress + "<br />");
                }

                if(!message.IsNullOrEmpty())
                    mailMessage.AppendLine("<b>" + L("EmailMessage") + "</b>: " + message + "<br />");

                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine(GetExceptionMessage(exception));
                var logLines = GetLast100LinesLog();
                var firstLine = true;
                foreach (var line in logLines)
                {
                    if (line.StartsWith("DEBUG"))
                    {
                        if (!firstLine) mailMessage.Append("</font>");
                        mailMessage.Append("<font color='#858796'>");
                    }

                    if (line.StartsWith("INFO"))
                    {
                        if (!firstLine) mailMessage.Append("</font>");
                        mailMessage.Append("<font color='#4e73df'>");
                    }

                    if (line.StartsWith("WARN"))
                    {
                        if (!firstLine) mailMessage.Append("</font>");
                        mailMessage.Append("<font color='#a53415'>" );
                    }

                    if (line.StartsWith("ERROR"))
                    {
                        if (!firstLine) mailMessage.Append("</font>");
                        mailMessage.Append("<font color='#e74a3b'>");
                    }

                    if (line.StartsWith("FATAL"))
                    {
                        if (!firstLine) mailMessage.Append("</font>");
                        mailMessage.Append("<font color='#e74a3b'>");
                    }

                    mailMessage.AppendLine(line + "<br />");
                    firstLine = false;
                }

                if (logLines.Count > 0) mailMessage.Append("</font>");

                mailMessage.AppendLine();
                emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());
                var mailToSend = new MailMessage
                {
                    To = { supportUser.EmailAddress },
                    Subject = L("EmailError_Subject"),
                    Body = emailTemplate.ToString(),
                    IsBodyHtml = true
                };

                if (replyToUser != null)
                {
                    mailToSend.ReplyToList.Clear();
                    mailToSend.ReplyToList.Add(replyToUser.EmailAddress);
                }

                if (attachment != null && attachment.File.Length > 0)
                {
                    var attachmentFileStream = new MemoryStream(attachment.File);
                    mailToSend.Attachments.Clear();
                    mailToSend.Attachments.Add(new Attachment(attachmentFileStream, attachment.FileName, attachment.FileType));
                }

                _emailSender.Send(mailToSend);
            });
        }

        private StringBuilder GetTitleAndSubTitle(int? companyId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(companyId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private string GetExceptionMessage(Exception exception)
        {
            var errorMessage = new StringBuilder();
            var errorInfo = _errorInfoBuilder.BuildForException(exception);
            if (errorInfo != null)
            {
                if(errorInfo.Code > 0)
                    errorMessage.AppendLine("<b>" + L("ErrorCode") + "</b>: " + errorInfo.Code + "<br />");

                errorMessage.AppendLine("<b>" + L("ErrorMessage") + "</b>: " + errorInfo.Message + "<br />");
                if(!errorInfo.Details.IsNullOrWhiteSpace())
                    errorMessage.AppendLine("<b>" + L("ErrorDetails") + "</b>: " + errorInfo.Details + "<br />");

                if (errorInfo.ValidationErrors?.Length > 0)
                {
                    errorMessage.AppendLine("<b>" + L("ErrorValidations") + "</b>:<br />");
                    foreach (var t in errorInfo.ValidationErrors)
                    {
                        errorMessage.AppendLine("<ol>");
                        errorMessage.AppendLine("<li>" + t.Message + "</li>");
                        errorMessage.AppendLine("</ol>");
                    }
                }

                errorMessage.AppendLine("<br />");
            }

            return errorMessage.ToString();
        }

        private List<string> GetLast100LinesLog()
        {
            var directory = new DirectoryInfo(_appFolders.LogsFolder);
            if (!directory.Exists) return new List<string>();

            var lastLogFile = directory.GetFiles("*.txt", SearchOption.AllDirectories).MaxBy(f => f.LastWriteTime);

            if (lastLogFile == null) return new List<string>();

            var lines = AppFileHelper.ReadLines(lastLogFile.FullName).Reverse().Take(KontecgCoreConsts.DefaultPageSize)
                .ToList();
            var logLineCount = 0;
            var lineCount = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("DEBUG") ||
                    line.StartsWith("INFO") ||
                    line.StartsWith("WARN") ||
                    line.StartsWith("ERROR") ||
                    line.StartsWith("FATAL"))
                    logLineCount++;

                lineCount++;

                if (logLineCount == 100) break;
            }

            return lines.Take(lineCount).Reverse().ToList();
        }

        private void CheckMailSettingsEmptyOrNull()
        {
            if (DebugHelper.IsDebug) return;

            if ((SettingManager.GetSettingValue(EmailSettingNames.DefaultFromAddress)).IsNullOrEmpty() ||
                (SettingManager.GetSettingValue(EmailSettingNames.Smtp.Host)).IsNullOrEmpty())
                throw new UserFriendlyException(L("SMTPSettingsNotProvidedWarningText"));

            if (SettingManager.GetSettingValue<bool>(EmailSettingNames.Smtp.UseDefaultCredentials)) return;

            if (
                (SettingManager.GetSettingValue(EmailSettingNames.Smtp.UserName)).IsNullOrEmpty() ||
                (SettingManager.GetSettingValue(EmailSettingNames.Smtp.Password)).IsNullOrEmpty()
            )
                throw new UserFriendlyException(L("SMTPSettingsNotProvidedWarningText"));
        }
    }
}

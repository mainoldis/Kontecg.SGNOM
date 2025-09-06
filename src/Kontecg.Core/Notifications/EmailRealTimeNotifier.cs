using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Kontecg.Authorization.Users;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.Net.Mail;
using System.Text;
using Kontecg.Linq;

namespace Kontecg.Notifications
{
    public class EmailRealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        public bool UseOnlyIfRequestedAsTarget => true;

        public ILogger Logger { get; set; }
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateProvider _templateProvider;
        private readonly IRepository<User, long> _userRepository;

        public EmailRealTimeNotifier(
            IUnitOfWorkManager unitOfWorkManager,
            IEmailSender emailSender,
            IEmailTemplateProvider templateProvider,
            IRepository<User, long> userRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _emailSender = emailSender;
            _templateProvider = templateProvider;
            _userRepository = userRepository;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            Logger = NullLogger.Instance;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public async Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            var userNotificationsGroupedByCompany = userNotifications.GroupBy(un => un.CompanyId);
            foreach (var userNotificationByCompany in userNotificationsGroupedByCompany)
            {
                var companyId = userNotificationByCompany.First().CompanyId;
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    var allUserIds = userNotificationByCompany.ToList().Select(x => x.UserId).Distinct().ToList();
                    var usersToNotify = await AsyncQueryableExecuter.ToListAsync((await _userRepository.GetAllAsync())
                        .Where(x => allUserIds.Contains(x.Id))
                        .Select(u => new
                            {
                                u.Id,
                                u.EmailAddress,
                                u.Name
                            }
                        ));

                    foreach (var userNotification in userNotificationByCompany)
                    {
                        if (!userNotification.Notification.Data.Properties.ContainsKey("Message") ||
                            userNotification.Notification.Data["Message"] is not string)
                        {
                            Logger.Info(
                                "Message property is not found in notification data. Notification cannot be sent.");
                            continue;
                        }

                        var user = usersToNotify.FirstOrDefault(x => x.Id == userNotification.UserId);
                        if (user == null)
                        {
                            Logger.Info("Can not send notification to user: " + userNotification.UserId +
                                        ". User does not exists!");
                            continue;
                        }

                        if (user.EmailAddress.IsNullOrWhiteSpace())
                        {
                            Logger.Info("Can not send email to user: " + user.Name + ". User's email is empty!");
                            continue;
                        }

                        var title =
                            userNotification.Notification.Data.Properties.ContainsKey("Title") &&
                            userNotification.Notification.Data["Title"] is string
                                ? userNotification.Notification.Data["Title"].ToString()
                                : string.Empty;

                        var subTitle = userNotification.Notification.Data.Properties.ContainsKey("Subtitle") &&
                                       userNotification.Notification.Data["Subtitle"] is string
                            ? userNotification.Notification.Data["Subtitle"].ToString()
                            : string.Empty;

                        var subject = userNotification.Notification.Data.Properties.ContainsKey("Subject") &&
                                      userNotification.Notification.Data["Subject"] is string
                            ? userNotification.Notification.Data["Subject"].ToString()
                            : string.Empty;

                        var builder = GetTitleAndSubTitle(companyId, title, subTitle);

                        await ReplaceBodyAndSendAsync(user.EmailAddress, subject, builder,
                            userNotification.Notification.Data["Message"].ToString() ?? string.Empty);
                    }
                }
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? companyId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_templateProvider.GetDefaultTemplate(companyId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private async Task ReplaceBodyAndSendAsync(string emailAddress, string subject, StringBuilder emailTemplate, string mailMessage)
        {
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage);
            await _emailSender.SendAsync(new MailMessage
            {
                To = { emailAddress },
                Subject = subject,
                Body = emailTemplate.ToString(),
                IsBodyHtml = true
            });
        }
    }
}

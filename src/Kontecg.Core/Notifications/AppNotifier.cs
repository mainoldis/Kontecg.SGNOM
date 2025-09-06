using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Authorization.Users;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Timing;

namespace Kontecg.Notifications
{
    public class AppNotifier : KontecgCoreDomainServiceBase, IAppNotifier
    {
        private readonly INotificationPublisher _notificationPublisher;

        public AppNotifier(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public async Task WelcomeToTheApplicationAsync(User user)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.WelcomeToTheApplication,
                new MessageNotificationData(L("WelcomeToTheApplicationNotificationMessage")),
                severity: NotificationSeverity.Success,
                userIds: new[] { user.ToUserIdentifier() }
            );
        }

        public async Task NewCompanyRegisteredAsync(Company company)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "NewCompanyRegisteredNotificationMessage",
                    KontecgCoreConsts.LocalizationSourceName
                )
            )
            {
                ["companyName"] = company.CompanyName
            };

            await _notificationPublisher.PublishAsync(AppNotificationNames.NewCompanyRegistered, notificationData);
        }

        public async Task SendMessageAsync(UserIdentifier user, string message,
            NotificationSeverity severity = NotificationSeverity.Info)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.SimpleMessage,
                new MessageNotificationData(message),
                severity: severity,
                userIds: new[] {user}
            );
        }

        public async Task SendMessageAsync(string notificationName, string message, UserIdentifier[] userIds = null,
            NotificationSeverity severity = NotificationSeverity.Info)
        {
            var companies = NotificationPublisher.AllCompanies;
            await _notificationPublisher.PublishAsync(
                notificationName: notificationName,
                new MessageNotificationData(message),
                severity: severity,
                userIds: userIds
            );
        }

        public Task SendMessageAsync(UserIdentifier user, LocalizableString localizableMessage,
            IDictionary<string, object> localizableMessageData = null,
            NotificationSeverity severity = NotificationSeverity.Info)
        {
            return SendNotificationAsync(AppNotificationNames.SimpleMessage, user, localizableMessage,
                localizableMessageData, severity);
        }

        protected async Task SendNotificationAsync(string notificationName, UserIdentifier user,
            LocalizableString localizableMessage, IDictionary<string, object> localizableMessageData = null,
            NotificationSeverity severity = NotificationSeverity.Info)
        {
            var notificationData = new LocalizableMessageNotificationData(localizableMessage);
            if (localizableMessageData != null)
                foreach (var pair in localizableMessageData)
                    notificationData[pair.Key] = pair.Value;

            await _notificationPublisher.PublishAsync(notificationName, notificationData, severity: severity,
                userIds: new[] {user});
        }

        public async Task SendMassNotificationAsync(string message, UserIdentifier[] userIds = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            Type[] targetNotifiers = null
        )
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.MassNotification,
                new MessageNotificationData(message),
                severity: severity,
                userIds: userIds,
                targetNotifiers: targetNotifiers
            );
        }

        public async Task SendPeriodStatusNotificationAsync(PeriodInfo period)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "PeriodNotificationMessage",
                    KontecgCoreConsts.LocalizationSourceName
                )
            )
            {
                ["key"] = period.ReferenceGroup,
                ["year"] = period.Year,
                ["month"] = L(period.Month.ToString()),
                ["status"] = L(period.Status.ToString()),
            };
            
            await _notificationPublisher.PublishAsync(AppNotificationNames.PeriodNotification, notificationData);
        }
    }
}

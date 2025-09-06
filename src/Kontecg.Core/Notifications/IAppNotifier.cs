using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Authorization.Users;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Timing;

namespace Kontecg.Notifications
{
    public interface IAppNotifier
    {
        Task WelcomeToTheApplicationAsync(User user);

        Task NewCompanyRegisteredAsync(Company company);

        Task SendMessageAsync(UserIdentifier user, string message,
            NotificationSeverity severity = NotificationSeverity.Info);

        Task SendMessageAsync(string notificationName, string message, UserIdentifier[] userIds = null,
            NotificationSeverity severity = NotificationSeverity.Info);

        Task SendMessageAsync(UserIdentifier user, LocalizableString localizableMessage,
            IDictionary<string, object> localizableMessageData = null,
            NotificationSeverity severity = NotificationSeverity.Info);

        Task SendMassNotificationAsync(string message, UserIdentifier[] userIds = null,
            NotificationSeverity severity = NotificationSeverity.Info, Type[] targetNotifiers = null);

        Task SendPeriodStatusNotificationAsync(PeriodInfo period);
    }
}

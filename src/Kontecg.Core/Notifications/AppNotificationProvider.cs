using Kontecg.Authorization;
using Kontecg.Localization;

namespace Kontecg.Notifications
{
    public class AppNotificationProvider : NotificationProvider
    {
        public override void SetNotifications(INotificationDefinitionContext context)
        {
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.WelcomeToTheApplication,
                    displayName: L("WelcomeToTheApplication")
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.NewVersionAvailable,
                    displayName: L("NewVersionAvailable"),
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Root)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.NewCompanyRegistered,
                    displayName: L("NewCompanyRegistered"),
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Companies)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.PeriodNotification,
                    displayName: L("PeriodNotification")
                )
            );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, KontecgCoreConsts.LocalizationSourceName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Auditing;
using Kontecg.Authorization;
using Kontecg.Authorization.Users;
using Kontecg.BackgroundJobs;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration;
using Kontecg.Domain.Repositories;
using Kontecg.Linq.Extensions;
using Kontecg.Notifications.Dto;
using Kontecg.Organizations;
using Kontecg.Runtime.Session;
using Kontecg.UI;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Notifications
{
    [KontecgAuthorize]
    public class NotificationAppService : KontecgAppServiceBase, INotificationAppService
    {
        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly IUserNotificationManager _userNotificationManager;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IUserOrganizationUnitRepository _userOrganizationUnitRepository;
        private readonly INotificationConfiguration _notificationConfiguration;
        private readonly INotificationStore _notificationStore;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<UserNotificationInfo, Guid> _userNotificationRepository;

        public NotificationAppService(
            INotificationDefinitionManager notificationDefinitionManager,
            IUserNotificationManager userNotificationManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IRepository<User, long> userRepository,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IAppNotifier appNotifier,
            IUserOrganizationUnitRepository userOrganizationUnitRepository,
            INotificationConfiguration notificationConfiguration,
            INotificationStore notificationStore,
            IBackgroundJobManager backgroundJobManager,
            IRepository<UserNotificationInfo, Guid> userNotificationRepository)
        {
            _notificationDefinitionManager = notificationDefinitionManager;
            _userNotificationManager = userNotificationManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _userRepository = userRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _appNotifier = appNotifier;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _notificationConfiguration = notificationConfiguration;
            _notificationStore = notificationStore;
            _backgroundJobManager = backgroundJobManager;
            _userNotificationRepository = userNotificationRepository;
        }

        [DisableAuditing]
        public async Task<GetNotificationsOutput> GetUserNotificationsAsync(GetUserNotificationsInput input)
        {
            var totalCount = await _userNotificationManager.GetUserNotificationCountAsync(
                KontecgSession.ToUserIdentifier(), input.State, input.StartDate, input.EndDate
            );

            var unreadCount = await _userNotificationManager.GetUserNotificationCountAsync(
                KontecgSession.ToUserIdentifier(), UserNotificationState.Unread, input.StartDate, input.EndDate
            );
            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
                KontecgSession.ToUserIdentifier(), input.State, input.SkipCount, input.MaxResultCount, input.StartDate,
                input.EndDate
            );

            return new GetNotificationsOutput(totalCount, unreadCount, notifications);
        }

        public async Task<bool> ShouldUserUpdateAppAsync()
        {
            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
                KontecgSession.ToUserIdentifier(), UserNotificationState.Unread
            );

            return notifications.Any(x => x.Notification.NotificationName == AppNotificationNames.NewVersionAvailable);
        }

        public async Task<SetNotificationAsReadOutput> SetAllAvailableVersionNotificationAsReadAsync()
        {
            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
                KontecgSession.ToUserIdentifier(), UserNotificationState.Unread
            );

            var filteredNotifications = notifications
                .Where(x => x.Notification.NotificationName == AppNotificationNames.NewVersionAvailable)
                .ToList();

            if (!filteredNotifications.Any())
            {
                return new SetNotificationAsReadOutput(false);
            }

            foreach (var notification in filteredNotifications)
            {
                if (notification.State == UserNotificationState.Read)
                {
                    continue;
                }

                await _userNotificationManager.UpdateUserNotificationStateAsync(
                    notification.CompanyId,
                    notification.Id,
                    UserNotificationState.Read
                );
            }

            return new SetNotificationAsReadOutput(true);
        }

        public async Task SetAllNotificationsAsReadAsync()
        {
            await _userNotificationManager.UpdateAllUserNotificationStatesAsync(
                KontecgSession.ToUserIdentifier(),
                UserNotificationState.Read
            );
        }

        public async Task<SetNotificationAsReadOutput> SetNotificationAsReadAsync(EntityDto<Guid> input)
        {
            var userNotification =
                await _userNotificationManager.GetUserNotificationAsync(KontecgSession.CompanyId, input.Id);
            if (userNotification == null)
            {
                return new SetNotificationAsReadOutput(false);
            }

            if (userNotification.UserId != KontecgSession.GetUserId())
            {
                throw new Exception(
                    $"Given user notification id ({input.Id}) is not belong to the current user ({KontecgSession.GetUserId()})"
                );
            }

            if (userNotification.State == UserNotificationState.Read)
            {
                return new SetNotificationAsReadOutput(false);
            }

            await _userNotificationManager.UpdateUserNotificationStateAsync(KontecgSession.CompanyId, input.Id,
                UserNotificationState.Read);
            return new SetNotificationAsReadOutput(true);
        }

        public async Task<GetNotificationSettingsOutput> GetNotificationSettingsAsync()
        {
            var output = new GetNotificationSettingsOutput();

            output.ReceiveNotifications =
                await SettingManager.GetSettingValueAsync<bool>(NotificationSettingNames.ReceiveNotifications);

            //Get general notifications, not entity related notifications.
            var notificationDefinitions =
                (await _notificationDefinitionManager.GetAllAvailableAsync(KontecgSession.ToUserIdentifier())).Where(nd =>
                    nd.EntityType == null);

            output.Notifications =
                ObjectMapper.Map<List<NotificationSubscriptionWithDisplayNameDto>>(notificationDefinitions);

            var subscribedNotifications = (await _notificationSubscriptionManager
                    .GetSubscribedNotificationsAsync(KontecgSession.ToUserIdentifier()))
                .Select(ns => ns.NotificationName)
                .ToList();

            output.Notifications.ForEach(n => n.IsSubscribed = subscribedNotifications.Contains(n.Name));

            return output;
        }

        public async Task UpdateNotificationSettingsAsync(UpdateNotificationSettingsInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(KontecgSession.ToUserIdentifier(),
                NotificationSettingNames.ReceiveNotifications, input.ReceiveNotifications.ToString().ToLowerInvariant());

            foreach (var notification in input.Notifications)
            {
                if (notification.IsSubscribed)
                {
                    await _notificationSubscriptionManager.SubscribeAsync(KontecgSession.ToUserIdentifier(),
                        notification.Name);
                }
                else
                {
                    await _notificationSubscriptionManager.UnsubscribeAsync(KontecgSession.ToUserIdentifier(),
                        notification.Name);
                }
            }
        }

        public async Task DeleteNotificationAsync(EntityDto<Guid> input)
        {
            var notification = await _userNotificationManager.GetUserNotificationAsync(KontecgSession.CompanyId, input.Id);
            if (notification == null)
            {
                return;
            }

            if (notification.UserId != KontecgSession.GetUserId())
            {
                throw new UserFriendlyException(L("ThisNotificationDoesntBelongToYou"));
            }

            await _userNotificationManager.DeleteUserNotificationAsync(KontecgSession.CompanyId, input.Id);
        }

        public async Task DeleteAllUserNotificationsAsync(DeleteAllUserNotificationsInput input)
        {
            await _userNotificationManager.DeleteAllUserNotificationsAsync(
                KontecgSession.ToUserIdentifier(),
                input.State,
                input.StartDate,
                input.EndDate);
        }

        [KontecgAuthorize(PermissionNames.AdministrationMassNotification)]
        public async Task<PagedResultDto<MassNotificationUserLookupTableDto>> GetAllUserForLookupTableAsync(
            GetAllForLookupTableInput input)
        {
            var query = _userRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e =>
                        (e.Name != null && e.Name.Contains(input.Filter)) ||
                        (e.Surname != null && e.Surname.Contains(input.Filter)) ||
                        (e.EmailAddress != null && e.EmailAddress.Contains(input.Filter))
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<MassNotificationUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new MassNotificationUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name + " " + user.Surname + " (" + user.EmailAddress + ")"
                });
            }

            return new PagedResultDto<MassNotificationUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [KontecgAuthorize(PermissionNames.AdministrationMassNotification)]
        public async Task<PagedResultDto<MassNotificationOrganizationUnitLookupTableDto>>
            GetAllOrganizationUnitForLookupTableAsync(GetAllForLookupTableInput input)
        {
            var query = _organizationUnitRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => e.DisplayName != null && e.DisplayName.Contains(input.Filter));

            var totalCount = await query.CountAsync();

            var organizationUnitList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<MassNotificationOrganizationUnitLookupTableDto>();
            foreach (var organizationUnit in organizationUnitList)
            {
                lookupTableDtoList.Add(new MassNotificationOrganizationUnitLookupTableDto
                {
                    Id = organizationUnit.Id,
                    DisplayName = organizationUnit.DisplayName
                });
            }

            return new PagedResultDto<MassNotificationOrganizationUnitLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [KontecgAuthorize(PermissionNames.AdministrationMassNotification)]
        public async Task CreateMassNotificationAsync(CreateMassNotificationInput input)
        {
            if (input.TargetNotifiers.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("MassNotificationTargetNotifiersFieldIsRequiredMessage"));
            }

            var userIds = new List<UserIdentifier>();

            if (!input.UserIds.IsNullOrEmpty())
            {
                userIds.AddRange(input.UserIds.Select(i => new UserIdentifier(KontecgSession.CompanyId, i)));
            }

            if (!input.OrganizationUnitIds.IsNullOrEmpty())
            {
                userIds.AddRange(
                    await _userOrganizationUnitRepository.GetAllUsersInOrganizationUnitHierarchicalAsync(
                        input.OrganizationUnitIds)
                );
            }

            if (userIds.Count == 0)
            {
                if (input.OrganizationUnitIds.IsNullOrEmpty())
                {
                    // tried to get users from organization, but could not find any user
                    throw new UserFriendlyException(L("MassNotificationNoUsersFoundInOrganizationUnitMessage"));
                }

                throw new UserFriendlyException(L("MassNotificationUserOrOrganizationUnitFieldIsRequiredMessage"));
            }

            var targetNotifiers = new List<Type>();

            foreach (var notifier in _notificationConfiguration.Notifiers)
            {
                if (input.TargetNotifiers.Contains(notifier.FullName))
                {
                    targetNotifiers.Add(notifier);
                }
            }

            await _appNotifier.SendMassNotificationAsync(
                input.Message,
                userIds.DistinctBy(u => u.UserId).ToArray(),
                input.Severity,
                targetNotifiers.ToArray()
            );
        }

        [KontecgAuthorize(PermissionNames.AdministrationNewVersionCreate)]
        public async Task CreateNewVersionReleasedNotificationAsync()
        {
            var args = new SendNotificationToAllUsersArgs
            {
                NotificationName = AppNotificationNames.NewVersionAvailable,
                Message = L("NewVersionAvailableNotificationMessage")
            };

            await _backgroundJobManager.EnqueueAsync<SendNotificationToAllUsersBackgroundJob, SendNotificationToAllUsersArgs>(args);
        }

        public List<string> GetAllNotifiers()
        {
            return _notificationConfiguration.Notifiers.Select(n => n.FullName).ToList();
        }

        [KontecgAuthorize(PermissionNames.AdministrationMassNotification)]
        public async Task<GetPublishedNotificationsOutput> GetNotificationsPublishedByUserAsync(
            GetPublishedNotificationsInput input)
        {
            return new GetPublishedNotificationsOutput(
                await _notificationStore.GetNotificationsPublishedByUserAsync(KontecgSession.ToUserIdentifier(),
                    AppNotificationNames.MassNotification, input.StartDate, input.EndDate)
            );
        }
    }
}

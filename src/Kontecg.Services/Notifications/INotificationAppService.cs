using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Notifications.Dto;

namespace Kontecg.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task<GetNotificationsOutput> GetUserNotificationsAsync(GetUserNotificationsInput input);

        Task<SetNotificationAsReadOutput> SetAllAvailableVersionNotificationAsReadAsync();

        Task SetAllNotificationsAsReadAsync();

        Task<SetNotificationAsReadOutput> SetNotificationAsReadAsync(EntityDto<Guid> input);

        Task<GetNotificationSettingsOutput> GetNotificationSettingsAsync();

        Task UpdateNotificationSettingsAsync(UpdateNotificationSettingsInput input);

        Task DeleteNotificationAsync(EntityDto<Guid> input);

        Task DeleteAllUserNotificationsAsync(DeleteAllUserNotificationsInput input);

        Task CreateMassNotificationAsync(CreateMassNotificationInput input);

        Task CreateNewVersionReleasedNotificationAsync();

        Task<bool> ShouldUserUpdateAppAsync();

        List<string> GetAllNotifiers();

        Task<GetPublishedNotificationsOutput> GetNotificationsPublishedByUserAsync(GetPublishedNotificationsInput input);

        Task<PagedResultDto<MassNotificationUserLookupTableDto>> GetAllUserForLookupTableAsync(
            GetAllForLookupTableInput input);

        Task<PagedResultDto<MassNotificationOrganizationUnitLookupTableDto>>
            GetAllOrganizationUnitForLookupTableAsync(GetAllForLookupTableInput input);
    }
}

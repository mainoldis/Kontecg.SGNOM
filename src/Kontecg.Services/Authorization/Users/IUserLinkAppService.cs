using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization.Users.Dto;

namespace Kontecg.Authorization.Users
{
    public interface IUserLinkAppService : IApplicationService
    {
        Task LinkToUserAsync(LinkToUserInput linkToUserInput);

        Task<PagedResultDto<LinkedUserDto>> GetLinkedUsersAsync(GetLinkedUsersInput input);

        Task<ListResultDto<LinkedUserDto>> GetRecentlyUsedLinkedUsersAsync();

        Task UnlinkUserAsync(UnlinkUserInput input);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization.Users.Delegation.Dto;

namespace Kontecg.Authorization.Users.Delegation
{
    public interface IUserDelegationAppService : IApplicationService
    {
        Task<PagedResultDto<UserDelegationDto>> GetDelegatedUsersAsync(GetUserDelegationsInput input);

        Task DelegateNewUserAsync(CreateUserDelegationDto input);

        Task RemoveDelegationAsync(EntityDto<long> input);

        Task<List<UserDelegationDto>> GetActiveUserDelegationsAsync();
    }
}

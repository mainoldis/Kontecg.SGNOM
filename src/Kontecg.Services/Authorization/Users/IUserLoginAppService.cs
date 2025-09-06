using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization.Users.Dto;

namespace Kontecg.Authorization.Users
{
    public interface IUserLoginAppService : IApplicationService
    {
        Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts();
    }
}

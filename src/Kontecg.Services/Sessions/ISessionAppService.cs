using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Sessions.Dto;

namespace Kontecg.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationOutput> GetCurrentLoginInformationAsync();

        GetCurrentLoginInformationOutput GetCurrentLoginInformation();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInTokenAsync();
    }
}

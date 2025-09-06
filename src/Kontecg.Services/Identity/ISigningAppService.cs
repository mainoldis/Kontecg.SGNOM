using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Identity.Dto;

namespace Kontecg.Identity
{
    public interface ISigningAppService : IApplicationService
    {
        SignDto GetSignById(int id);

        Task<SignDto> GetSignByIdAsync(int id);
    }
}

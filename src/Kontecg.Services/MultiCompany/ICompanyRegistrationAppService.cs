using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.MultiCompany.Dto;

namespace Kontecg.MultiCompany
{
    public interface ICompanyRegistrationAppService : IApplicationService
    {
        Task<RegisterCompanyOutput> RegisterCompanyAsync(RegisterCompanyInput input);
    }
}

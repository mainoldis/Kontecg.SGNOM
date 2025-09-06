using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Configuration.Companies.Dto;

namespace Kontecg.Configuration.Companies
{
    public interface ICompanySettingsAppService : IApplicationService
    {
        Task<CompanySettingsEditDto> GetAllSettingsAsync();

        Task UpdateAllSettingsAsync(CompanySettingsEditDto input);
    }
}

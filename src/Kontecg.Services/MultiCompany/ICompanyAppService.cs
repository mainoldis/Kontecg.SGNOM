using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.MultiCompany.Dto;

namespace Kontecg.MultiCompany
{
    public interface ICompanyAppService : IApplicationService
    {
        Task<PagedResultDto<CompanyListDto>> GetCompaniesAsync(GetCompaniesInput input);

        Task CreateCompanyAsync(CreateCompanyInput input);

        Task<CompanyEditDto> GetCompanyForEditAsync(EntityDto input);

        Task UpdateCompanyAsync(CompanyEditDto input);

        Task DeleteCompanyAsync(EntityDto input);

        Task<GetCompanyFeaturesEditOutput> GetCompanyFeaturesForEditAsync(EntityDto input);

        Task UpdateCompanyFeaturesAsync(UpdateCompanyFeaturesInput input);

        Task ResetCompanySpecificFeaturesAsync(EntityDto input);

        Task UnlockCompanyAdminAsync(EntityDto input);
    }
}

using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Common.Dto;
using Kontecg.MultiCompany.Dto;

namespace Kontecg.Common
{
    public interface ICommonLookupAppService : IApplicationService
    {
        Task<PagedResultDto<NameValueDto>> FindUsersAsync(FindUsersInput input);

        Task<PagedResultDto<NameValueDto>> FindPersonsAsync(FindPersonsInput input);

        Task<ListResultDto<ComboboxItemDto>> GetCompaniesForComboboxAsync();

        Task<ListResultDto<ComboboxItemDto>> GetStatesForComboboxAsync();

        Task<ListResultDto<ComboboxItemDto>> GetCitiesByStateForComboboxAsync(FindCitiesInput input);

        CompanyInfoDto GetCompanyInfo(int? companyId = null);

        Task<CompanyInfoDto> GetCompanyInfoAsync(int? companyId = null);
    }
}

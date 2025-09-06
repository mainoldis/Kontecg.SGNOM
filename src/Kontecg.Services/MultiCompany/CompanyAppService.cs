using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus;
using Kontecg.Extensions;
using Kontecg.Features.Dto;
using Kontecg.HumanResources;
using Kontecg.Linq.Extensions;
using Kontecg.MultiCompany.Dto;
using Kontecg.Runtime.Security;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.MultiCompany
{
    [KontecgAuthorize(PermissionNames.Companies)]
    public class CompanyAppService : KontecgAppServiceBase, ICompanyAppService
    {
        public CompanyAppService()
        {
            EventBus = NullEventBus.Instance;
        }

        public IEventBus EventBus { get; set; }

        public async Task<PagedResultDto<CompanyListDto>> GetCompaniesAsync(GetCompaniesInput input)
        {
            var query = CompanyManager.Companies
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                    t => t.Name.Contains(input.Filter) || t.CompanyName.Contains(input.Filter))
                .WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
                .WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value);

            var companyCount = await query.CountAsync();
            var companies = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<CompanyListDto>(
                companyCount,
                ObjectMapper.Map<List<CompanyListDto>>(companies)
            );
        }

        [KontecgAuthorize(PermissionNames.CompaniesCreate)]
        [UnitOfWork(IsDisabled = true)]
        public async Task CreateCompanyAsync(CreateCompanyInput input)
        {
            await CompanyManager.CreateWithAdminUserAsync(input.CompanyName,
                input.Name,
                input.Reup,
                input.Organism,
                new Address(input.Street, input.City, input.State, input.Country, input.ZipCode, input.PhoneNumber, input.Fax),
                input.AdminPassword,
                input.AdminEmailAddress,
                input.ConnectionString,
                input.IsActive,
                input.ShouldChangePasswordOnNextLogin
            );
        }

        [KontecgAuthorize(PermissionNames.CompaniesEdit)]
        public async Task<CompanyEditDto> GetCompanyForEditAsync(EntityDto input)
        {
            var companyEditDto = ObjectMapper.Map<CompanyEditDto>(await CompanyManager.GetByIdAsync(input.Id));
            companyEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(companyEditDto.ConnectionString);
            return companyEditDto;
        }

        [KontecgAuthorize(PermissionNames.CompaniesEdit)]
        public async Task UpdateCompanyAsync(CompanyEditDto input)
        {
            input.ConnectionString = SimpleStringCipher.Instance.Encrypt(input.ConnectionString);
            var company = await CompanyManager.GetByIdAsync(input.Id);

            ObjectMapper.Map(input, company);

            await CompanyManager.UpdateAsync(company);
        }

        [KontecgAuthorize(PermissionNames.CompaniesDelete)]
        public async Task DeleteCompanyAsync(EntityDto input)
        {
            var company = await CompanyManager.GetByIdAsync(input.Id);
            await CompanyManager.DeleteAsync(company);
        }

        [KontecgAuthorize(PermissionNames.CompaniesChangeFeatures)]
        public async Task<GetCompanyFeaturesEditOutput> GetCompanyFeaturesForEditAsync(EntityDto input)
        {
            var features = FeatureManager.GetAll()
                .Where(f => f.Scope.HasFlag(FeatureScopes.Company));
            var featureValues = await CompanyManager.GetFeatureValuesAsync(input.Id);

            return new GetCompanyFeaturesEditOutput
            {
                Features = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList(),
                FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
            };
        }

        [KontecgAuthorize(PermissionNames.CompaniesChangeFeatures)]
        public async Task UpdateCompanyFeaturesAsync(UpdateCompanyFeaturesInput input)
        {
            await CompanyManager.SetFeatureValuesAsync(input.Id,
                input.FeatureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
        }

        [KontecgAuthorize(PermissionNames.CompaniesChangeFeatures)]
        public async Task ResetCompanySpecificFeaturesAsync(EntityDto input)
        {
            await CompanyManager.ResetAllFeaturesAsync(input.Id);
        }

        public async Task UnlockCompanyAdminAsync(EntityDto input)
        {
            using (CurrentUnitOfWork.SetCompanyId(input.Id))
            {
                var companyAdmin = await UserManager.GetAdminAsync();
                companyAdmin?.Unlock();
            }
        }
    }
}

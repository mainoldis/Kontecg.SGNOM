using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Common.Dto;
using Kontecg.Extensions;
using Kontecg.HumanResources;
using Kontecg.Linq.Extensions;
using Kontecg.MimeTypes;
using Kontecg.MultiCompany;
using Kontecg.MultiCompany.Dto;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Threading;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Common
{
    [KontecgAuthorize]
    public class CommonLookupAppService : KontecgAppServiceBase, ICommonLookupAppService
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IMimeTypeMap _mimeTypeMap;

        private readonly CompanyManager _companyManager;
        private readonly PersonManager _personManager;
        private readonly IPersonRepository _personRepository;

        public CommonLookupAppService(
            CompanyManager companyManager,
            IPersonRepository personRepository,
            PersonManager personManager,
            IBinaryObjectManager binaryObjectManager,
            ITempFileCacheManager tempFileCacheManager,
            IMimeTypeMap mimeTypeMap)
        {
            _companyManager = companyManager;
            _personRepository = personRepository;
            _personManager = personManager;

            _binaryObjectManager = binaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;
            _mimeTypeMap = mimeTypeMap;
        }

        public async Task<PagedResultDto<NameValueDto>> FindUsersAsync(FindUsersInput input)
        {
            if (KontecgSession.CompanyId != null)
                //Prevent companies to get other company's users.
                input.CompanyId = KontecgSession.CompanyId;

            using (CurrentUnitOfWork.SetCompanyId(input.CompanyId))
            {
                var query = UserManager.Users
                    .WhereIf(
                        !input.Filter.IsNullOrWhiteSpace(),
                        u =>
                            u.Name.Contains(input.Filter) ||
                            u.Surname.Contains(input.Filter) ||
                            u.UserName.Contains(input.Filter) ||
                            u.EmailAddress.Contains(input.Filter)
                    ).WhereIf(input.ExcludeCurrentUser, u => u.Id != KontecgSession.GetUserId());

                var userCount = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.Name)
                    .ThenBy(u => u.Surname)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<NameValueDto>(
                    userCount,
                    users.Select(u =>
                        new NameValueDto(
                            u.FullName + " (" + u.EmailAddress + ")",
                            u.Id.ToString()
                        )
                    ).ToList()
                );
            }
        }

        public async Task<PagedResultDto<NameValueDto>> FindPersonsAsync(FindPersonsInput input)
        {
            using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
            {
                var query = _personRepository.GetAllIncluding()
                    .WhereIf(
                        !input.Filter.IsNullOrWhiteSpace(),
                        p =>
                            p.Name.Contains(input.Filter) ||
                            p.Surname.Contains(input.Filter) ||
                            p.Lastname.Contains(input.Filter) ||
                            p.IdentityCard.Contains(input.Filter)
                    );

                var personsCount = await query.CountAsync();
                var persons = await query
                    .OrderBy(u => u.Name)
                    .ThenBy(u => u.Surname)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<NameValueDto>(
                    personsCount,
                    persons.Select(u =>
                        new NameValueDto(
                            u.FullName + " (" + u.IdentityCard + ")",
                            u.Id.ToString()
                        )
                    ).ToList()
                );
            }
        }

        [KontecgAllowAnonymous]
        public async Task<ListResultDto<ComboboxItemDto>> GetCompaniesForComboboxAsync()
        {
            var companies =
                (await _companyManager.Companies.Where(a => a.IsActive).ToListAsync()).OrderBy(e => e.CompanyName);

            return new ListResultDto<ComboboxItemDto>(
                companies.Select(e => new ComboboxItemDto(e.Id.ToString(), e.CompanyName)).ToList()
            );
        }

        public async Task<ListResultDto<ComboboxItemDto>> GetStatesForComboboxAsync()
        {
            var states = await _personManager.GetStatesByCountryCodeAsync();
            return new ListResultDto<ComboboxItemDto>(
                states.OrderBy(e => e.Code).Select(e => new ComboboxItemDto(e.Code, e.Name){IsSelected = e.Code == "88"}).ToList()
            );
        }

        public async Task<ListResultDto<ComboboxItemDto>> GetCitiesByStateForComboboxAsync(FindCitiesInput input)
        {
            var cities = await _personManager.GetCitiesByStateCodeAsync(input.State);
            return new ListResultDto<ComboboxItemDto>(
                cities.OrderBy(e => e.Code).Select(e => new ComboboxItemDto(e.Code, e.Name)).ToList()
            );
        }

        private Company GetCompany(int companyId)
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                return CompanyManager.GetById(companyId);
            }
        }

        private async Task<Company> GetCompanyAsync(int companyId)
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                return await CompanyManager.GetByIdAsync(companyId).ConfigureAwait(false);
            }
        }

        public CompanyInfoDto GetCompanyInfo(int? companyId = null)
        {
            var currentCompany = companyId != null ? GetCompany(companyId.Value) : GetCurrentCompany();
            var result = ObjectMapper.Map<CompanyInfoDto>(currentCompany);

            if (currentCompany.HasLogo())
            {
                var logoExtension = _mimeTypeMap.GetExtension(currentCompany.LogoFileType);
                var logoBinaryObject =
                    AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(currentCompany.LogoId.Value));

                result.LogoFile = logoBinaryObject != null
                    ? new TempFileInfo(currentCompany.CompanyName + "_logo" + logoExtension, currentCompany.LogoFileType,
                        logoBinaryObject.Bytes)
                    : new TempFileInfo();
                if (logoBinaryObject != null)
                    _tempFileCacheManager.SetFile(currentCompany.LogoId.Value.ToString(), result.LogoFile);
            }

            if (currentCompany.HasLetterHead())
            {
                var letterExtension = _mimeTypeMap.GetExtension(currentCompany.LetterHeadFileType);
                var letterBinaryObject =
                    AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(currentCompany.LetterHeadId.Value));

                result.LetterHeadFile = letterBinaryObject != null
                    ? new TempFileInfo(currentCompany.CompanyName + "_legal" + letterExtension, currentCompany.LetterHeadFileType,
                        letterBinaryObject.Bytes)
                    : new TempFileInfo();
                if (letterBinaryObject != null)
                    _tempFileCacheManager.SetFile(currentCompany.LetterHeadId.Value.ToString(), result.LetterHeadFile);
            }

            return result;
        }

        public async Task<CompanyInfoDto> GetCompanyInfoAsync(int? companyId = null)
        {
            var currentCompany = companyId != null
                ? await GetCompanyAsync(companyId.Value)
                : await GetCurrentCompanyAsync();

            var result = ObjectMapper.Map<CompanyInfoDto>(currentCompany);

            if (currentCompany.HasLogo())
            {
                var logoExtension = _mimeTypeMap.GetExtension(currentCompany.LogoFileType);
                var logoBinaryObject = await _binaryObjectManager.GetOrNullAsync(currentCompany.LogoId.Value).ConfigureAwait(false);

                result.LogoFile = logoBinaryObject != null
                    ? new TempFileInfo(currentCompany.CompanyName + "_logo" + logoExtension, currentCompany.LogoFileType,
                        logoBinaryObject.Bytes)
                    : new TempFileInfo();
                if (logoBinaryObject != null)
                    _tempFileCacheManager.SetFile(currentCompany.LogoId.Value.ToString(), result.LogoFile);
            }

            if (currentCompany.HasLetterHead())
            {
                var letterExtension = _mimeTypeMap.GetExtension(currentCompany.LetterHeadFileType);
                var letterBinaryObject = await _binaryObjectManager.GetOrNullAsync(currentCompany.LetterHeadId.Value).ConfigureAwait(false);

                result.LetterHeadFile = letterBinaryObject != null
                    ? new TempFileInfo(currentCompany.CompanyName + "_legal" + letterExtension, currentCompany.LetterHeadFileType,
                        letterBinaryObject.Bytes)
                    : new TempFileInfo();
                if (letterBinaryObject != null)
                    _tempFileCacheManager.SetFile(currentCompany.LetterHeadId.Value.ToString(), result.LetterHeadFile);
            }
            return result;
        }
    }
}

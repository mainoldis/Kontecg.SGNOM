using Kontecg.Application.Services.Dto;
using Kontecg.Auditing;
using Kontecg.Authorization.Users.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Linq.Dynamic.Core;
using Kontecg.Domain.Repositories;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Session;
using Kontecg.UI;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Authorization.Users
{
    [KontecgAuthorize]
    public class UserLinkAppService : KontecgAppServiceBase, IUserLinkAppService
    {
        private readonly KontecgLoginResultTypeHelper _kontecgLoginResultTypeHelper;
        private readonly IUserLinkManager _userLinkManager;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<UserAccount, long> _userAccountRepository;
        private readonly LogInManager _logInManager;

        public UserLinkAppService(KontecgLoginResultTypeHelper kontecgLoginResultTypeHelper, IUserLinkManager userLinkManager, IRepository<Company> companyRepository, IRepository<UserAccount, long> userAccountRepository, LogInManager logInManager)
        {
            _kontecgLoginResultTypeHelper = kontecgLoginResultTypeHelper;
            _userLinkManager = userLinkManager;
            _companyRepository = companyRepository;
            _userAccountRepository = userAccountRepository;
            _logInManager = logInManager;
        }

        public async Task LinkToUserAsync(LinkToUserInput input)
        {
            var loginResult = await _logInManager.LoginAsync(input.UsernameOrEmailAddress, input.Password, input.CompanyName);

            if (loginResult.Result != KontecgLoginResultType.Success)
            {
                throw _kontecgLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult, input.UsernameOrEmailAddress, input.CompanyName);
            }

            if (KontecgSession.IsUser(loginResult.User))
            {
                throw new UserFriendlyException(L("YouCannotLinkToSameAccount"));
            }

            if (loginResult.User.ShouldChangePasswordOnNextLogin)
            {
                throw new UserFriendlyException(L("ChangePasswordBeforeLinkToAnAccount"));
            }

            var currentUser = await GetCurrentUserAsync();
            await _userLinkManager.LinkAsync(currentUser, loginResult.User);
        }

        public async Task<PagedResultDto<LinkedUserDto>> GetLinkedUsersAsync(GetLinkedUsersInput input)
        {
            var currentUserAccount = await _userLinkManager.GetUserAccountAsync(KontecgSession.ToUserIdentifier());
            if (currentUserAccount == null)
                return new PagedResultDto<LinkedUserDto>(0, new List<LinkedUserDto>());

            var query = CreateLinkedUsersQuery(currentUserAccount, input.Sorting);

            var totalCount = await query.CountAsync();

            var linkedUsers = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return new PagedResultDto<LinkedUserDto>(totalCount, linkedUsers);
        }

        [DisableAuditing]
        public async Task<ListResultDto<LinkedUserDto>> GetRecentlyUsedLinkedUsersAsync()
        {
            var currentUserAccount = await _userLinkManager.GetUserAccountAsync(KontecgSession.ToUserIdentifier());
            if (currentUserAccount == null)
            {
                return new ListResultDto<LinkedUserDto>();
            }

            var query = CreateLinkedUsersQuery(currentUserAccount, "CompanyName, Username");
            var recentlyUsedlinkedUsers = await query.Take(3).ToListAsync();

            return new ListResultDto<LinkedUserDto>(recentlyUsedlinkedUsers);
        }

        public async Task UnlinkUserAsync(UnlinkUserInput input)
        {
            var currentUserAccount = await _userLinkManager.GetUserAccountAsync(KontecgSession.ToUserIdentifier());

            if (!currentUserAccount.UserLinkId.HasValue)
            {
                throw new Exception(L("YouAreNotLinkedToAnyAccount"));
            }

            if (!await _userLinkManager.AreUsersLinkedAsync(KontecgSession.ToUserIdentifier(), input.ToUserIdentifier()))
            {
                return;
            }

            await _userLinkManager.UnlinkAsync(input.ToUserIdentifier());
        }

        private IQueryable<LinkedUserDto> CreateLinkedUsersQuery(UserAccount currentUserAccount, string sorting)
        {
            var currentUserIdentifier = KontecgSession.ToUserIdentifier();

            return (from userAccount in _userAccountRepository.GetAll()
                    join company in _companyRepository.GetAll() on userAccount.CompanyId equals company.Id into companyJoined
                    from company in companyJoined.DefaultIfEmpty()
                    where
                        (userAccount.CompanyId != currentUserIdentifier.CompanyId ||
                        userAccount.UserId != currentUserIdentifier.UserId) &&
                        userAccount.UserLinkId.HasValue &&
                        userAccount.UserLinkId == currentUserAccount.UserLinkId
                    select new LinkedUserDto
                    {
                        Id = userAccount.UserId,
                        CompanyId = userAccount.CompanyId,
                        CompanyName = company == null ? "." : company.CompanyName,
                        Username = userAccount.UserName
                    }).OrderBy(sorting);
        }
    }
}

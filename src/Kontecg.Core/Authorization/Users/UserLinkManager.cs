using Kontecg.Authorization.Impersonation;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Session;
using Kontecg.UI;

namespace Kontecg.Authorization.Users
{
    public class UserLinkManager : KontecgCoreDomainServiceBase, IUserLinkManager
    {
        private readonly IRepository<UserAccount, long> _userAccountRepository;
        private readonly ICacheManager _cacheManager;
        private readonly UserManager _userManager;
        private readonly UserClaimsPrincipalFactory _principalFactory;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public IKontecgSession KontecgSession { get; set; }

        public UserLinkManager(
            IRepository<UserAccount, long> userAccountRepository,
            ICacheManager cacheManager,
            UserManager userManager,
            UserClaimsPrincipalFactory principalFactory,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userAccountRepository = userAccountRepository;
            _cacheManager = cacheManager;
            _userManager = userManager;
            _principalFactory = principalFactory;
            _unitOfWorkManager = unitOfWorkManager;

            KontecgSession = NullKontecgSession.Instance;
        }

        public virtual async Task LinkAsync(User firstUser, User secondUser)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var firstUserAccount = await GetUserAccountAsync(firstUser.ToUserIdentifier());
                var secondUserAccount = await GetUserAccountAsync(secondUser.ToUserIdentifier());

                var userLinkId = firstUserAccount.UserLinkId ?? firstUserAccount.Id;
                firstUserAccount.UserLinkId = userLinkId;

                var userAccountsToLink = secondUserAccount.UserLinkId.HasValue
                    ? await _userAccountRepository.GetAllListAsync(ua => ua.UserLinkId == secondUserAccount.UserLinkId.Value)
                    : [secondUserAccount];

                userAccountsToLink.ForEach(u =>
                {
                    u.UserLinkId = userLinkId;
                });

                await CurrentUnitOfWork.SaveChangesAsync();
            });
        }

        public virtual async Task<bool> AreUsersLinkedAsync(UserIdentifier firstUserIdentifier, UserIdentifier secondUserIdentifier)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var firstUserAccount = await GetUserAccountAsync(firstUserIdentifier);
                var secondUserAccount = await GetUserAccountAsync(secondUserIdentifier);

                if (!firstUserAccount.UserLinkId.HasValue || !secondUserAccount.UserLinkId.HasValue)
                {
                    return false;
                }

                return firstUserAccount.UserLinkId == secondUserAccount.UserLinkId;
            });
        }

        public virtual async Task UnlinkAsync(UserIdentifier userIdentifier)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var targetUserAccount = await GetUserAccountAsync(userIdentifier);
                targetUserAccount.UserLinkId = null;

                await CurrentUnitOfWork.SaveChangesAsync();
            });
        }

        public virtual async Task<UserAccount> GetUserAccountAsync(UserIdentifier userIdentifier)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _userAccountRepository.FirstOrDefaultAsync(
                    ua => ua.CompanyId == userIdentifier.CompanyId && ua.UserId == userIdentifier.UserId
                );
            });
        }

        public async Task<string> GetAccountSwitchTokenAsync(long targetUserId, int? targetCompanyId)
        {
            //Create a cache item
            var cacheItem = new SwitchToLinkedAccountCacheItem(
                targetCompanyId,
                targetUserId,
                KontecgSession.ImpersonatorCompanyId,
                KontecgSession.ImpersonatorUserId
            );

            //Create a random token and save to the cache
            var token = Guid.NewGuid().ToString();

            await _cacheManager
                .GetSwitchToLinkedAccountCache()
                .SetAsync(token, cacheItem, TimeSpan.FromMinutes(1));

            return token;
        }

        public async Task<UserAndIdentity> GetSwitchedUserAndIdentityAsync(string switchAccountToken)
        {
            var cacheItem = await _cacheManager.GetSwitchToLinkedAccountCache().GetOrDefaultAsync(switchAccountToken);
            if (cacheItem == null)
            {
                throw new UserFriendlyException(L("SwitchToLinkedAccountTokenErrorMessage"));
            }

            //Get the user from company
            var user = await _userManager.FindByIdAsync(cacheItem.TargetUserId.ToString());

            //Create identity
            var identity = (ClaimsIdentity)(await _principalFactory.CreateAsync(user)).Identity;

            //Add claims for audit logging
            if (KontecgSession.ImpersonatorCompanyId.HasValue)
            {
                identity.AddClaim(new Claim(KontecgClaimTypes.ImpersonatorCompanyId, cacheItem.ImpersonatorCompanyId.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (cacheItem.ImpersonatorUserId.HasValue)
            {
                identity.AddClaim(new Claim(KontecgClaimTypes.ImpersonatorUserId, cacheItem.ImpersonatorUserId.Value.ToString(CultureInfo.InvariantCulture)));
            }

            //Remove the cache item to prevent re-use
            await _cacheManager.GetSwitchToLinkedAccountCache().RemoveAsync(switchAccountToken);

            return new UserAndIdentity(user, identity);
        }
    }
}

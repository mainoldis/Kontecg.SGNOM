using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Kontecg.Authorization.Users;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Session;
using Kontecg.UI;

namespace Kontecg.Authorization.Impersonation
{
    public class ImpersonationManager : KontecgCoreDomainServiceBase, IImpersonationManager
    {
        public IKontecgSession KontecgSession { get; set; }

        private readonly ICacheManager _cacheManager;
        private readonly UserManager _userManager;
        private readonly UserClaimsPrincipalFactory _principalFactory;

        public ImpersonationManager(
            ICacheManager cacheManager,
            UserManager userManager,
            UserClaimsPrincipalFactory principalFactory)
        {
            _cacheManager = cacheManager;
            _userManager = userManager;
            _principalFactory = principalFactory;

            KontecgSession = NullKontecgSession.Instance;
        }

        public async Task<UserAndIdentity> GetImpersonatedUserAndIdentityAsync(string impersonationToken)
        {
            var cacheItem = await _cacheManager.GetImpersonationCache().GetOrDefaultAsync(impersonationToken);
            if (cacheItem == null)
            {
                throw new UserFriendlyException(L("ImpersonationTokenErrorMessage"));
            }

            CheckCurrentCompany(cacheItem.TargetCompanyId);

            //Get the user from company
            var user = await _userManager.FindByIdAsync(cacheItem.TargetUserId.ToString());

            //Create identity
            var identity = await GetClaimsIdentityFromCacheAsync(user, cacheItem);

            //Remove the cache item to prevent re-use
            await _cacheManager.GetImpersonationCache().RemoveAsync(impersonationToken);

            return new UserAndIdentity(user, identity);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentityFromCacheAsync(User user, ImpersonationCacheItem cacheItem)
        {
            var identity = (ClaimsIdentity) (await _principalFactory.CreateAsync(user)).Identity;

            if (!cacheItem.IsBackToImpersonator)
            {
                //Add claims for audit logging
                if (cacheItem.ImpersonatorCompanyId.HasValue)
                {
                    identity.AddClaim(new Claim(KontecgClaimTypes.ImpersonatorCompanyId,
                        cacheItem.ImpersonatorCompanyId.Value.ToString(CultureInfo.InvariantCulture)));
                }

                identity.AddClaim(new Claim(KontecgClaimTypes.ImpersonatorUserId,
                    cacheItem.ImpersonatorUserId.ToString(CultureInfo.InvariantCulture)));
            }

            return identity;
        }

        public Task<string> GetImpersonationTokenAsync(long userId, int? companyId)
        {
            if (KontecgSession.ImpersonatorUserId.HasValue)
            {
                throw new UserFriendlyException(L("CascadeImpersonationErrorMessage"));
            }

            if (KontecgSession.CompanyId.HasValue)
            {
                if (!companyId.HasValue)
                {
                    throw new UserFriendlyException(L("FromCompanyToHostImpersonationErrorMessage"));
                }

                if (companyId.Value != KontecgSession.CompanyId.Value)
                {
                    throw new UserFriendlyException(L("DifferentCompanyImpersonationErrorMessage"));
                }
            }

            return GenerateImpersonationTokenAsync(companyId, userId, false);
        }

        public Task<string> GetBackToImpersonatorTokenAsync()
        {
            if (!KontecgSession.ImpersonatorUserId.HasValue)
            {
                throw new UserFriendlyException(L("NotImpersonatedLoginErrorMessage"));
            }

            return GenerateImpersonationTokenAsync(KontecgSession.ImpersonatorCompanyId, KontecgSession.ImpersonatorUserId.Value, true);
        }

        private void CheckCurrentCompany(int? companyId)
        {
            if (KontecgSession.CompanyId != companyId)
            {
                throw new KontecgException($"Current company is different than given company. KontecgSession.CompanyId: {KontecgSession.CompanyId}, given companyId: {companyId}");
            }
        }

        private async Task<string> GenerateImpersonationTokenAsync(int? companyId, long userId, bool isBackToImpersonator)
        {
            //Create a cache item
            var cacheItem = new ImpersonationCacheItem(
                companyId,
                userId,
                isBackToImpersonator
            );

            if (!isBackToImpersonator)
            {
                cacheItem.ImpersonatorCompanyId = KontecgSession.CompanyId;
                cacheItem.ImpersonatorUserId = KontecgSession.GetUserId();
            }

            //Create a random token and save to the cache
            var token = Guid.NewGuid().ToString();

            await _cacheManager
                .GetImpersonationCache()
                .SetAsync(token, cacheItem, TimeSpan.FromMinutes(1));

            return token;
        }
    }
}

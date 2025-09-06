using Kontecg.Application.Services;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Session;
using Kontecg.Threading;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using Kontecg.Authorization.Users;
using Kontecg.IdentityFramework;

namespace Kontecg
{
    public class SGNOMAppServiceBase : ApplicationService
    {
        protected SGNOMAppServiceBase()
        {
            LocalizationSourceName = SGNOMConsts.LocalizationSourceName;
        }

        public PersonManager PersonManager { get; set; }

        public CompanyManager CompanyManager { get; set; }

        public UserManager UserManager { get; set; }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(KontecgSession.GetUserId().ToString());
            if (user == null) throw new Exception("There is no current user!");

            return user;
        }

        protected virtual User GetCurrentUser()
        {
            return AsyncHelper.RunSync(GetCurrentUserAsync);
        }

        protected virtual Task<Company> GetCurrentCompanyAsync()
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                return CompanyManager.GetByIdAsync(KontecgSession.GetCompanyId());
            }
        }

        protected virtual Company GetCurrentCompany()
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                return CompanyManager.GetById(KontecgSession.GetCompanyId());
            }
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
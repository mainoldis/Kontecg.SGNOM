using System.Security.Claims;
using System.Threading.Tasks;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Timing;
using Microsoft.AspNetCore.Identity;

namespace Kontecg.Authorization
{
    public class LogInManager : KontecgLogInManager<Company, Role, User>
    {
        private readonly UserClaimsPrincipalFactory _claimsPrincipalFactory;

        public LogInManager(
            UserManager userManager,
            IMultiCompanyConfig multiCompanyConfig,
            IRepository<Company> companyRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher,
            KontecgRoleManager<Role, User> roleManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory)
            : base(
                userManager,
                multiCompanyConfig,
                companyRepository, unitOfWorkManager,
                settingManager,
                userLoginAttemptRepository,
                userManagementConfig,
                iocResolver,
                passwordHasher,
                roleManager,
                claimsPrincipalFactory)
        {
            _claimsPrincipalFactory = claimsPrincipalFactory;
        }

        protected override async Task<KontecgLoginResult<Company, User>> CreateLoginResultAsync(User user, Company company = null)
        {
            if (await IsPersonRequiredForLoginAsync(user.CompanyId)
                && user.PersonId == null
                && user.UserName != KontecgUserBase.AdminUserName) //ignore for admin user
            {
                var kontecgLoginResult = new KontecgLoginResult<Company, User>(KontecgLoginResultType.FailedForOtherReason);
                kontecgLoginResult.SetFailReason(new LocalizableString("Identity.UserNotAssociateWithPerson", KontecgCoreConsts.LocalizationSourceName));
                return kontecgLoginResult;
            }

            if (!user.IsActive)
            {
                return new KontecgLoginResult<Company, User>(KontecgLoginResultType.UserIsNotActive);
            }

            if (await IsEmailConfirmationRequiredForLoginAsync(user.CompanyId) && !user.IsEmailConfirmed)
            {
                return new KontecgLoginResult<Company, User>(KontecgLoginResultType.UserEmailIsNotConfirmed);
            }

            if (await IsPhoneConfirmationRequiredForLoginAsync(user.CompanyId) && !user.IsPhoneNumberConfirmed)
            {
                return new KontecgLoginResult<Company, User>(KontecgLoginResultType.UserPhoneNumberIsNotConfirmed);
            }

            user.LastLoginTime = Clock.Now;

            await UserManager.UpdateAsync(user);

            var principal = await _claimsPrincipalFactory.CreateAsync(user);

            return new KontecgLoginResult<Company, User>(
                company,
                user,
                principal.Identity as ClaimsIdentity
            );
        }

        protected virtual async Task<bool> IsPersonRequiredForLoginAsync(int? companyId)
        {
            if (companyId.HasValue)
                return await SettingManager.GetSettingValueForCompanyAsync<bool>(AppSettings.UserManagement
                    .IsPersonRequiredForLogin, companyId.Value);

            return await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement
                .IsPersonRequiredForLogin);
        }
    }
}

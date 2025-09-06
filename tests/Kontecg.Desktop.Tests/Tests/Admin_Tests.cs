using System.Linq;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Authorization.Accounts;
using Kontecg.Authorization.Accounts.Dto;
using Kontecg.Authorization.Users;
using Kontecg.Authorization.Users.Delegation;
using Kontecg.Authorization.Users.Delegation.Dto;
using Kontecg.Authorization.Users.Dto;
using Kontecg.Runtime.Session;
using Kontecg.Timing;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Admin_Tests : DesktopTestModuleTestBase
    {
        private readonly LogInManager _logInManager;

        public Admin_Tests()
        {
            _logInManager = Resolve<LogInManager>();
        }

        [Fact]
        public async Task Authenticate_new_user_Test()
        {
            var kontecgLoginResult = await _logInManager.LoginAsync("kontecg", "admin", "ECG", shouldLockout: false);
            kontecgLoginResult.Result.ShouldBe(KontecgLoginResultType.Success);

            KontecgSession.UserId = kontecgLoginResult.User?.Id;
            KontecgSession.CompanyId = kontecgLoginResult.Company?.Id;
        }

        [Fact]
        public void Create_new_role_and_user_Test()
        {
            var newUser = CreateUser("test");
            GrantPermission(newUser, PermissionNames.Accounting);
            GrantPermission(newUser, PermissionNames.AccountingExpenseItems);
            GrantPermission(newUser, PermissionNames.AccountingExpenseItemsCreate);
        }

        [Fact]
        public async Task Get_all_permissions_Test()
        {
            var kontecgLoginResult = await _logInManager.LoginAsync("kontecg", "admin", "ECG", shouldLockout: false);

            if (kontecgLoginResult.Result == KontecgLoginResultType.Success)
            {
                using var shouldBeDisposable = KontecgSession.Use(kontecgLoginResult.Company.Id, kontecgLoginResult.User.Id);

                var allPermissions = await PermissionManager.GetAllPermissionsAsync();

                allPermissions.ShouldNotBeEmpty();

                await WithUnitOfWorkAsync(async () =>
                {
                    var user = await UserManager.GetUserAsync(KontecgSession.ToUserIdentifier());
                    
                    var denied =
                        allPermissions.Where(p => p.Name.StartsWith("Kontecg.Administration.OrganizationUnits")).ToArray();

                    foreach (var t in denied) await UserManager.ProhibitPermissionAsync(user, t);
                });
            }
        }

        [Fact]
        public async Task Link_to_other_user_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var linkAppService = Resolve<IUserLinkAppService>();

            await linkAppService.LinkToUserAsync(new LinkToUserInput()
                {CompanyName = "ECG", Password = "admin", UsernameOrEmailAddress = "kontecg"});
        }

        [Fact]
        public async Task Get_linked_users_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var linkAppService = Resolve<IUserLinkAppService>();

            var linkedUsers = await linkAppService.GetLinkedUsersAsync(new GetLinkedUsersInput {MaxResultCount = KontecgCoreConsts.DefaultPageSize});
            linkedUsers.TotalCount.ShouldBe(1);
        }

        [Fact]
        public async Task Delegate_user_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var delegationAppService = Resolve<IUserDelegationAppService>();

            await delegationAppService.DelegateNewUserAsync(new CreateUserDelegationDto()
                {StartTime = Clock.Now, EndTime = Clock.Now.AddDays(30), TargetUserId = 5});
        }

        [Fact]
        public async Task Get_active_delegations_Test()
        {
            using (var shouldBeDisposable = KontecgSession.Use(null, 5))
            {
                var delegationAppService = Resolve<IUserDelegationAppService>();
                var accountAppService = Resolve<IAccountAppService>();

                var activeUserDelegations = await delegationAppService.GetActiveUserDelegationsAsync();
                activeUserDelegations.ShouldNotBeEmpty();

                var impersonateOutput = await accountAppService.DelegatedImpersonateAsync(new DelegatedImpersonateInput() { UserDelegationId = activeUserDelegations[0].Id });

                var user = await UserManager.GetUserOrNullAsync(KontecgSession.ToUserIdentifier());
                var grantedPermissionsAsync = await UserManager.GetGrantedPermissionsAsync(user);
            }
        }
    }
}

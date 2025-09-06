using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Facilities.Logging;
using Kontecg.Authorization;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Castle.Logging.Log4Net;
using Kontecg.EFCore;
using Kontecg.IdentityFramework;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.TestBase;
using Kontecg.Threading;
using Kontecg.Timing;
using NMoneys;
using Shouldly;

namespace Kontecg.Desktop
{
    public abstract class DesktopTestModuleTestBase : KontecgIntegratedTestBase<DesktopTestModule>
    {
        protected readonly RoleManager RoleManager;
        protected readonly UserManager UserManager;
        protected readonly IPermissionManager PermissionManager;
        protected readonly IPermissionChecker PermissionChecker;
        protected readonly UserStore UserStore;

        protected DesktopTestModuleTestBase()
        {
            Clock.Provider = ClockProviders.Local;
            //Fix cultureInfo

            var cultureInfo = CultureHelper.GetCultureInfoByChecking("es");
            // The following line provides localization for the application's user interface.
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            // The following line provides localization for data formats.
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            // Set this culture as the default culture for all threads in this application.
            // Note: The following properties are supported in the .NET Framework 4.5+
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ThreadCultureSanitizer.Sanitize();
            Currency.InitializeAllCurrencies();

            RoleManager = Resolve<RoleManager>();
            UserManager = Resolve<UserManager>();
            PermissionManager = Resolve<IPermissionManager>();
            PermissionChecker = Resolve<IPermissionChecker>();
            UserStore = Resolve<UserStore>();
            LocalIocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseKontecgLog4Net().WithConfig("log4net.config"));
        }

        public void UsingDbContext(Action<KontecgCoreDbContext> action)
        {
            using var context = LocalIocManager.Resolve<KontecgCoreDbContext>();
            action(context);
            context.SaveChanges();
        }

        public T UsingDbContext<T>(Func<KontecgCoreDbContext, T> func)
        {
            using var context = LocalIocManager.Resolve<KontecgCoreDbContext>();
            var result = func(context);
            context.SaveChanges();

            return result;
        }

        public async Task UsingDbContextAsync(Func<KontecgCoreDbContext, Task> action)
        {
            await using var context = LocalIocManager.Resolve<KontecgCoreDbContext>();
            await action(context);
            await context.SaveChangesAsync(true);
        }

        public async Task<T> UsingDbContextAsync<T>(Func<KontecgCoreDbContext, Task<T>> func)
        {
            await using var context = LocalIocManager.Resolve<KontecgCoreDbContext>();
            var result = await func(context);
            await context.SaveChangesAsync();

            return result;
        }

        protected Company GetDefaultCompany()
        {
            return GetCompany(KontecgCompanyBase.DefaultCompanyName);
        }

        protected Company GetCompany(string companyName)
        {
            return UsingDbContext(
                context =>
                {
                    return context.Companies.Single(t => t.CompanyName == companyName);
                });
        }

        protected User GetDefaultCompanyAdmin()
        {
            var defaultCompany = GetDefaultCompany();
            return UsingDbContext(
                context =>
                {
                    return context.Users.Single(u => u.UserName == KontecgUserBase.AdminUserName && u.CompanyId == defaultCompany.Id);
                });
        }

        protected Role CreateRole(string name)
        {
            return CreateRole(name, name);
        }

        protected Role CreateRole(string name, string displayName)
        {
            var role = new Role(null, name, displayName);

            AsyncHelper.RunSync(() => RoleManager.CreateAsync(role)).Succeeded.ShouldBe(true);

            UsingDbContext( context =>
            {
                var createdRole = context.Roles.FirstOrDefault(r => r.Name == name);
                createdRole.ShouldNotBe(null);
            });

            return role;
        }

        protected User CreateUser(string userName)
        {
            var user = new User
            {
                CompanyId = KontecgSession.CompanyId,
                UserName = userName,
                Name = userName,
                Surname = userName,
                EmailAddress = userName + "@ecg.moa.minem.cu",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            };

            user.SetNormalizedNames();

            WithUnitOfWork( () => AsyncHelper.RunSync(() => UserManager.CreateAsync(user)).CheckErrors());

            UsingDbContext(context =>
            {
                var createdUser = context.Users.FirstOrDefault(u => u.UserName == userName);
                createdUser.ShouldNotBe(null);
            });

            return user;
        }

        protected async Task ProhibitPermissionAsync(Role role, string permissionName)
        {
            await RoleManager.ProhibitPermissionAsync(role, PermissionManager.GetPermission(permissionName));
            (await RoleManager.IsGrantedAsync(role.Id, PermissionManager.GetPermission(permissionName))).ShouldBe(false);
        }

        protected async Task GrantPermissionAsync(Role role, string permissionName)
        {
            await RoleManager.GrantPermissionAsync(role, PermissionManager.GetPermission(permissionName));
            (await RoleManager.IsGrantedAsync(role.Id, PermissionManager.GetPermission(permissionName))).ShouldBe(true);
        }

        protected async Task GrantPermissionAsync(User user, string permissionName)
        {
            await UserManager.GrantPermissionAsync(user, PermissionManager.GetPermission(permissionName));
            (await UserManager.IsGrantedAsync(user.Id, permissionName)).ShouldBe(true);
        }

        protected void GrantPermission(User user, string permissionName)
        {
            GrantPermission(user, PermissionManager.GetPermission(permissionName));
            UserManager.IsGranted(user.Id, permissionName).ShouldBe(true);
        }

        /// <summary>
        /// Grants a permission for a user if not already granted.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        protected void GrantPermission(User user, Permission permission)
        {
            UserStore.RemovePermission(user, new PermissionGrantInfo(permission.Name, false));

            if (UserManager.IsGranted(user.Id, permission))
            {
                return;
            }

            UserStore.AddPermission(user, new PermissionGrantInfo(permission.Name, true));
        }

        protected async Task ProhibitPermissionAsync(User user, string permissionName)
        {
            await UserManager.ProhibitPermissionAsync(user, PermissionManager.GetPermission(permissionName));
            (await UserManager.IsGrantedAsync(user.Id, permissionName)).ShouldBe(false);
        }
        protected void ProhibitPermission(User user, Permission permission)
        {
            UserStore.RemovePermission(user, new PermissionGrantInfo(permission.Name, true));

            if (!UserManager.IsGranted(user.Id, permission))
            {
                return;
            }

            UserStore.AddPermission(user, new PermissionGrantInfo(permission.Name, false));
            UserManager.IsGranted(user.Id, permission.Name).ShouldBe(false);
        }
    }
}

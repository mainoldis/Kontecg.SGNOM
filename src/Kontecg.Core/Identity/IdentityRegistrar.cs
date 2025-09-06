using Kontecg.Authorization;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.MultiCompany;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();
            services.AddAuthentication();

            return services.AddKontecgIdentity<Company, User, Role>()
                .AddKontecgCompanyManager<CompanyManager>()
                .AddKontecgUserManager<UserManager>()
                .AddKontecgRoleManager<RoleManager>()
                .AddKontecgUserStore<UserStore>()
                .AddKontecgRoleStore<RoleStore>()
                .AddKontecgUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddPermissionChecker<PermissionChecker>()
                .AddDefaultTokenProviders();
        }
    }
}

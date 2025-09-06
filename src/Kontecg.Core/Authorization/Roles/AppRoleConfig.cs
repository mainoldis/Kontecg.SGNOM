using Kontecg.Baseline.Configuration;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization.Roles
{
    public static class AppRoleConfig
    {
        public static void Configure(IRoleManagementConfig roleManagementConfig)
        {
            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Admin,
                    MultiCompanySides.Host,
                    true
                )
            );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Admin,
                    MultiCompanySides.Company,
                    true
                )
            );

            var publicStaticRole = new StaticRoleDefinition(
                StaticRoleNames.Public,
                MultiCompanySides.Company
            );
            publicStaticRole.GrantedPermissions.Add(PermissionNames.CompanyDashboard);
            roleManagementConfig.StaticRoles.Add(publicStaticRole);
        }
    }
}

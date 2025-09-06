using Kontecg.Application.Features;
using Kontecg.Configuration.Startup;
using Kontecg.Features;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using System.Collections.Generic;

namespace Kontecg.Authorization
{
    public class WinFormsAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiCompanyEnabled;

        /// <inheritdoc />
        public WinFormsAuthorizationProvider(bool isMultiCompanyEnabled)
        {
            _isMultiCompanyEnabled = isMultiCompanyEnabled;
        }

        public WinFormsAuthorizationProvider(IMultiCompanyConfig multiCompanyConfig)
        {
            _isMultiCompanyEnabled = multiCompanyConfig.IsEnabled;
        }

        /// <inheritdoc />
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var companyDashboard = Find(context, PermissionNames.Root, PermissionNames.CompanyDashboard);

            if (companyDashboard != null)
                companyDashboard.CreateChildPermission(WinFormsPermissionNames.CoreCompanyDashboardDesigner, L("Designer"),
                    multiCompanySides: MultiCompanySides.Company, featureDependency: new SimpleFeatureDependency(WinFormsFeatureNames.DashboardDesignerFeature));

            var hostDashboard = Find(context, PermissionNames.Root, PermissionNames.AdministrationHostDashboard);
            if (hostDashboard != null)
                hostDashboard.CreateChildPermission(WinFormsPermissionNames.CoreAdministrationHostDashboardDesigner,
                    L("Designer"),
                    multiCompanySides: MultiCompanySides.Host,
                    featureDependency: new SimpleFeatureDependency(WinFormsFeatureNames.DashboardDesignerFeature));
        }

        private Permission Find(IPermissionDefinitionContext context, string rootPermissionName, string permissionName)
        {
            var permission = context.GetPermissionOrNull(rootPermissionName);
            if (permission == null) return null;

            if (permission.Name == permissionName)
                return permission;

            return Find(permission.Children, permissionName);
        }

        private Permission Find(IReadOnlyList<Permission> permissions, string permissionName)
        {
            if (permissions == null || permissions.Count == 0) return null;

            Permission result = null;

            for (int i = 0; i < permissions.Count; i++)
            {
                if (permissions[i].Name == permissionName)
                {
                    result = permissions[i];
                    break;
                }
                result = Find(permissions[i].Children, permissionName);
            }
            return result;
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, KontecgCoreConsts.LocalizationSourceName);
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kontecg.Runtime.Validation;

namespace Kontecg.Authorization.Permissions
{
    public static class PermissionManagerExtensions
    {
        /// <summary>
        ///     Gets all permissions by names.
        ///     Throws <see cref="KontecgValidationException" /> if can not find any of the permission names.
        /// </summary>
        public static IEnumerable<Permission> GetPermissionsFromNamesByValidating(
            this IPermissionManager permissionManager, IEnumerable<string> permissionNames)
        {
            var permissions = new List<Permission>();
            var undefinedPermissionNames = new List<string>();

            foreach (var permissionName in permissionNames)
            {
                var permission = permissionManager.GetPermissionOrNull(permissionName);
                if (permission == null) undefinedPermissionNames.Add(permissionName);

                permissions.Add(permission);
            }

            if (undefinedPermissionNames.Count > 0)
                throw new KontecgValidationException(
                    $"There are {undefinedPermissionNames.Count} undefined permission names.")
                {
                    ValidationErrors = undefinedPermissionNames.Select(permissionName =>
                        new ValidationResult("Undefined permission: " + permissionName)).ToList()
                };

            return permissions;
        }
    }
}

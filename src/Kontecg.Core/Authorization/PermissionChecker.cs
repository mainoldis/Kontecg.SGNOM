using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;

namespace Kontecg.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}

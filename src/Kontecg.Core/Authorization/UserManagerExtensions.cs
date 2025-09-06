using System.Threading.Tasks;
using Kontecg.Authorization.Users;

namespace Kontecg.Authorization
{
    public static class UserManagerExtensions
    {
        public static async Task<User> GetAdminAsync(this UserManager userManager)
        {
            return await userManager.FindByNameAsync(KontecgUserBase.AdminUserName);
        }
    }
}

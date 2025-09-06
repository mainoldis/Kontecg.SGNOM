using Kontecg.Authorization.Users;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;

namespace Kontecg.Authorization.Roles
{
    public class RoleStore : KontecgRoleStore<Role, User>
    {
        public RoleStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Role> roleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository)
            : base(
                unitOfWorkManager,
                roleRepository,
                rolePermissionSettingRepository)
        {
        }
    }
}

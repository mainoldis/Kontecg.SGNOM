using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization.Roles.Dto;

namespace Kontecg.Authorization.Roles
{
    /// <summary>
    ///     Application service that is used by 'role management'.
    /// </summary>
    public interface IRoleAppService : IApplicationService
    {
        Task<ListResultDto<RoleListDto>> GetRoles(GetRolesInput input);

        Task<GetRoleForEditOutput> GetRoleForEdit(NullableIdDto input);

        Task CreateOrUpdateRole(CreateOrUpdateRoleInput input);

        Task DeleteRole(EntityDto input);
    }
}

using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.HumanResources.Dto;
using Kontecg.Organizations.Dto;

namespace Kontecg.Organizations
{
    public interface IOrganizationUnitAppService : IApplicationService
    {
        Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync();

        Task<PagedResultDto<OrganizationUnitUserListDto>> GetOrganizationUnitUsersAsync(GetOrganizationUnitUsersInput input);

        Task<OrganizationUnitDto> CreateOrganizationUnitAsync(CreateOrganizationUnitInput input);

        OrganizationUnitDto CreateOrganizationUnit(CreateOrganizationUnitInput input);

        Task<OrganizationUnitDto> UpdateOrganizationUnitAsync(UpdateOrganizationUnitInput input);

        OrganizationUnitDto UpdateOrganizationUnit(UpdateOrganizationUnitInput input);

        Task<OrganizationUnitDto> MoveOrganizationUnitAsync(MoveOrganizationUnitInput input);

        OrganizationUnitDto MoveOrganizationUnit(MoveOrganizationUnitInput input);

        Task DeleteOrganizationUnitAsync(EntityDto<long> input);

        void DeleteOrganizationUnit(EntityDto<long> input);

        Task RemoveUserFromOrganizationUnitAsync(UserToOrganizationUnitInput input);

        Task RemoveRoleFromOrganizationUnitAsync(RoleToOrganizationUnitInput input);

        Task RemovePersonFromOrganizationUnitAsync(PersonToOrganizationUnitInput input);

        Task AddUsersToOrganizationUnitAsync(UsersToOrganizationUnitInput input);

        Task AddRolesToOrganizationUnitAsync(RolesToOrganizationUnitInput input);

        Task AddPersonsToOrganizationUnitAsync(PersonsToOrganizationUnitInput input);

        Task<PagedResultDto<NameValueDto>> FindUsersAsync(FindOrganizationUnitUsersInput input);

        Task<PagedResultDto<NameValueDto>> FindRolesAsync(FindOrganizationUnitRolesInput input);

        Task<ListResultDto<PersonDto>> FindPersonsByOrganizationUnitAsync(FindOrganizationUnitPersonsInput input);

        Task<PagedResultDto<NameValueDto>> FindPersonsAsync(FindOrganizationUnitPersonsInput input);

        Task<PagedResultDto<OrganizationUnitRoleListDto>> GetOrganizationUnitRoles(
            GetOrganizationUnitRolesInput input);
    }
}

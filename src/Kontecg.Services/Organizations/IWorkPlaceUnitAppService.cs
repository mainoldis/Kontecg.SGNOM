using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Organizations.Dto;
using System.Threading.Tasks;

namespace Kontecg.Organizations
{
    public interface IWorkPlaceUnitAppService : IApplicationService
    {
        WorkPlaceUnitDto GetByCode(string input);

        Task<WorkPlaceUnitDto> GetByCodeAsync(string input);

        ListResultDto<WorkPlaceUnitDto> GetWorkPlaceUnits();

        Task<ListResultDto<WorkPlaceUnitDto>> GetWorkPlaceUnitsAsync();

        Task<WorkPlaceUnitDto> CreateWorkPlaceUnitAsync(CreateWorkPlaceUnitInput input);

        WorkPlaceUnitDto CreateWorkPlaceUnit(CreateWorkPlaceUnitInput input);

        Task<WorkPlaceUnitDto> UpdateWorkPlaceUnitAsync(UpdateWorkPlaceUnitInput input);

        WorkPlaceUnitDto UpdateWorkPlaceUnit(UpdateWorkPlaceUnitInput input);

        Task<WorkPlaceUnitDto> MoveWorkPlaceUnitAsync(MoveOrganizationUnitInput input);

        WorkPlaceUnitDto MoveWorkPlaceUnit(MoveOrganizationUnitInput input);

        Task DeleteWorkPlaceUnitAsync(EntityDto<long> input);

        void DeleteWorkPlaceUnit(EntityDto<long> input);

        Task<PagedResultDto<OrganizationUnitRoleListDto>> GetWorkPlaceUnitRoles(
            GetOrganizationUnitRolesInput input);
    }
}
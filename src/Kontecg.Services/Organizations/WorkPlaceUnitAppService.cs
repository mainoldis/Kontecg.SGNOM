using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Domain.Repositories;
using Kontecg.EntityHistory;
using Kontecg.HumanResources;
using Kontecg.Linq.Extensions;
using Kontecg.Organizations.Dto;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Organizations
{
    [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnits)]
    [UseCase(Description = "Estructura Organizativa")]
    public class WorkPlaceUnitAppService : KontecgAppServiceBase, IWorkPlaceUnitAppService
    {
        private readonly WorkPlaceUnitManager _workPlaceUnitManager;
        private readonly IRepository<WorkPlaceUnit, long> _workPlaceUnitRepository;
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;
        private readonly RoleManager _roleManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<PersonOrganizationUnit, long> _personOrganizationUnitRepository;
        private readonly IRepository<WorkPlaceUnitCenterCost, long> _workPlaceUnitCenterCostRepository;

        public WorkPlaceUnitAppService(
            WorkPlaceUnitManager workPlaceUnitManager,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            RoleManager roleManager,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository,
            IRepository<WorkPlaceUnit, long> workPlaceUnitRepository,
            IRepository<PersonOrganizationUnit, long> personOrganizationUnitRepository,
            IRepository<WorkPlaceUnitCenterCost, long> workPlaceUnitCenterCostRepository)
        {
            _workPlaceUnitManager = workPlaceUnitManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _roleManager = roleManager;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _workPlaceUnitRepository = workPlaceUnitRepository;
            _personOrganizationUnitRepository = personOrganizationUnitRepository;
            _workPlaceUnitCenterCostRepository = workPlaceUnitCenterCostRepository;
        }

        /// <inheritdoc />
        public WorkPlaceUnitDto GetByCode(string input)
        {
            return CreateWorkPlaceUnitDto(_workPlaceUnitManager.GetByCode(input));
        }

        /// <inheritdoc />
        public async Task<WorkPlaceUnitDto> GetByCodeAsync(string input)
        {
            return await CreateWorkPlaceUnitDtoAsync(await _workPlaceUnitManager.GetByCodeAsync(input));
        }

        /// <inheritdoc />
        public ListResultDto<WorkPlaceUnitDto> GetWorkPlaceUnits()
        {
            var workPlaceUnits = _workPlaceUnitRepository
                                          .GetAllIncluding(
                                              w => w.Classification,
                                              w => w.WorkPlacePayment)
                                          .OrderBy(o => o.Order)
                                          .ToList();

            var workPlaceUnitPersonCounts = _personOrganizationUnitRepository.GetAll()
                                                                              .GroupBy(x => x.OrganizationUnitId)
                                                                              .Select(groupedPersons => new
                                                                              {
                                                                                  organizationUnitId = groupedPersons.Key,
                                                                                  count = groupedPersons.Count()
                                                                              }).ToDictionary(x => x.organizationUnitId, y => y.count);

            var workPlaceUnitCenterCostCounts = _workPlaceUnitCenterCostRepository.GetAll()
                                                                       .GroupBy(x => x.OrganizationUnitId)
                                                                       .Select(groupedCenterCosts => new
                                                                       {
                                                                           organizationUnitId = groupedCenterCosts.Key,
                                                                           count = groupedCenterCosts.Count()
                                                                       }).ToDictionary(x => x.organizationUnitId, y => y.count);

            return new ListResultDto<WorkPlaceUnitDto>(
                workPlaceUnits.Select(ou =>
                {
                    var organizationUnitDto = ObjectMapper.Map<WorkPlaceUnitDto>(ou);
                    organizationUnitDto.PersonCount = workPlaceUnitPersonCounts.ContainsKey(ou.Id)
                        ? workPlaceUnitPersonCounts[ou.Id]
                        : 0;
                    organizationUnitDto.CenterCostCount = workPlaceUnitCenterCostCounts.ContainsKey(ou.Id)
                        ? workPlaceUnitCenterCostCounts[ou.Id]
                        : 0;
                    return organizationUnitDto;
                }).ToList());
        }

        public async Task<ListResultDto<WorkPlaceUnitDto>> GetWorkPlaceUnitsAsync()
        {
            var organizationUnits = await _workPlaceUnitRepository
                .GetAllIncluding(
                    w => w.Classification,
                    w => w.WorkPlacePayment)
                .OrderBy(o => o.Order)
                .ToListAsync();

            var workPlaceUnitPersonCounts = await _personOrganizationUnitRepository.GetAll()
                .GroupBy(x => x.OrganizationUnitId)
                .Select(groupedPersons => new
                {
                    organizationUnitId = groupedPersons.Key,
                    count = groupedPersons.Count()
                }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

            var workPlaceUnitCenterCostCounts = await _workPlaceUnitCenterCostRepository.GetAll()
                .GroupBy(x => x.OrganizationUnitId)
                .Select(groupedCenterCosts => new
                {
                    organizationUnitId = groupedCenterCosts.Key,
                    count = groupedCenterCosts.Count()
                }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

            return new ListResultDto<WorkPlaceUnitDto>(
                organizationUnits.Select(ou =>
                {
                    var organizationUnitDto = ObjectMapper.Map<WorkPlaceUnitDto>(ou);
                    organizationUnitDto.PersonCount = workPlaceUnitPersonCounts.ContainsKey(ou.Id)
                        ? workPlaceUnitPersonCounts[ou.Id]
                        : 0;
                    organizationUnitDto.CenterCostCount = workPlaceUnitCenterCostCounts.ContainsKey(ou.Id)
                        ? workPlaceUnitCenterCostCounts[ou.Id]
                        : 0;
                    return organizationUnitDto;
                }).ToList());
        }

        [UseCase(Description = "Creación de un nuevo elemento en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree)]
        public async Task<WorkPlaceUnitDto> CreateWorkPlaceUnitAsync(CreateWorkPlaceUnitInput input)
        {
            var organizationUnit = new WorkPlaceUnit(KontecgSession.CompanyId, input.DisplayName, maxMembersApproved: input.MaxMembersApproved, parentId: input.ParentId)
            {
                ClassificationId = input.ClassificationId,
                WorkPlacePaymentId = input.WorkPlacePaymentId
            };
            await _workPlaceUnitManager.CreateAsync(organizationUnit);

            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<WorkPlaceUnitDto>(organizationUnit);
        }

        [UseCase(Description = "Creación de un nuevo elemento en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree)]
        public WorkPlaceUnitDto CreateWorkPlaceUnit(CreateWorkPlaceUnitInput input)
        {
            var organizationUnit = new WorkPlaceUnit(KontecgSession.CompanyId, input.DisplayName, maxMembersApproved: input.MaxMembersApproved, parentId: input.ParentId)
            {
                ClassificationId = input.ClassificationId,
                WorkPlacePaymentId = input.WorkPlacePaymentId
            };

            _workPlaceUnitManager.Create(organizationUnit);

            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<WorkPlaceUnitDto>(organizationUnit);
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree)]
        public async Task<WorkPlaceUnitDto> UpdateWorkPlaceUnitAsync(UpdateWorkPlaceUnitInput input)
        {
            var organizationUnit = await _workPlaceUnitRepository.GetAsync(input.Id);

            organizationUnit.DisplayName = input.DisplayName;
            organizationUnit.Acronym = input.Acronym;
            organizationUnit.ClassificationId = input.ClassificationId;
            organizationUnit.WorkPlacePaymentId = input.WorkPlacePaymentId;
            organizationUnit.MaxMembersApproved = input.MaxMembersApproved;

            await _workPlaceUnitManager.UpdateAsync(organizationUnit);

            return await CreateWorkPlaceUnitDtoAsync(organizationUnit);
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree)]
        public WorkPlaceUnitDto UpdateWorkPlaceUnit(UpdateWorkPlaceUnitInput input)
        {
            var organizationUnit = _workPlaceUnitRepository.Get(input.Id);

            organizationUnit.DisplayName = input.DisplayName;
            organizationUnit.Acronym = input.Acronym;
            organizationUnit.ClassificationId = input.ClassificationId;
            organizationUnit.WorkPlacePaymentId = input.WorkPlacePaymentId;
            organizationUnit.MaxMembersApproved = input.MaxMembersApproved;

            _workPlaceUnitManager.Update(organizationUnit);

            return CreateWorkPlaceUnitDto(organizationUnit);
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree)]
        public async Task<WorkPlaceUnitDto> MoveWorkPlaceUnitAsync(MoveOrganizationUnitInput input)
        {
            await _workPlaceUnitManager.MoveAsync(input.Id, input.NewParentId);
            return await CreateWorkPlaceUnitDtoAsync(await _workPlaceUnitRepository.GetAsync(input.Id));
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree)]
        public WorkPlaceUnitDto MoveWorkPlaceUnit(MoveOrganizationUnitInput input)
        {
            _workPlaceUnitManager.Move(input.Id, input.NewParentId);

            return CreateWorkPlaceUnitDto(_workPlaceUnitRepository.Get(input.Id));
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree)]
        public async Task DeleteWorkPlaceUnitAsync(EntityDto<long> input)
        {
            await _userOrganizationUnitRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
            await _personOrganizationUnitRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
            await _organizationUnitRoleRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
            await _workPlaceUnitCenterCostRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
            await _workPlaceUnitManager.DeleteAsync(input.Id);
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree)]
        public void DeleteWorkPlaceUnit(EntityDto<long> input)
        {
            _userOrganizationUnitRepository.Delete(x => x.OrganizationUnitId == input.Id);
            _personOrganizationUnitRepository.Delete(x => x.OrganizationUnitId == input.Id);
            _organizationUnitRoleRepository.Delete(x => x.OrganizationUnitId == input.Id);
            _workPlaceUnitCenterCostRepository.Delete(x => x.OrganizationUnitId == input.Id);
            _workPlaceUnitManager.Delete(input.Id);
        }

        public async Task<PagedResultDto<OrganizationUnitRoleListDto>> GetWorkPlaceUnitRoles(
            GetOrganizationUnitRolesInput input)
        {
            var query = from ouRole in _organizationUnitRoleRepository.GetAll()
                        join ou in _workPlaceUnitRepository.GetAll() on ouRole.OrganizationUnitId equals ou.Id
                        join role in _roleManager.Roles on ouRole.RoleId equals role.Id
                        where ouRole.OrganizationUnitId == input.Id
                        select new
                        {
                            ouRole,
                            role
                        };

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<OrganizationUnitRoleListDto>(
                totalCount,
                items.Select(item =>
                {
                    var organizationUnitRoleDto = ObjectMapper.Map<OrganizationUnitRoleListDto>(item.role);
                    organizationUnitRoleDto.AddedTime = item.ouRole.CreationTime;
                    return organizationUnitRoleDto;
                }).ToList());
        }

        private async Task<WorkPlaceUnitDto> CreateWorkPlaceUnitDtoAsync(WorkPlaceUnit organizationUnit)
        {
            var dto = ObjectMapper.Map<WorkPlaceUnitDto>(organizationUnit);
            dto.MemberCount =
                await _userOrganizationUnitRepository.CountAsync(uou => uou.OrganizationUnitId == organizationUnit.Id);
            dto.PersonCount =
                await _personOrganizationUnitRepository.CountAsync(uou => uou.OrganizationUnitId == organizationUnit.Id);
            dto.CenterCostCount =
                await _workPlaceUnitCenterCostRepository.CountAsync(uou => uou.OrganizationUnitId == organizationUnit.Id);
            return dto;
        }

        private WorkPlaceUnitDto CreateWorkPlaceUnitDto(WorkPlaceUnit organizationUnit)
        {
            var dto = ObjectMapper.Map<WorkPlaceUnitDto>(organizationUnit);
            dto.MemberCount =
                _userOrganizationUnitRepository.Count(uou => uou.OrganizationUnitId == organizationUnit.Id);
            dto.PersonCount =
                _personOrganizationUnitRepository.Count(uou => uou.OrganizationUnitId == organizationUnit.Id);
            dto.CenterCostCount =
                _workPlaceUnitCenterCostRepository.Count(uou => uou.OrganizationUnitId == organizationUnit.Id);
            return dto;
        }
    }
}
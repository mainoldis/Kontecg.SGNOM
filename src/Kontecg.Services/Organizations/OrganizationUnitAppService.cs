using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Domain.Repositories;
using Kontecg.EntityHistory;
using Kontecg.Extensions;
using Kontecg.HumanResources;
using Kontecg.HumanResources.Dto;
using Kontecg.Linq.Extensions;
using Kontecg.Organizations.Dto;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Organizations
{
    [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnits)]
    [UseCase(Description = "Estructura Organizativa")]
    public class OrganizationUnitAppService : KontecgAppServiceBase, IOrganizationUnitAppService
    {
        private readonly OrganizationUnitManager _organizationUnitManager;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;
        private readonly RoleManager _roleManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<PersonOrganizationUnit, long> _personOrganizationUnitRepository;

        public OrganizationUnitAppService(
            OrganizationUnitManager organizationUnitManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            RoleManager roleManager,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository,
            IRepository<PersonOrganizationUnit, long> personOrganizationUnitRepository)
        {
            _organizationUnitManager = organizationUnitManager;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _roleManager = roleManager;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _personOrganizationUnitRepository = personOrganizationUnitRepository;
        }

        public async Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync()
        {
            var organizationUnits = await _organizationUnitRepository.GetAll().OrderBy(o => o.Order).ToListAsync();

            var organizationUnitMemberCounts = await _userOrganizationUnitRepository.GetAll()
                .GroupBy(x => x.OrganizationUnitId)
                .Select(groupedUsers => new
                {
                    organizationUnitId = groupedUsers.Key,
                    count = groupedUsers.Count()
                }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

            var organizationUnitPersonCounts = await _personOrganizationUnitRepository.GetAll()
                .GroupBy(x => x.OrganizationUnitId)
                .Select(groupedPersons => new
                {
                    organizationUnitId = groupedPersons.Key,
                    count = groupedPersons.Count()
                }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

            var organizationUnitRoleCounts = await _organizationUnitRoleRepository.GetAll()
                .GroupBy(x => x.OrganizationUnitId)
                .Select(groupedRoles => new
                {
                    organizationUnitId = groupedRoles.Key,
                    count = groupedRoles.Count()
                }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

            return new ListResultDto<OrganizationUnitDto>(
                organizationUnits.Select(ou =>
                {
                    var organizationUnitDto = ObjectMapper.Map<OrganizationUnitDto>(ou);
                    organizationUnitDto.MemberCount = organizationUnitMemberCounts.ContainsKey(ou.Id)
                        ? organizationUnitMemberCounts[ou.Id]
                        : 0;
                    organizationUnitDto.PersonCount = organizationUnitPersonCounts.ContainsKey(ou.Id)
                        ? organizationUnitPersonCounts[ou.Id]
                        : 0;
                    organizationUnitDto.RoleCount = organizationUnitRoleCounts.ContainsKey(ou.Id)
                        ? organizationUnitRoleCounts[ou.Id]
                        : 0;
                    return organizationUnitDto;
                }).ToList());
        }

        public async Task<PagedResultDto<OrganizationUnitUserListDto>> GetOrganizationUnitUsersAsync(
            GetOrganizationUnitUsersInput input)
        {
            var query = from ouUser in _userOrganizationUnitRepository.GetAll()
                join ou in _organizationUnitRepository.GetAll() on ouUser.OrganizationUnitId equals ou.Id
                join user in UserManager.Users on ouUser.UserId equals user.Id
                where ouUser.OrganizationUnitId == input.Id
                select new
                {
                    ouUser,
                    user
                };

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<OrganizationUnitUserListDto>(
                totalCount,
                items.Select(item =>
                {
                    var organizationUnitUserDto = ObjectMapper.Map<OrganizationUnitUserListDto>(item.user);
                    organizationUnitUserDto.AddedTime = item.ouUser.CreationTime;
                    return organizationUnitUserDto;
                }).ToList());
        }

        [UseCase(Description = "Creación de un nuevo elemento en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree)]
        public async Task<OrganizationUnitDto> CreateOrganizationUnitAsync(CreateOrganizationUnitInput input)
        {
            var organizationUnit =
                new OrganizationUnit(KontecgSession.CompanyId, input.DisplayName, input.ParentId);
            await _organizationUnitManager.CreateAsync(organizationUnit);

            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
        }

        [UseCase(Description = "Creación de un nuevo elemento en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree)]
        public OrganizationUnitDto CreateOrganizationUnit(CreateOrganizationUnitInput input)
        {
            var organizationUnit =
                new OrganizationUnit(KontecgSession.CompanyId, input.DisplayName, input.ParentId);

            _organizationUnitManager.Create(organizationUnit);
            CurrentUnitOfWork.SaveChanges();

            return ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree)]
        public async Task<OrganizationUnitDto> UpdateOrganizationUnitAsync(UpdateOrganizationUnitInput input)
        {
            var organizationUnit = await _organizationUnitRepository.GetAsync(input.Id);

            organizationUnit.DisplayName = input.DisplayName;

            await _organizationUnitManager.UpdateAsync(organizationUnit);

            return await CreateOrganizationUnitDtoAsync(organizationUnit);
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree)]
        public OrganizationUnitDto UpdateOrganizationUnit(UpdateOrganizationUnitInput input)
        {
            var organizationUnit = _organizationUnitRepository.Get(input.Id);

            organizationUnit.DisplayName = input.DisplayName;

            _organizationUnitManager.Update(organizationUnit);

            return CreateOrganizationUnitDto(organizationUnit);
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree)]
        public async Task<OrganizationUnitDto> MoveOrganizationUnitAsync(MoveOrganizationUnitInput input)
        {
            await _organizationUnitManager.MoveAsync(input.Id, input.NewParentId);
            return await CreateOrganizationUnitDtoAsync(await _organizationUnitRepository.GetAsync(input.Id));
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree)]
        public OrganizationUnitDto MoveOrganizationUnit(MoveOrganizationUnitInput input)
        {
            _organizationUnitManager.Move(input.Id, input.NewParentId);

            return CreateOrganizationUnitDto(_organizationUnitRepository.Get(input.Id));
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree)]
        public async Task DeleteOrganizationUnitAsync(EntityDto<long> input)
        {
            await _userOrganizationUnitRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
            await _personOrganizationUnitRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
            await _organizationUnitRoleRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
            await _organizationUnitManager.DeleteAsync(input.Id);
        }

        [UseCase(Description = "Cambio en la estructura")]
        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree)]
        public void DeleteOrganizationUnit(EntityDto<long> input)
        {
            _userOrganizationUnitRepository.Delete(x => x.OrganizationUnitId == input.Id);
            _personOrganizationUnitRepository.Delete(x => x.OrganizationUnitId == input.Id);
            _organizationUnitRoleRepository.Delete(x => x.OrganizationUnitId == input.Id);
            _organizationUnitManager.Delete(input.Id);
        }

        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageMembers)]
        public async Task RemoveUserFromOrganizationUnitAsync(UserToOrganizationUnitInput input)
        {
            await UserManager.RemoveFromOrganizationUnitAsync(input.UserId, input.OrganizationUnitId);
        }

        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageRoles)]
        public async Task RemoveRoleFromOrganizationUnitAsync(RoleToOrganizationUnitInput input)
        {
            await _roleManager.RemoveFromOrganizationUnitAsync(input.RoleId, input.OrganizationUnitId);
        }

        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageMembers)]
        public async Task RemovePersonFromOrganizationUnitAsync(PersonToOrganizationUnitInput input)
        {
            //await PersonManager.RemoveFromOrganizationUnitAsync(input.PersonId, input.OrganizationUnitId);
        }

        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageMembers)]
        public async Task AddUsersToOrganizationUnitAsync(UsersToOrganizationUnitInput input)
        {
            foreach (var userId in input.UserIds)
                await UserManager.AddToOrganizationUnitAsync(userId, input.OrganizationUnitId);
        }

        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageRoles)]
        public async Task AddRolesToOrganizationUnitAsync(RolesToOrganizationUnitInput input)
        {
            foreach (var roleId in input.RoleIds)
                await _roleManager.AddToOrganizationUnitAsync(roleId, input.OrganizationUnitId,
                    KontecgSession.CompanyId);
        }

        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageMembers)]
        public async Task AddPersonsToOrganizationUnitAsync(PersonsToOrganizationUnitInput input)
        {
            //foreach (var personId in input.PersonIds)
            //    await PersonManager.AddToOrganizationUnitAsync(personId, input.OrganizationUnitId);
        }

        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageMembers)]
        public async Task<PagedResultDto<NameValueDto>> FindUsersAsync(FindOrganizationUnitUsersInput input)
        {
            var userIdsInOrganizationUnit = _userOrganizationUnitRepository.GetAll()
                .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
                .Select(uou => uou.UserId);

            var query = UserManager.Users
                .Where(u => !userIdsInOrganizationUnit.Contains(u.Id))
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            var userCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.Name)
                .ThenBy(u => u.Surname)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<NameValueDto>(
                userCount,
                users.Select(u =>
                    new NameValueDto(
                        u.FullName + " (" + u.EmailAddress + ")",
                        u.Id.ToString()
                    )
                ).ToList()
            );
        }

        [KontecgAuthorize(PermissionNames.AdministrationOrganizationUnitsManageRoles)]
        public async Task<PagedResultDto<NameValueDto>> FindRolesAsync(FindOrganizationUnitRolesInput input)
        {
            var roleIdsInOrganizationUnit = _organizationUnitRoleRepository.GetAll()
                .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
                .Select(uou => uou.RoleId);

            var query = _roleManager.Roles
                .Where(u => !roleIdsInOrganizationUnit.Contains(u.Id))
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.DisplayName.Contains(input.Filter) ||
                        u.Name.Contains(input.Filter)
                );

            var roleCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.DisplayName)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<NameValueDto>(
                roleCount,
                users.Select(u =>
                    new NameValueDto(
                        u.DisplayName,
                        u.Id.ToString()
                    )
                ).ToList()
            );
        }

        public async Task<ListResultDto<PersonDto>> FindPersonsByOrganizationUnitAsync(FindOrganizationUnitPersonsInput input)
        {
            var personIdsInOrganizationUnit = _personOrganizationUnitRepository.GetAll()
                .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
                .Select(uou => uou.PersonId);

            var query = PersonManager.Persons
                .Where(u => personIdsInOrganizationUnit.Contains(u.Id));

            var persons = await query
                .OrderBy(u => u.Name)
                .ThenBy(u => u.Surname)
                .ThenBy(u => u.Lastname)
                .ToListAsync();

            return new ListResultDto<PersonDto>(persons.Select(p => ObjectMapper.Map<PersonDto>(p)).ToList());
        }

        public async Task<PagedResultDto<NameValueDto>> FindPersonsAsync(FindOrganizationUnitPersonsInput input)
        {
            var personIdsInOrganizationUnit = _personOrganizationUnitRepository.GetAll()
                .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
                .Select(uou => uou.PersonId);

            var query = PersonManager.Persons
                .Where(u => !personIdsInOrganizationUnit.Contains(u.Id))
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.Lastname.Contains(input.Filter) ||
                        u.IdentityCard.Contains(input.Filter)
                );

            var personCount = await query.CountAsync();
            var persons = await query
                .OrderBy(u => u.Name)
                .ThenBy(u => u.Surname)
                .ThenBy(u => u.Lastname)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<NameValueDto>(
                personCount,
                persons.Select(u =>
                    new NameValueDto(
                        u.FullName + " (" + u.IdentityCard + ")",
                        u.Id.ToString()
                    )
                ).ToList()
            );
        }

        public async Task<PagedResultDto<OrganizationUnitRoleListDto>> GetOrganizationUnitRoles(
            GetOrganizationUnitRolesInput input)
        {
            var query = from ouRole in _organizationUnitRoleRepository.GetAll()
                join ou in _organizationUnitRepository.GetAll() on ouRole.OrganizationUnitId equals ou.Id
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

        private async Task<OrganizationUnitDto> CreateOrganizationUnitDtoAsync(OrganizationUnit organizationUnit)
        {
            var dto = ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
            dto.MemberCount =
                await _userOrganizationUnitRepository.CountAsync(uou => uou.OrganizationUnitId == organizationUnit.Id);
            return dto;
        }

        private OrganizationUnitDto CreateOrganizationUnitDto(OrganizationUnit organizationUnit)
        {
            var dto = ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
            dto.MemberCount =
                _userOrganizationUnitRepository.Count(uou => uou.OrganizationUnitId == organizationUnit.Id);
            return dto;
        }
    }
}

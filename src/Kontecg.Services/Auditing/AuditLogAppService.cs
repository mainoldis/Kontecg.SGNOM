using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Auditing.Dto;
using Kontecg.Auditing.Exporting;
using Kontecg.Authorization;
using Kontecg.Authorization.Users;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Repositories;
using Kontecg.Dto;
using Kontecg.EntityHistory;
using Kontecg.Extensions;
using Kontecg.Linq.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Auditing
{
    [DisableAuditing]
    [KontecgAuthorize(PermissionNames.AdministrationAuditLogs)]
    public class AuditLogAppService : KontecgAppServiceBase, IAuditLogAppService
    {
        private readonly IAuditLogListExcelExporter _auditLogListExcelExporter;
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly IRepository<EntityChange, long> _entityChangeRepository;
        private readonly IRepository<EntityChangeSet, long> _entityChangeSetRepository;
        private readonly IRepository<EntityPropertyChange, long> _entityPropertyChangeRepository;
        private readonly IKontecgStartupConfiguration _kontecgStartupConfiguration;
        private readonly INamespaceStripper _namespaceStripper;
        private readonly IRepository<User, long> _userRepository;

        public AuditLogAppService(
            IRepository<AuditLog, long> auditLogRepository,
            IRepository<User, long> userRepository,
            IAuditLogListExcelExporter auditLogListExcelExporter,
            INamespaceStripper namespaceStripper,
            IRepository<EntityChange, long> entityChangeRepository,
            IRepository<EntityChangeSet, long> entityChangeSetRepository,
            IRepository<EntityPropertyChange, long> entityPropertyChangeRepository,
            IKontecgStartupConfiguration kontecgStartupConfiguration)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
            _auditLogListExcelExporter = auditLogListExcelExporter;
            _namespaceStripper = namespaceStripper;
            _entityChangeRepository = entityChangeRepository;
            _entityChangeSetRepository = entityChangeSetRepository;
            _entityPropertyChangeRepository = entityPropertyChangeRepository;
            _kontecgStartupConfiguration = kontecgStartupConfiguration;
        }

        #region audit logs

        public async Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input)
        {
            var query = CreateAuditLogAndUsersQuery(input);

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var auditLogListDtos = ConvertToAuditLogListDtos(results);

            return new PagedResultDto<AuditLogListDto>(resultCount, auditLogListDtos);
        }

        public async Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input)
        {
            var auditLogs = await CreateAuditLogAndUsersQuery(input)
                .AsNoTracking()
                .OrderByDescending(al => al.AuditLog.ExecutionTime)
                .ToListAsync();

            var auditLogListDtos = ConvertToAuditLogListDtos(auditLogs);

            return _auditLogListExcelExporter.ExportToFile(auditLogListDtos);
        }

        private List<AuditLogListDto> ConvertToAuditLogListDtos(List<AuditLogAndUser> results)
        {
            return results.Select(
                result =>
                {
                    var auditLogListDto = ObjectMapper.Map<AuditLogListDto>(result.AuditLog);
                    auditLogListDto.UserName = result.User?.UserName;
                    auditLogListDto.ServiceName = _namespaceStripper.StripNameSpace(auditLogListDto.ServiceName);
                    return auditLogListDto;
                }).ToList();
        }

        private IQueryable<AuditLogAndUser> CreateAuditLogAndUsersQuery(GetAuditLogsInput input)
        {
            var query = from auditLog in _auditLogRepository.GetAll()
                join user in _userRepository.GetAll() on auditLog.UserId equals user.Id into userJoin
                from joinedUser in userJoin.DefaultIfEmpty()
                where auditLog.ExecutionTime >= input.StartDate && auditLog.ExecutionTime <= input.EndDate
                select new AuditLogAndUser {AuditLog = auditLog, User = joinedUser};

            query = query
                .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                .WhereIf(!input.ServiceName.IsNullOrWhiteSpace(),
                    item => item.AuditLog.ServiceName.Contains(input.ServiceName))
                .WhereIf(!input.MethodName.IsNullOrWhiteSpace(),
                    item => item.AuditLog.MethodName.Contains(input.MethodName))
                .WhereIf(!input.ClientInfo.IsNullOrWhiteSpace(),
                    item => item.AuditLog.ClientInfo.Contains(input.ClientInfo))
                .WhereIf(input.MinExecutionDuration is > 0,
                    item => item.AuditLog.ExecutionDuration >= input.MinExecutionDuration.Value)
                .WhereIf(input.ExecutionDuration is < int.MaxValue,
                    item => item.AuditLog.ExecutionDuration <= input.ExecutionDuration.Value)
                .WhereIf(input.HasException == true,
                    item => item.AuditLog.Exception != null && item.AuditLog.Exception != "")
                .WhereIf(input.HasException == false,
                    item => item.AuditLog.Exception == null || item.AuditLog.Exception == "");
            return query;
        }

        #endregion

        #region entity changes

        public List<NameValueDto> GetEntityHistoryObjectTypes()
        {
            var entityHistoryObjectTypes = new List<NameValueDto>();
            var enabledEntities = (_kontecgStartupConfiguration.GetCustomConfig()
                .FirstOrDefault(x => x.Key == EntityHistoryCoreHelper.EntityHistoryConfigurationName)
                .Value as EntityHistoryUiSetting)?.EnabledEntities ?? new List<string>();

            enabledEntities = KontecgSession.CompanyId == null
                ? EntityHistoryCoreHelper.HostSideTrackedTypes.Select(t => t.FullName).Intersect(enabledEntities)
                    .ToList()
                : EntityHistoryCoreHelper.CompanySideTrackedTypes.Select(t => t.FullName).Intersect(enabledEntities)
                    .ToList();

            foreach (var enabledEntity in enabledEntities)
                entityHistoryObjectTypes.Add(new NameValueDto(L(enabledEntity), enabledEntity));

            return entityHistoryObjectTypes;
        }

        public async Task<PagedResultDto<EntityChangeListDto>> GetEntityChanges(GetEntityChangeInput input)
        {
            var query = CreateEntityChangesAndUsersQuery(input);

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

            return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
        }

        public async Task<PagedResultDto<EntityChangeListDto>> GetEntityTypeChanges(GetEntityTypeChangeInput input)
        {
            // Fix for: https://github.com/aspnetzero/aspnet-zero-core/issues/2101
            var entityId = "\"" + input.EntityId + "\"";

            var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange
                    .EntityChangeSetId
                join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                where entityChange.EntityTypeFullName == input.EntityTypeFullName &&
                      (entityChange.EntityId == input.EntityId || entityChange.EntityId == entityId)
                select new EntityChangeAndUser
                {
                    EntityChange = entityChange,
                    User = user
                };

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

            return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
        }

        public async Task<FileDto> GetEntityChangesToExcel(GetEntityChangeInput input)
        {
            var entityChanges = await CreateEntityChangesAndUsersQuery(input)
                .AsNoTracking()
                .OrderByDescending(ec => ec.EntityChange.EntityChangeSetId)
                .ThenByDescending(ec => ec.EntityChange.ChangeTime)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(entityChanges);

            return _auditLogListExcelExporter.ExportToFile(entityChangeListDtos);
        }

        public async Task<List<EntityPropertyChangeDto>> GetEntityPropertyChanges(long entityChangeId)
        {
            var entityPropertyChanges = (await _entityPropertyChangeRepository.GetAllListAsync())
                .Where(epc => epc.EntityChangeId == entityChangeId);

            return ObjectMapper.Map<List<EntityPropertyChangeDto>>(entityPropertyChanges);
        }

        private List<EntityChangeListDto> ConvertToEntityChangeListDtos(List<EntityChangeAndUser> results)
        {
            return results.Select(
                result =>
                {
                    var entityChangeListDto = ObjectMapper.Map<EntityChangeListDto>(result.EntityChange);
                    entityChangeListDto.UserName = result.User?.UserName;
                    return entityChangeListDto;
                }).ToList();
        }

        private IQueryable<EntityChangeAndUser> CreateEntityChangesAndUsersQuery(GetEntityChangeInput input)
        {
            var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange
                    .EntityChangeSetId
                join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                where entityChange.ChangeTime >= input.StartDate && entityChange.ChangeTime <= input.EndDate
                select new EntityChangeAndUser
                {
                    EntityChange = entityChange,
                    User = user
                };

            query = query
                .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                .WhereIf(!input.EntityTypeFullName.IsNullOrWhiteSpace(),
                    item => item.EntityChange.EntityTypeFullName.Contains(input.EntityTypeFullName));

            return query;
        }

        #endregion
    }
}

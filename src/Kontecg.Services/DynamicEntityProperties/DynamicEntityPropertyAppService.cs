using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.DynamicEntityProperties.Dto;

namespace Kontecg.DynamicEntityProperties
{
    [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityProperties)]
    public class DynamicEntityPropertyAppService : KontecgAppServiceBase, IDynamicEntityPropertyAppService
    {
        private readonly IDynamicEntityPropertyManager _dynamicEntityPropertyManager;

        public DynamicEntityPropertyAppService(IDynamicEntityPropertyManager dynamicEntityPropertyManager)
        {
            _dynamicEntityPropertyManager = dynamicEntityPropertyManager;
        }

        public async Task<DynamicEntityPropertyDto> GetAsync(int id)
        {
            var entity = await _dynamicEntityPropertyManager.GetAsync(id);
            return ObjectMapper.Map<DynamicEntityPropertyDto>(entity);
        }

        public async Task<ListResultDto<DynamicEntityPropertyDto>> GetAllPropertiesOfAnEntityAsync(DynamicEntityPropertyGetAllInput input)
        {
            var entities = await _dynamicEntityPropertyManager.GetAllAsync(input.EntityFullName);
            return new ListResultDto<DynamicEntityPropertyDto>(
                ObjectMapper.Map<List<DynamicEntityPropertyDto>>(entities)
            );
        }

        public async Task<ListResultDto<DynamicEntityPropertyDto>> GetAllAsync()
        {
            var entities = await _dynamicEntityPropertyManager.GetAllAsync();
            return new ListResultDto<DynamicEntityPropertyDto>(
                ObjectMapper.Map<List<DynamicEntityPropertyDto>>(entities)
            );
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertiesCreate)]
        public async Task AddAsync(DynamicEntityPropertyDto dto)
        {
            dto.CompanyId = KontecgSession.CompanyId;
            await _dynamicEntityPropertyManager.AddAsync(ObjectMapper.Map<DynamicEntityProperty>(dto));
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertiesEdit)]
        public async Task UpdateAsync(DynamicEntityPropertyDto dto)
        {
            await _dynamicEntityPropertyManager.UpdateAsync(ObjectMapper.Map<DynamicEntityProperty>(dto));
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertiesDelete)]
        public async Task DeleteAsync(int id)
        {
            await _dynamicEntityPropertyManager.DeleteAsync(id);
        }

        public async Task<ListResultDto<GetAllEntitiesHasDynamicPropertyOutput>> GetAllEntitiesHasDynamicPropertyAsync()
        {
            var entities = await _dynamicEntityPropertyManager.GetAllAsync();
            return new ListResultDto<GetAllEntitiesHasDynamicPropertyOutput>(
                entities?.Select(x => new GetAllEntitiesHasDynamicPropertyOutput()
                {
                    EntityFullName = x.EntityFullName
                }).DistinctBy(x => x.EntityFullName).ToList()
            );
        }
    }
}

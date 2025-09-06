using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.DynamicEntityProperties.Dto;

namespace Kontecg.DynamicEntityProperties
{
    [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValue)]
    public class DynamicPropertyValueAppService : KontecgAppServiceBase, IDynamicPropertyValueAppService
    {
        private readonly IDynamicPropertyValueManager _dynamicPropertyValueManager;
        private readonly IDynamicPropertyValueStore _dynamicPropertyValueStore;

        public DynamicPropertyValueAppService(
            IDynamicPropertyValueManager dynamicPropertyValueManager,
            IDynamicPropertyValueStore dynamicPropertyValueStore
        )
        {
            _dynamicPropertyValueManager = dynamicPropertyValueManager;
            _dynamicPropertyValueStore = dynamicPropertyValueStore;
        }

        public async Task<DynamicPropertyValueDto> GetAsync(int id)
        {
            var entity = await _dynamicPropertyValueManager.GetAsync(id);
            return ObjectMapper.Map<DynamicPropertyValueDto>(entity);
        }

        public async Task<ListResultDto<DynamicPropertyValueDto>> GetAllValuesOfDynamicPropertyAsync(EntityDto input)
        {
            var entities = await _dynamicPropertyValueStore.GetAllValuesOfDynamicPropertyAsync(input.Id);
            return new ListResultDto<DynamicPropertyValueDto>(
                ObjectMapper.Map<List<DynamicPropertyValueDto>>(entities)
            );
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueCreate)]
        public async Task AddAsync(DynamicPropertyValueDto dto)
        {
            dto.CompanyId = KontecgSession.CompanyId;
            await _dynamicPropertyValueManager.AddAsync(ObjectMapper.Map<DynamicPropertyValue>(dto));
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueEdit)]
        public async Task UpdateAsync(DynamicPropertyValueDto dto)
        {
            dto.CompanyId = KontecgSession.CompanyId;
            await _dynamicPropertyValueManager.UpdateAsync(ObjectMapper.Map<DynamicPropertyValue>(dto));
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueDelete)]
        public async Task DeleteAsync(int id)
        {
            await _dynamicPropertyValueManager.DeleteAsync(id);
        }
    }
}

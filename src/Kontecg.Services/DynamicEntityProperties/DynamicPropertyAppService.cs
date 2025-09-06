using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.DynamicEntityProperties.Dto;
using Kontecg.UI.Inputs;

namespace Kontecg.DynamicEntityProperties
{
    [KontecgAuthorize(PermissionNames.AdministrationDynamicProperties)]
    public class DynamicPropertyAppService : KontecgAppServiceBase, IDynamicPropertyAppService
    {
        private readonly IDynamicPropertyManager _dynamicPropertyManager;
        private readonly IDynamicPropertyStore _dynamicPropertyStore;
        private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;

        public DynamicPropertyAppService(
            IDynamicPropertyManager dynamicPropertyManager,
            IDynamicPropertyStore dynamicPropertyStore,
            IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager)
        {
            _dynamicPropertyManager = dynamicPropertyManager;
            _dynamicPropertyStore = dynamicPropertyStore;
            _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;
        }

        public async Task<DynamicPropertyDto> GetAsync(int id)
        {
            var entity = await _dynamicPropertyManager.GetAsync(id);
            return ObjectMapper.Map<DynamicPropertyDto>(entity);
        }

        public async Task<ListResultDto<DynamicPropertyDto>> GetAllAsync()
        {
            var entities = await _dynamicPropertyStore.GetAllAsync();

            return new ListResultDto<DynamicPropertyDto>(
                ObjectMapper.Map<List<DynamicPropertyDto>>(entities)
            );
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicPropertiesCreate)]
        public async Task AddAsync(DynamicPropertyDto dto)
        {
            dto.CompanyId = KontecgSession.CompanyId;
            await _dynamicPropertyManager.AddAsync(ObjectMapper.Map<DynamicProperty>(dto));
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicPropertiesEdit)]
        public async Task UpdateAsync(DynamicPropertyDto dto)
        {
            dto.CompanyId = KontecgSession.CompanyId;
            await _dynamicPropertyManager.UpdateAsync(ObjectMapper.Map<DynamicProperty>(dto));
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicPropertiesDelete)]
        public async Task DeleteAsync(int id)
        {
            await _dynamicPropertyManager.DeleteAsync(id);
        }

        public IInputType FindAllowedInputType(string name)
        {
            return _dynamicEntityPropertyDefinitionManager.GetOrNullAllowedInputType(name);
        }
    }
}

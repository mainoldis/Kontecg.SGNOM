using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Entities;
using Kontecg.DynamicEntityProperties.Dto;
using Kontecg.DynamicEntityPropertyValues.Dto;

namespace Kontecg.DynamicEntityProperties
{
    [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValue)]
    public class DynamicEntityPropertyValueAppService : KontecgAppServiceBase, IDynamicEntityPropertyValueAppService
    {
        private readonly IDynamicEntityPropertyValueManager _dynamicEntityPropertyValueManager;
        private readonly IDynamicPropertyValueManager _dynamicPropertyValueManager;
        private readonly IDynamicEntityPropertyManager _dynamicEntityPropertyManager;
        private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;

        public DynamicEntityPropertyValueAppService(
            IDynamicEntityPropertyValueManager dynamicEntityPropertyValueManager,
            IDynamicPropertyValueManager dynamicPropertyValueManager,
            IDynamicEntityPropertyManager dynamicEntityPropertyManager,
            IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager)
        {
            _dynamicEntityPropertyValueManager = dynamicEntityPropertyValueManager;
            _dynamicPropertyValueManager = dynamicPropertyValueManager;
            _dynamicEntityPropertyManager = dynamicEntityPropertyManager;
            _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;
        }

        public async Task<DynamicEntityPropertyValueDto> GetAsync(int id)
        {
            var entity = await _dynamicEntityPropertyValueManager.GetAsync(id);
            return ObjectMapper.Map<DynamicEntityPropertyValueDto>(entity);
        }

        public async Task<ListResultDto<DynamicEntityPropertyValueDto>> GetAllAsync(GetAllInput input)
        {
            var entities = await _dynamicEntityPropertyValueManager.GetValuesAsync(input.PropertyId, input.EntityId);
            return new ListResultDto<DynamicEntityPropertyValueDto>(
                ObjectMapper.Map<List<DynamicEntityPropertyValueDto>>(entities)
            );
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueCreate)]
        public async Task AddAsync(DynamicEntityPropertyValueDto input)
        {
            var entity = ObjectMapper.Map<DynamicEntityPropertyValue>(input);
            entity.CompanyId = KontecgSession.CompanyId;
            await _dynamicEntityPropertyValueManager.AddAsync(entity);
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueEdit)]
        public async Task UpdateAsync(DynamicEntityPropertyValueDto input)
        {
            var entity = await _dynamicEntityPropertyValueManager.GetAsync(input.Id);
            if (entity == null || entity.CompanyId != KontecgSession.CompanyId)
            {
                throw new EntityNotFoundException(typeof(DynamicEntityPropertyValue), input.Id);
            }

            entity.Value = input.Value;
            entity.DynamicEntityPropertyId = input.DynamicEntityPropertyId;
            entity.EntityId = input.EntityId;

            await _dynamicEntityPropertyValueManager.UpdateAsync(entity);
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueDelete)]
        public async Task DeleteAsync(int id)
        {
            await _dynamicEntityPropertyValueManager.DeleteAsync(id);
        }

        public async Task<GetAllDynamicEntityPropertyValuesOutput> GetAllDynamicEntityPropertyValuesAsync(GetAllDynamicEntityPropertyValuesInput input)
        {
            var localCacheOfDynamicPropertyValues = new Dictionary<int, List<string>>();

            async Task<List<string>> LocalGetAllValuesOfDynamicProperty(int dynamicPropertyId)
            {
                if (!localCacheOfDynamicPropertyValues.ContainsKey(dynamicPropertyId))
                {
                    localCacheOfDynamicPropertyValues[dynamicPropertyId] = (await _dynamicPropertyValueManager
                            .GetAllValuesOfDynamicPropertyAsync(dynamicPropertyId))
                        .Select(x => x.Value).ToList();
                }

                return localCacheOfDynamicPropertyValues[dynamicPropertyId];
            }

            var output = new GetAllDynamicEntityPropertyValuesOutput();
            var dynamicEntityProperties = await _dynamicEntityPropertyManager.GetAllAsync(input.EntityFullName);

            var dynamicEntityPropertySelectedValues = (await _dynamicEntityPropertyValueManager.GetValuesAsync(input.EntityFullName, input.EntityId))
                .GroupBy(value => value.DynamicEntityPropertyId)
                .ToDictionary(
                    group => group.Key,
                    items => items.ToList().Select(value => value.Value)
                        .ToList()
                );

            foreach (var dynamicEntityProperty in dynamicEntityProperties)
            {
                var outputItem = new GetAllDynamicEntityPropertyValuesOutputItem
                {
                    DynamicEntityPropertyId = dynamicEntityProperty.Id,
                    InputType = _dynamicEntityPropertyDefinitionManager.GetOrNullAllowedInputType(dynamicEntityProperty.DynamicProperty.InputType),
                    PropertyName = dynamicEntityProperty.DynamicProperty.PropertyName,
                    AllValuesInputTypeHas = await LocalGetAllValuesOfDynamicProperty(dynamicEntityProperty.DynamicProperty.Id),
                    SelectedValues = dynamicEntityPropertySelectedValues.ContainsKey(dynamicEntityProperty.Id)
                        ? dynamicEntityPropertySelectedValues[dynamicEntityProperty.Id]
                        : new List<string>()
                };

                output.Items.Add(outputItem);
            }

            return output;
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueCreate)]
        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueEdit)]
        public async Task InsertOrUpdateAllValuesAsync(InsertOrUpdateAllValuesInput input)
        {
            if (input.Items.IsNullOrEmpty())
            {
                return;
            }

            foreach (var item in input.Items)
            {
                await _dynamicEntityPropertyValueManager.CleanValuesAsync(item.DynamicEntityPropertyId, item.EntityId);

                foreach (var newValue in item.Values)
                {
                    await _dynamicEntityPropertyValueManager.AddAsync(new DynamicEntityPropertyValue
                    {
                        DynamicEntityPropertyId = item.DynamicEntityPropertyId,
                        EntityId = item.EntityId,
                        Value = newValue,
                        CompanyId = KontecgSession.CompanyId
                    });
                }
            }
        }

        [KontecgAuthorize(PermissionNames.AdministrationDynamicEntityPropertyValueDelete)]
        public async Task CleanValuesAsync(CleanValuesInput input)
        {
            await _dynamicEntityPropertyValueManager.CleanValuesAsync(input.DynamicEntityPropertyId, input.EntityId);
        }
    }
}

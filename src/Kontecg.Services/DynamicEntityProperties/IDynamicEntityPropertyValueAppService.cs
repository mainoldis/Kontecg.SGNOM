using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.DynamicEntityProperties.Dto;
using Kontecg.DynamicEntityPropertyValues.Dto;

namespace Kontecg.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyValueAppService
    {
        Task<DynamicEntityPropertyValueDto> GetAsync(int id);

        Task<ListResultDto<DynamicEntityPropertyValueDto>> GetAllAsync(GetAllInput input);

        Task AddAsync(DynamicEntityPropertyValueDto input);

        Task UpdateAsync(DynamicEntityPropertyValueDto input);

        Task DeleteAsync(int id);

        Task<GetAllDynamicEntityPropertyValuesOutput> GetAllDynamicEntityPropertyValuesAsync(GetAllDynamicEntityPropertyValuesInput input);
    }
}

using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.DynamicEntityProperties.Dto;

namespace Kontecg.DynamicEntityProperties
{
    public interface IDynamicPropertyValueAppService
    {
        Task<DynamicPropertyValueDto> GetAsync(int id);

        Task<ListResultDto<DynamicPropertyValueDto>> GetAllValuesOfDynamicPropertyAsync(EntityDto input);

        Task AddAsync(DynamicPropertyValueDto dto);

        Task UpdateAsync(DynamicPropertyValueDto dto);

        Task DeleteAsync(int id);
    }
}

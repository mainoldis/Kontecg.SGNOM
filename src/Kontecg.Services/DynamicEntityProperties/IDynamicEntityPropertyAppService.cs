using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.DynamicEntityProperties.Dto;

namespace Kontecg.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyAppService
    {
        Task<DynamicEntityPropertyDto> GetAsync(int id);

        Task<ListResultDto<DynamicEntityPropertyDto>> GetAllPropertiesOfAnEntityAsync(DynamicEntityPropertyGetAllInput input);

        Task<ListResultDto<DynamicEntityPropertyDto>> GetAllAsync();

        Task AddAsync(DynamicEntityPropertyDto dto);

        Task UpdateAsync(DynamicEntityPropertyDto dto);

        Task DeleteAsync(int id);

        Task<ListResultDto<GetAllEntitiesHasDynamicPropertyOutput>> GetAllEntitiesHasDynamicPropertyAsync();
    }
}

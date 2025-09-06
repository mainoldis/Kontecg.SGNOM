using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.DynamicEntityProperties.Dto;
using Kontecg.UI.Inputs;

namespace Kontecg.DynamicEntityProperties
{
    public interface IDynamicPropertyAppService
    {
        Task<DynamicPropertyDto> GetAsync(int id);

        Task<ListResultDto<DynamicPropertyDto>> GetAllAsync();

        Task AddAsync(DynamicPropertyDto dto);

        Task UpdateAsync(DynamicPropertyDto dto);

        Task DeleteAsync(int id);

        IInputType FindAllowedInputType(string name);
    }
}

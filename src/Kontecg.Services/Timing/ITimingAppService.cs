using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Timing.Dto;

namespace Kontecg.Timing
{
    public interface ITimingAppService : IApplicationService
    {
        Task<ListResultDto<NameValueDto>> GetTimezones(GetTimezonesInput input);

        Task<List<ComboboxItemDto>> GetTimezoneComboboxItems(GetTimezoneComboboxItemsInput input);
    }
}

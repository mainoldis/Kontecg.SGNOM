using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Calendar.Dto;
using Kontecg.Dependency;

namespace Kontecg.Calendar
{
    public interface IWorkScheduleAppService : IApplicationService, ITransientDependency
    {
        Task<IList<WorkScheduleDto>> GetAllSchedulesAsync();

        IList<WorkScheduleDto> GetAllSchedules();
    }
}

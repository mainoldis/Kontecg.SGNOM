using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Calendar.Dto;
using Kontecg.Timing;

namespace Kontecg.Calendar
{
    [KontecgAuthorize(SGNOMPermissions.WorkSchedule)]
    public class WorkScheduleAppService : SGNOMAppServiceBase, IWorkScheduleAppService
    {
        private readonly IWorkShiftRepository _workShiftRepository;

        public WorkScheduleAppService(IWorkShiftRepository workShiftRepository)
        {
            _workShiftRepository = workShiftRepository;
        }

        [KontecgAuthorize(SGNOMPermissions.WorkScheduleList)]
        public async Task<IList<WorkScheduleDto>> GetAllSchedulesAsync()
        {
            throw new System.NotImplementedException();
        }

        [KontecgAuthorize(SGNOMPermissions.WorkScheduleList)]
        public IList<WorkScheduleDto> GetAllSchedules()
        {
            throw new System.NotImplementedException();
        }
    }
}

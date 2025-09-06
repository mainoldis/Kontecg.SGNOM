using Kontecg.Application.Services.Dto;

namespace Kontecg.Calendar.Dto
{
    public class WorkScheduleDto : EntityDto
    {
        public int CompanyId { get; set; }

        public WorkShiftDto WorkShift { get; set; }

        public string RegimenWorkRest { get; set; }
    }
}

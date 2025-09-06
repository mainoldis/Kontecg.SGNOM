using Kontecg.Application.Services.Dto;
using Kontecg.Timing;

namespace Kontecg.Calendar.Dto
{
    public class RTDDto : EntityDto
    {
        public WorkRegimeType Type { get; set; }

        public string LegalName { get; set; }

        public string DaysScheduling { get; set; }

        public string TimeScheduling { get; set; }
    }
}

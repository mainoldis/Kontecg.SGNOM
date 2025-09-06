using System;
using Itenso.TimePeriod;
using Kontecg.Application.Services.Dto;
using Kontecg.Runtime.Validation;

namespace Kontecg.Calendar.Dto
{
    public class WorkShiftDto : EntityDto, IShouldNormalize
    {
        public string DisplayName { get; set; }

        public DateTime StartDate { get; set; }

        public RTDDto Regime { get; set; }

        public string HoursWorking { get; set; }

        public string RestingTimesPerShift { get; set; }

        public decimal AverageHoursPerShift { get; set; }

        public string Legal { get; set; }

        public void Normalize()
        {
            StartDate = TimeTrim.Hour(StartDate);
        }
    }
}

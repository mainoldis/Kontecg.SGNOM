using Kontecg.Application.Services.Dto;
using System;

namespace Kontecg.Timing.Dto
{
    public class PeriodInfoDto : EntityDto
    {
        public int Year { get; set; }

        public string Month { get; set; }

        public string Quarter { get; set; }

        public DateTime Since { get; set; }

        public DateTime Until { get; set; }

        public string Status { get; set; }

        public int CompanyId { get; set; }

        public string ReferenceGroup { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
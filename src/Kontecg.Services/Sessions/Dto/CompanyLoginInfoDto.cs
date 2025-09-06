using System;
using System.Collections.Generic;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Sessions.Dto
{
    public class CompanyLoginInfoDto : EntityDto
    {
        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string Organism { get; set; }

        public string Address { get; set; }

        public string Reup { get; set; }

        public Guid? LogoId { get; set; }

        public string LogoFileType { get; set; }

        public List<NameValueDto> FeatureValues { get; set; }

        public DateTime CreationTime { get; set; }

        public string CreationTimeString { get; set; }

        public CompanyLoginInfoDto()
        {
            FeatureValues = new List<NameValueDto>();
        }
    }
}

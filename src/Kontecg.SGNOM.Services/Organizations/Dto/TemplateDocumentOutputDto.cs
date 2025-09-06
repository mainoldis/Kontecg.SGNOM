using System;
using Kontecg.Application.Services.Dto;
using Kontecg.Identity.Dto;
using Kontecg.MultiCompany.Dto;

namespace Kontecg.Organizations.Dto
{
    public class TemplateDocumentOutputDto : EntityDto
    {
        public string ContractString { get; set; }

        public CompanyInfoDto Company { get; set; }

        public DateTime MadedOn { get; set; }

        public SignDto MadeBy { get; set; }

        public SignDto ApprovedBy { get; set; }

        public string Review { get; set; }

        public ListResultDto<TemplateListDto> Template { get; set; }

        public ListResultDto<JobPositionListDto> JobPositions { get; set; }
    }
}

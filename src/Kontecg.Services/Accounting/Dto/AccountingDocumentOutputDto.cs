using System;
using Kontecg.Application.Services.Dto;
using Kontecg.MultiCompany.Dto;
using Kontecg.Timing.Dto;

namespace Kontecg.Accounting.Dto
{
    public class AccountingDocumentOutputDto : EntityDto
    {
        public DateTime MadeOn { get; set; }

        public CompanyInfoDto Company { get; set; }

        public int CompanyId { get; set; }

        public PeriodInfoDto Period { get; set; }

        public DocumentDefinitionDto DocumentDefinition { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Review { get; set; }
    }
}

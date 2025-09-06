using System;
using Kontecg.Application.Services.Dto;
using Kontecg.HumanResources.Dto;
using Kontecg.Identity.Dto;
using Kontecg.MultiCompany.Dto;
using Kontecg.Workflows;

namespace Kontecg.WorkRelations.Dto
{
    public class LegalEmploymentDocumentDto : CreationAuditedEntityDto<long>
    {
        public CompanyInfoDto Company { get; set; }

        public DateTime MadeOn { get; set; }

        public DateTime EffectiveSince { get; set; } 

        public int Exp { get; set; }

        public PersonDto Person { get; set; }

        public string Code { get; set; }

        public InnerPartEmploymentDocumentDto Before { get; set; }

        public InnerPartEmploymentDocumentDto After { get; set; }

        public ContractType Contract { get; set; }

        public EmploymentType Type { get; set; }

        public string DisplaySummary { get; set;}

        public DateTime? ExpirationDate { get; set; }

        public SignDto MadeBy { get; set; }

        public SignDto ReviewedBy { get; set; }

        public SignDto Approved1By { get; set; }

        public SignDto Approved2By { get; set; }

        public SignDto Registered1By { get; set; }

        public SignDto Registered2By { get; set; }

        public ReviewStatus Review { get; set; }
    }
}

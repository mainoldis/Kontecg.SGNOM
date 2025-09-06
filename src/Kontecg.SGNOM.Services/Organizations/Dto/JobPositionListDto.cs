using Kontecg.Application.Services.Dto;
using Kontecg.Calendar.Dto;
using Kontecg.MultiCompany.Dto;
using Kontecg.Salary;
using Kontecg.WorkRelations.Dto;

namespace Kontecg.Organizations.Dto
{
    public class JobPositionListDto : EntityDto
    {
        public TemplateListDto Template { get; set; }

        public string Code { get; set; }

        public CompanyInfoDto Company { get; set; }

        public string OrganizationUnitCode { get; set; }

        public WorkPlaceUnitDto OrganizationUnit { get; set; }

        public int CenterCost { get; set; }

        public EmployeeSalaryForm EmployeeSalaryForm { get; set; }

        public OccupationListDto Occupation { get; set; }

        public string ScholarshipLevel { get; set; }

        public WorkShiftDto WorkShift { get; set; }

        public EmploymentDocumentInfoDto Document { get; set; }

        public EmploymentDocumentInfoDto TemporalDocument { get; set; }
    }
}

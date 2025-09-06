using Kontecg.Application.Services.Dto;

namespace Kontecg.Organizations.Dto
{
    public class TemplateListDto : EntityDto
    {
        public WorkPlaceUnitDto OrganizationUnit { get; set; }

        public int CenterCost { get; set; }

        public string EmployeeSalaryForm { get; set; }

        public OccupationListDto Occupation { get; set;}

        public string ScholarshipLevel { get; set; }

        public int Proposals { get; set; }

        public int? Approved { get; set; }
    }
}

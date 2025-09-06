using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.HumanResources;
using Kontecg.Json;
using Kontecg.MultiCompany;
using Kontecg.Salary;

namespace Kontecg.Organizations
{
    [Table("templates", Schema = "est")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class Template : AuditedEntity, IMustHaveOrganizationUnit, IMustHaveCompany
    {
        public virtual int DocumentId { get; set; }

        [Required]
        [ForeignKey("DocumentId")]
        public virtual TemplateDocument Document { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [NotMapped]
        public virtual Company Company { get; set; }
        
        [Required]
        public virtual long OrganizationUnitId { get; set; }

        [NotMapped]
        public virtual WorkPlaceUnit OrganizationUnit { get; set; }

        [Required]
        [StringLength(Kontecg.Organizations.OrganizationUnit.MaxCodeLength)]
        public virtual string OrganizationUnitCode { get; set; }

        [Required]
        public virtual int CenterCost { get; set; }

        [Required]
        public virtual EmployeeSalaryForm EmployeeSalaryForm { get; set; }

        public virtual int OccupationId { get; set; }

        [Required]
        [ForeignKey("OccupationId")]
        public virtual Occupation Occupation { get; set; }

        public virtual int? ScholarshipLevelId { get; set; }

        [ForeignKey("ScholarshipLevelId")]
        public virtual ScholarshipLevelDefinition ScholarshipLevel { get; set; }

        [Required]
        [StringLength(250)]
        public virtual string WorkShift { get; set; }

        [Required]
        public virtual int Proposals { get; set; }

        public virtual int? Approved { get; set; }

        public List<TemplateJobPosition> JobPositions { get; set; }

        public Template()
        {
            string[] pattern = {"N"};

            EmployeeSalaryForm = EmployeeSalaryForm.Royal;
            Proposals = 1;
            Approved = 0;
            WorkShift = pattern.ToJsonString();
            JobPositions = new List<TemplateJobPosition>();
        }
    }
}

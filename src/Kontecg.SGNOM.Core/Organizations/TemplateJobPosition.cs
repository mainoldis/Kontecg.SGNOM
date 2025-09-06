using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;
using Kontecg.Salary;
using Kontecg.Timing;
using Kontecg.WorkRelations;

namespace Kontecg.Organizations
{
    [Table("template_job_positions", Schema = "est")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class TemplateJobPosition : CreationAuditedEntity, IMustHaveOrganizationUnit, IMustHaveCompany
    {
        public const int MaxCodeLength = 4;

        public virtual int TemplateId { get; set; }

        [Required]
        [ForeignKey("TemplateId")]
        public virtual Template Template { get; set; }

        public virtual int CompanyId { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

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

        public virtual int WorkShiftId { get; set; }

        [Required]
        [ForeignKey("WorkShiftId")]
        public virtual WorkShift WorkShift { get; set; }

        [Required]
        public virtual bool ByAssignment { get; set; }

        [Required]
        public virtual bool ByOfficial { get; set; }

        public virtual long? DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual EmploymentDocument Document { get; set; }

        public virtual long? TemporalDocumentId { get; set; }

        [ForeignKey("TemporalDocumentId")]
        public virtual EmploymentDocument TemporalDocument { get; set; }
        
        public TemplateJobPosition()
        {
        }

        public static TemplateJobPosition CreateFromTemplate(Template template)
        {
            return new TemplateJobPosition()
            {
                Code = "AUTO",
                CompanyId = template.CompanyId,
                TemplateId = template.Id,
                OrganizationUnitId = template.OrganizationUnitId,
                OrganizationUnitCode = template.OrganizationUnitCode,
                CenterCost = template.CenterCost,
                EmployeeSalaryForm = template.EmployeeSalaryForm,
                OccupationId = template.OccupationId,
                ScholarshipLevelId = template.ScholarshipLevelId
            };
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("person_scholarships", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PersonScholarship : AuditedEntity
    {
        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        public virtual int ScholarshipLevelId { get; set; }

        [Required]
        [ForeignKey("ScholarshipLevelId")]
        public virtual ScholarshipLevelDefinition ScholarshipLevel { get; set; }

        public virtual int? DegreeId { get; set; }

        [ForeignKey("DegreeId")]
        public virtual DegreeDefinition Degree { get; set; }

        public virtual DateTime? Graduation { get; set; }

        public virtual string StudyCenter { get; set; }
    }
}

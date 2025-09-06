using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.Organizations
{
    [Table("qualification_requirement_definitions", Schema = "est")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class QualificationRequirementDefinition : Entity, IMustHaveCompany
    {
        public const int MaxDisplayNameLength = 450;

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public virtual int OccupationId { get; set; }

        [Required]
        [ForeignKey("OccupationId")]
        public virtual Occupation Occupation { get; set; }

        public QualificationRequirementDefinition()
        {
        }

        public QualificationRequirementDefinition(string displayName)
            :this()
        {
            DisplayName = displayName;
        }
    }
}

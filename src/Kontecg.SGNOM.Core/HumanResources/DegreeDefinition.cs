using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("degree_definitions", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class DegreeDefinition : Entity
    {
        [Required]
        [StringLength(Person.MaxScholarshipLength)]
        public virtual string DisplayName { get; set; }

        public DegreeDefinition()
        {
        }

        public DegreeDefinition(string displayName)
            : this()
        {
            DisplayName = displayName;
            SetDescriptionNormalized();
        }

        protected virtual void SetDescriptionNormalized()
        {
            DisplayName = DisplayName?.ToUpperInvariant();
        }

        public override string ToString()
        {
            return $"{DisplayName}";
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("military_location_definitions", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class MilitaryLocationDefinition : Entity
    {
        public const int MaxDisplayNameLength = 150;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public MilitaryLocationDefinition()
        {
        }

        public MilitaryLocationDefinition(string displayName)
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

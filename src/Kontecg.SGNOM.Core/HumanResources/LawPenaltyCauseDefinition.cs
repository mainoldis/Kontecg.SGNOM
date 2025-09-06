using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("law_penalty_cause_definitions", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class LawPenaltyCauseDefinition : Entity
    {
        public const int MaxDisplayNameLength = 1000;

        public const int MaxLegalLength = 100;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(MaxLegalLength)]
        public virtual string Legal { get; set; }

        public LawPenaltyCauseDefinition()
        {
        }

        public LawPenaltyCauseDefinition(string displayName, string legal)
            : this()
        {
            DisplayName = displayName;
            Legal = legal;
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

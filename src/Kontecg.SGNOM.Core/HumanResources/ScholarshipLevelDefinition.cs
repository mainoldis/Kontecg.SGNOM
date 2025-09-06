using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("scholarship_level_definitions", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class ScholarshipLevelDefinition : Entity
    {
        public const int MaxAcronymLength = 10;

        public const int MaxDisplayNameLength = 150;

        [Required]
        [StringLength(MaxAcronymLength)]
        public virtual string Acronym { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        public virtual ScopeData Scope { get; set; }

        public virtual decimal Weight { get; set; }

        public ScholarshipLevelDefinition()
        {
            Scope = ScopeData.Personal;
        }

        public ScholarshipLevelDefinition(string displayName, string acronym, decimal weight)
            : this()
        {
            DisplayName = displayName;
            Acronym = acronym;
            Weight = weight;
            SetDescriptionNormalized();
        }

        public ScholarshipLevelDefinition(string displayName, string acronym, ScopeData scope, decimal weight)
        {
            DisplayName = displayName;
            Acronym = acronym;
            Scope = scope;
            Weight = weight;
            SetDescriptionNormalized();
        }

        protected virtual void SetDescriptionNormalized()
        {
            Acronym = Acronym?.ToUpperInvariant();
        }

        public override string ToString()
        {
            return $"{Acronym}";
        }
    }
}

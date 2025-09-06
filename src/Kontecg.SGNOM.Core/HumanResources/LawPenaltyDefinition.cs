using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("law_penalty_definitions", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class LawPenaltyDefinition : Entity
    {
        public const int MaxDisplayNameLength = 150;

        public const int MaxDescriptionLength = 500;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        public virtual bool Severe { get; set; }

        public virtual TimeSpan? Rehab { get; set; }

        public LawPenaltyDefinition()
        {
        }

        public LawPenaltyDefinition(string displayName, string description)
            : this()
        {
            DisplayName = displayName;
            Description = description;

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

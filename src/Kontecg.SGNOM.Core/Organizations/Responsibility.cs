using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.Organizations
{
    [Table("responsibility_levels", Schema = "est")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class Responsibility : Entity
    {
        public const int MaxDisplayNameLength = 150;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string NormalizedDescription { get; set; }

        public Responsibility()
        {
        }

        public Responsibility(string description)
        {
            DisplayName = description;

            SetDescriptionNormalized();
        }

        protected virtual void SetDescriptionNormalized()
        {
            NormalizedDescription = DisplayName?.ToUpperInvariant();
        }

        public override string ToString()
        {
            return $"{DisplayName}";
        }
    }
}

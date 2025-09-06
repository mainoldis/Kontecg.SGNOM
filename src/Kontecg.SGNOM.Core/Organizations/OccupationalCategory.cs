using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.Organizations
{
    [Table("occupational_categories", Schema = "est")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class OccupationalCategory : Entity
    {
        public const int MaxDisplayNameLength = 50;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        public virtual char Code { get; set; }

        public OccupationalCategory()
        {
        }

        public OccupationalCategory(char code, string displayName)
        {
            Code = code;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return $"{Code}";
        }
    }
}

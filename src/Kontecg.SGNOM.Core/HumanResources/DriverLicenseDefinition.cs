using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("driver_license_definitions", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class DriverLicenseDefinition : Entity
    {
        public const int MaxDisplayNameLength = 250;

        public const int MaxCategoryMaxLength = 3;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(MaxCategoryMaxLength)]
        public virtual string Category { get; set; }
    }
}

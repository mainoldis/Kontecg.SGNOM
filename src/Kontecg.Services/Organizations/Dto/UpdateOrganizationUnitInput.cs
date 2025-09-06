using System.ComponentModel.DataAnnotations;

namespace Kontecg.Organizations.Dto
{
    public class UpdateOrganizationUnitInput
    {
        [Range(1, long.MaxValue)] public long Id { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        [StringLength(OrganizationUnit.MaxCodeUnitLength)]
        public string Code { get; set; }
    }
}

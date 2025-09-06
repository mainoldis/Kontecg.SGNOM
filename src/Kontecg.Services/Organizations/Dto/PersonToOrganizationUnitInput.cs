using System.ComponentModel.DataAnnotations;

namespace Kontecg.Organizations.Dto
{
    public class PersonToOrganizationUnitInput
    {
        [Range(1, long.MaxValue)] public long PersonId { get; set; }

        [Range(1, long.MaxValue)] public long OrganizationUnitId { get; set; }
    }
}

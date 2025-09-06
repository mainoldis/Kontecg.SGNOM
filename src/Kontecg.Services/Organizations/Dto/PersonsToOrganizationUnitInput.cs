using System.ComponentModel.DataAnnotations;

namespace Kontecg.Organizations.Dto
{
    public class PersonsToOrganizationUnitInput
    {
        public long[] PersonIds { get; set; }

        [Range(1, long.MaxValue)] public long OrganizationUnitId { get; set; }
    }
}

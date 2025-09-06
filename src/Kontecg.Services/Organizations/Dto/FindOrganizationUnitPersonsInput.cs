using Kontecg.Dto;

namespace Kontecg.Organizations.Dto
{
    public class FindOrganizationUnitPersonsInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}

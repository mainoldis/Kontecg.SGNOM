using Kontecg.Dto;

namespace Kontecg.Organizations.Dto
{
    public class FindTemplateInputDto : PagedAndFilteredInputDto
    {
        public int DocumentId { get; set; }

        public string OrganizationUnitCode { get; set; }

        public string Contract { get; set; }
    }
}

using Kontecg.Dto;

namespace Kontecg.Common.Dto
{
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? CompanyId { get; set; }

        public bool ExcludeCurrentUser { get; set; }
    }
}

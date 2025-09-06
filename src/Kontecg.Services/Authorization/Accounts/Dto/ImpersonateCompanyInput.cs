using System.ComponentModel.DataAnnotations;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class ImpersonateCompanyInput
    {
        public int? CompanyId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}

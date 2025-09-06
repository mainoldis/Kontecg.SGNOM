using System.ComponentModel.DataAnnotations;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class IsCompanyAvailableInput
    {
        [Required]
        [MaxLength(KontecgCompanyBase.MaxCompanyNameLength)]
        public string CompanyName { get; set; }
    }
}

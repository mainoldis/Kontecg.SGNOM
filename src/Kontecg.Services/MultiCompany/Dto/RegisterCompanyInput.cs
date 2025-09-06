using System.ComponentModel.DataAnnotations;
using Kontecg.Auditing;
using Kontecg.Authorization.Users;
using Kontecg.HumanResources;

namespace Kontecg.MultiCompany.Dto
{
    public class RegisterCompanyInput
    {
        [Required]
        [StringLength(KontecgCompanyBase.MaxCompanyNameLength)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(KontecgUserBase.NameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(Company.MaxReupLength)]
        public string Reup { get; set; }

        [Required]
        [StringLength(Company.MaxOrganismLength)]
        public string Organism { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(KontecgUserBase.EmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(KontecgUserBase.PlainPasswordLength)]
        [DisableAuditing]
        public string AdminPassword { get; set; }

        [Required]
        public Address Address { get; set; }
    }
}

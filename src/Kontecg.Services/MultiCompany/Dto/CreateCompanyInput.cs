using System.ComponentModel.DataAnnotations;
using Kontecg.Auditing;
using Kontecg.Authorization.Users;

namespace Kontecg.MultiCompany.Dto
{
    public class CreateCompanyInput
    {
        [Required]
        [StringLength(KontecgCompanyBase.MaxCompanyNameLength)]
        [RegularExpression(KontecgCompanyBase.CompanyNameRegex)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(KontecgCompanyBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(Company.MaxReupLength)]
        public string Reup { get; set; }

        [Required]
        [StringLength(Company.MaxOrganismLength)]
        public string Organism { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Phone]
        public string Fax { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(KontecgUserBase.EmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(KontecgUserBase.PasswordLength)]
        [DisableAuditing]
        public string AdminPassword { get; set; }

        [MaxLength(KontecgCompanyBase.MaxConnectionStringLength)]
        [DisableAuditing]
        public string ConnectionString { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public bool SendActivationEmail { get; set; }

        public bool IsActive { get; set; }
    }
}

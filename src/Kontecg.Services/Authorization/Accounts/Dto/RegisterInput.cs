using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kontecg.Auditing;
using Kontecg.Authorization.Users;
using Kontecg.Extensions;
using Kontecg.Validation;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class RegisterInput : IValidatableObject
    {
        [Required]
        [StringLength(KontecgUserBase.NameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(KontecgUserBase.SurnameLength)]
        public string Surname { get; set; }

        [Required]
        [StringLength(KontecgUserBase.UserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(KontecgUserBase.EmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(KontecgUserBase.PlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserName.IsNullOrEmpty())
                if (!UserName.Equals(EmailAddress, StringComparison.OrdinalIgnoreCase) &&
                    ValidationHelper.IsEmail(UserName))
                    yield return new ValidationResult(
                        "Username cannot be an email address unless it's same with your email address !");
        }
    }
}

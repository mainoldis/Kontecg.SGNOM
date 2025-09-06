using System.ComponentModel.DataAnnotations;
using Kontecg.Auditing;
using Kontecg.Domain.Entities;

namespace Kontecg.Authorization.Users.Dto
{
    //Mapped to/from User in CustomDtoMapper
    public class UserEditDto : IPassivable
    {
        /// <summary>
        ///     Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }

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

        [StringLength(UserConsts.PhoneNumberLength)]
        public string PhoneNumber { get; set; }

        // Not used "Required" attribute since empty value is used to 'not change password'
        [StringLength(KontecgUserBase.PlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public virtual bool IsLockoutEnabled { get; set; }

        public bool IsActive { get; set; }
    }
}

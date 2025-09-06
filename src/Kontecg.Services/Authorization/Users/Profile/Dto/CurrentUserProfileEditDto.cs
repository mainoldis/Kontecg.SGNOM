using System.ComponentModel.DataAnnotations;

namespace Kontecg.Authorization.Users.Profile.Dto
{
    public class CurrentUserProfileEditDto
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
        [StringLength(KontecgUserBase.EmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(UserConsts.PhoneNumberLength)]
        public string PhoneNumber { get; set; }

        public virtual bool IsPhoneNumberConfirmed { get; set; }

        public string Timezone { get; set; }
    }
}

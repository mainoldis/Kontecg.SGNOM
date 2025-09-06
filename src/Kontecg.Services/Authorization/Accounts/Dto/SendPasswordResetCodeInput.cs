using System.ComponentModel.DataAnnotations;
using Kontecg.Authorization.Users;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class SendPasswordResetCodeInput
    {
        [Required]
        [MaxLength(KontecgUserBase.EmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}

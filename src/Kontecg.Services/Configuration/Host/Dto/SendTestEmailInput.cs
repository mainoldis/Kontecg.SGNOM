using System.ComponentModel.DataAnnotations;
using Kontecg.Authorization.Users;

namespace Kontecg.Configuration.Host.Dto
{
    public class SendTestEmailInput
    {
        [Required]
        [MaxLength(KontecgUserBase.EmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}

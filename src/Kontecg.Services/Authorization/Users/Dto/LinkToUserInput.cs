using Kontecg.Auditing;
using System.ComponentModel.DataAnnotations;

namespace Kontecg.Authorization.Users.Dto
{
    public class LinkToUserInput
    {
        public string CompanyName { get; set; }

        [Required]
        public string UsernameOrEmailAddress { get; set; }

        [Required]
        [DisableAuditing]
        public string Password { get; set; }
    }
}

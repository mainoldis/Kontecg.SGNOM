using System.ComponentModel.DataAnnotations;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInput
    {
        [Required] public string EmailAddress { get; set; }
    }
}

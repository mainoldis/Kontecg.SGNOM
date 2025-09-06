using System.ComponentModel.DataAnnotations;

namespace Kontecg.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required] public string LanguageName { get; set; }
    }
}

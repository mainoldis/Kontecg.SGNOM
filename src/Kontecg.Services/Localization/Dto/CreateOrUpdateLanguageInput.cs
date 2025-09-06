using System.ComponentModel.DataAnnotations;

namespace Kontecg.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required] public ApplicationLanguageEditDto Language { get; set; }
    }
}

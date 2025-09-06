using System.ComponentModel.DataAnnotations;

namespace Kontecg.Localization.Dto
{
    public class SetDefaultLanguageInput
    {
        [Required]
        [StringLength(ApplicationLanguage.NameLength)]
        public virtual string Name { get; set; }
    }
}

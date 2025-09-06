using System.ComponentModel.DataAnnotations;

namespace Kontecg.Localization.Dto
{
    public class UpdateLanguageTextInput
    {
        [Required]
        [StringLength(ApplicationLanguage.NameLength)]
        public string LanguageName { get; set; }

        [Required]
        [StringLength(ApplicationLanguageText.SourceNameLength)]
        public string SourceName { get; set; }

        [Required]
        [StringLength(ApplicationLanguageText.KeyLength)]
        public string Key { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(ApplicationLanguageText.ValueLength)]
        public string Value { get; set; }
    }
}

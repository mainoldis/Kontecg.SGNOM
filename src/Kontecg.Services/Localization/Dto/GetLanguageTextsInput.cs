using System.ComponentModel.DataAnnotations;
using Kontecg.Application.Services.Dto;
using Kontecg.Extensions;
using Kontecg.Runtime.Validation;

namespace Kontecg.Localization.Dto
{
    public class GetLanguageTextsInput : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        [Required]
        [MaxLength(ApplicationLanguageText.SourceNameLength)]
        public string SourceName { get; set; }

        [StringLength(ApplicationLanguage.NameLength)]
        public string BaseLanguageName { get; set; }

        [Required]
        [StringLength(ApplicationLanguage.NameLength, MinimumLength = 2)]
        public string TargetLanguageName { get; set; }

        public string TargetValueFilter { get; set; }

        public string FilterText { get; set; }

        [Range(0, int.MaxValue)] public int MaxResultCount { get; set; } //0: Unlimited.

        [Range(0, int.MaxValue)] public int SkipCount { get; set; }

        public void Normalize()
        {
            if (TargetValueFilter.IsNullOrEmpty()) TargetValueFilter = "ALL";
        }

        public string Sorting { get; set; }
    }
}

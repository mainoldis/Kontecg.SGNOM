using System.Collections.Generic;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Localization.Dto
{
    public class GetLanguagesOutput : ListResultDto<ApplicationLanguageListDto>
    {
        public GetLanguagesOutput()
        {
        }

        public GetLanguagesOutput(IReadOnlyList<ApplicationLanguageListDto> items, string defaultLanguageName)
            : base(items)
        {
            DefaultLanguageName = defaultLanguageName;
        }

        public string DefaultLanguageName { get; set; }
    }
}

using System.Collections.Generic;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Localization.Dto
{
    public class GetLanguageForEditOutput
    {
        public GetLanguageForEditOutput()
        {
            LanguageNames = new List<ComboboxItemDto>();
            Flags = new List<ComboboxItemDto>();
        }

        public ApplicationLanguageEditDto Language { get; set; }

        public List<ComboboxItemDto> LanguageNames { get; set; }

        public List<ComboboxItemDto> Flags { get; set; }
    }
}

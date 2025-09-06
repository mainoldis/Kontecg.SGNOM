using System.Collections.Generic;
using Kontecg.Localization;

namespace Kontecg.Configuration.Dtos
{
    public class KontecgUserLocalizationConfigDto
    {
        public KontecgUserCurrentCultureConfigDto CurrentCulture { get; set; }

        public List<LanguageInfo> Languages { get; set; }

        public LanguageInfo CurrentLanguage { get; set; }

        public List<KontecgLocalizationSourceDto> Sources { get; set; }

        public Dictionary<string, Dictionary<string, string>> Values { get; set; }
    }
}

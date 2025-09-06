using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Dto;
using Kontecg.Localization.Dto;

namespace Kontecg.Localization.Exporting
{
    public interface ILanguageTextsExcelExporter
    {
		FileDto ExportToFile(string fileName, List<LanguageTextListDto> languageTextListDtos);

        Task<FileDto> ExportToFileAsync(string fileName, List<LanguageTextListDto> languageTextListDtos);
    }
}

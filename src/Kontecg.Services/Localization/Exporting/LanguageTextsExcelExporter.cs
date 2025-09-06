using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.DataExporting.Excel.NPOI;
using Kontecg.Dto;
using Kontecg.Features;
using Kontecg.Localization.Dto;
using Kontecg.MimeTypes;
using Kontecg.Storage;
using Kontecg.Timing;
using Kontecg.UI;

namespace Kontecg.Localization.Exporting
{
    [KontecgAuthorize(PermissionNames.HumanResources)]
    public class LanguageTextsExcelExporter : NpoiExcelExporterBase, ILanguageTextsExcelExporter
    {
        public LanguageTextsExcelExporter(
            ITempFileCacheManager tempFileCacheManager,
            IMimeTypeMap mimeTypeMap)
            : base(tempFileCacheManager, mimeTypeMap)
        {
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        public FileDto ExportToFile(string fileName, List<LanguageTextListDto> languageTextListDtos)
        {
            CheckFeatureEnabled();

            return CreateExcelPackage(fileName + "_" + Clock.Now.ToString("yyyyMMddHHMMss") + ".xlsx", excelPackage =>
            {
                var sheet = excelPackage.CreateSheet(L("LanguageTexts"));

                List<string> headerTexts = new()
                {
                    L("Key"),
                    L("BaseValue"),
                    L("TargetValue"),
                };

                AddHeader(sheet, headerTexts.ToArray());

                List<Func<LanguageTextListDto, int, object>> propertySelectors = new()
                {
                    (o,_) => o.Key,
                    (o, _) => o.BaseValue,
                    (o, _) => o.TargetValue,
                };

                AddObjects(
                    sheet, languageTextListDtos,
                    propertySelectors.ToArray()
                );

                for (var i = 0; i < headerTexts.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
            });
        }

        public async Task<FileDto> ExportToFileAsync(string fileName, List<LanguageTextListDto> languageTextListDtos)
        {
            return await Task.FromResult(ExportToFile(fileName, languageTextListDtos));
        }

        private void CheckFeatureEnabled()
        {
            if(!IsEnabled(CoreFeatureNames.ExportingExcelFeature))
                throw new UserFriendlyException();
        }
    }
}

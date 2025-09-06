using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.DataExporting.Excel.NPOI;
using Kontecg.Dto;
using Kontecg.Features;
using Kontecg.MimeTypes;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Timing;
using Kontecg.Timing.Timezone;
using Kontecg.UI;
using Kontecg.WorkRelations.Dto;

namespace Kontecg.WorkRelations.Exporting
{
    [KontecgAuthorize(PermissionNames.HumanResources)]
    public class EmploymentDocumentExcelExporter : NpoiExcelExporterBase, IEmploymentDocumentExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IKontecgSession _kontecgSession;

        public EmploymentDocumentExcelExporter(
            ITempFileCacheManager tempFileCacheManager,
            IMimeTypeMap mimeTypeMap,
            ITimeZoneConverter timeZoneConverter,
            IKontecgSession kontecgSession)
            : base(tempFileCacheManager, mimeTypeMap)
        {
            _timeZoneConverter = timeZoneConverter;
            _kontecgSession = kontecgSession;

            LocalizationSourceName = SGNOMConsts.LocalizationSourceName;
        }

        public FileDto ExportToFile(string fileName, List<EmploymentDocumentInfoDto> employmentDocumentInfoDtos)
        {
            CheckFeatureEnabled();

            return CreateExcelPackage(fileName + "_" + Clock.Now.ToString("yyyy_MM_dd") + ".xlsx", excelPackage =>
            {
                var sheet = excelPackage.CreateSheet(L("EmploymentDocuments"));

                List<string> headerTexts = new()
                {
                    L("Organism"),
                    L("CompanyName"),
                    L("Exp"),
                    L("FullName"),
                    L("IdentityCard"),
                    L("Gender"),
                    L("Scholarship"),
                    L("CenterCostAcronym"),
                    L("OccupationCategoryAcronym"),
                    L("ComplexityGroup"),
                    L("OccupationCode"),
                    L("OccupationDescription"),
                    L("Code"),
                    L("MadeOn"),
                    L("EffectiveSince"),
                    L("EffectiveUntil"),
                    L("Contract"),
                    L("Type"),
                    L("SubType"),
                    L("WorkPlacePaymentCode"),
                    L("WorkPlacePaymentDescription"),
                    L("FirstLevelCode"),
                    L("FirstLevelDisplayName"),
                    L("SecondLevelCode"),
                    L("SecondLevelDisplayName"),
                    L("ThirdLevelCode"),
                    L("ThirdLevelDisplayName"),
                    L("WorkShiftDisplayName"),
                    L("WorkRegimenDisplayName"),
                    L("AverageHoursPerShift"),
                    L("BaseSalary")
                };

                var dynamicHeaderText = employmentDocumentInfoDtos
                    .Select(ed => ed.Plus.Select(epi => epi.PlusDefinition.Name)).SelectMany(d => d).Distinct()
                    .ToList();

                headerTexts.AddRange(dynamicHeaderText);

                headerTexts.Add(L("TotalSalary"));
                headerTexts.Add(L("RatePerHour"));
                headerTexts.Add(L("EmployeeSalaryForm"));
                headerTexts.Add(L("DisplaySummary"));
                headerTexts.Add(L("ByAssignment"));
                headerTexts.Add(L("ByOfficial"));
                headerTexts.Add(L("Previous"));
                headerTexts.Add(L("Period"));

                AddHeader(sheet, headerTexts.ToArray());

                List<Func<EmploymentDocumentInfoDto, int, object>> propertySelectors = new()
                {
                    (o,_) => o.Organism,
                    (o,_) => o.CompanyName,
                    (o,_) => o.Exp,
                    (o,_) => o.Person?.FullName,
                    (o,_) => o.Person?.IdentityCard,
                    (o,_) => o.Person?.Gender,
                    (o,_) => o.Person?.Scholarship,
                    (o,_) => o.CenterCost,
                    (o,_) => o.OccupationCategory,
                    (o,_) => o.ComplexityGroup,
                    (o,_) => o.OccupationCode,
                    (o,_) => o.FullOccupationDescription,
                    (o,_) => o.Code,
                    (o,_) => o.MadeOn < DateTime.MaxValue ? _timeZoneConverter.Convert(o.MadeOn, _kontecgSession.CompanyId, _kontecgSession.GetUserId()) : null,
                    (o,_) => o.EffectiveSince < DateTime.MaxValue ? _timeZoneConverter.Convert(o.EffectiveSince, _kontecgSession.CompanyId, _kontecgSession.GetUserId()) : null,
                    (o,_) => o.EffectiveUntil < DateTime.MaxValue ? _timeZoneConverter.Convert(o.EffectiveUntil, _kontecgSession.CompanyId, _kontecgSession.GetUserId()) : null,
                    (o,_) => o.Contract,
                    (o,_) => o.Type,
                    (o,_) => o.SubType,
                    (o,_) => o.WorkPlacePaymentCode,
                    (o,_) => o.WorkPlacePaymentDescription,
                    (o,_) => o.FirstLevelCode,
                    (o,_) => o.FirstLevelDisplayName,
                    (o,_) => o.SecondLevelCode,
                    (o,_) => o.SecondLevelDisplayName,
                    (o,_) => o.ThirdLevelCode,
                    (o,_) => o.ThirdLevelDisplayName,
                    (o,_) => o.WorkShift.DisplayName,
                    (o,_) => o.WorkShift.Regime.LegalName,
                    (o,_) => o.WorkShift.AverageHoursPerShift,
                    (o,_) => o.Salary
                };

                for (int i = 0; i < dynamicHeaderText.Count; i++)
                {
                    propertySelectors.Add((o, index) => o.Plus.Where(p => p.PlusDefinition.Name == headerTexts[index]).Select(p => p.Amount.Amount).Sum());
                }

                propertySelectors.Add((o, _) => o.TotalSalary);
                propertySelectors.Add((o, _) => o.RatePerHour);
                propertySelectors.Add((o, _) => o.EmployeeSalaryForm);
                propertySelectors.Add((o, _) => o.DisplaySummary);
                propertySelectors.Add((o, _) => o.ByAssignment ? L("Yes") : L("No"));
                propertySelectors.Add((o, _) => o.ByOfficial ? L("Yes") : L("No"));
                propertySelectors.Add((o, _) => o.Previous?.Code);
                propertySelectors.Add((o, _) => o.Period.Duration > TimeSpan.Zero ? $"{o.Period.Start} | {o.Period.End}" : "");

                AddObjects(
                    sheet, employmentDocumentInfoDtos,
                    propertySelectors.ToArray()
                );

                var indexOfMadeOn = headerTexts.IndexOf(L("MadeOn"));
                var indexOfEffectiveSince = headerTexts.IndexOf(L("EffectiveSince"));
                var indexOfEffectiveUntil = headerTexts.IndexOf(L("EffectiveUntil"));

                for (var i = 1; i <= employmentDocumentInfoDtos.Count; i++)
                {
                    //Formatting cells
                    SetCellDataFormat(sheet.GetRow(i).Cells[indexOfMadeOn], "yyyy/mm/dd");
                    SetCellDataFormat(sheet.GetRow(i).Cells[indexOfEffectiveSince], "yyyy/mm/dd");
                    SetCellDataFormat(sheet.GetRow(i).Cells[indexOfEffectiveUntil], "yyyy/mm/dd");
                }

                for (var i = 0; i < headerTexts.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
            });
        }

        public async Task<FileDto> ExportToFileAsync(string fileName, List<EmploymentDocumentInfoDto> employmentDocumentInfoDtos)
        {
            return await Task.FromResult(ExportToFile(fileName, employmentDocumentInfoDtos));
        }

        private void CheckFeatureEnabled()
        {
            if(!IsEnabled(CoreFeatureNames.ExportingExcelFeature))
                throw new UserFriendlyException();
        }
    }
}

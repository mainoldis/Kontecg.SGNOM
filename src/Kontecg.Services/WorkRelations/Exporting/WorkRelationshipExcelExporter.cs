using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.DataExporting.Excel.NPOI;
using Kontecg.Dto;
using Kontecg.MimeTypes;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Timing;
using Kontecg.Timing.Timezone;
using Kontecg.WorkRelations.Dto;

namespace Kontecg.WorkRelations.Exporting
{
    public class WorkRelationshipExcelExporter : NpoiExcelExporterBase, IWorkRelationshipExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IKontecgSession _kontecgSession;

        public WorkRelationshipExcelExporter(
            ITempFileCacheManager tempFileCacheManager,
            IMimeTypeMap mimeTypeMap,
            ITimeZoneConverter timeZoneConverter,
            IKontecgSession kontecgSession)
            : base(tempFileCacheManager, mimeTypeMap)
        {
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
            _timeZoneConverter = timeZoneConverter;
            _kontecgSession = kontecgSession;
        }

        public FileDto ExportToFile(List<WorkRelationshipDto> workRelationshipDtos)
        {
            return CreateExcelPackage(L("WorkRelationship") + "_" + Clock.Now.ToString("yyyy_MM_dd") + ".xlsx", excelPackage =>
            {
                var sheet = excelPackage.CreateSheet(L("WorkRelationship"));

                AddHeader(
                    sheet,
                    L("Organism"),
                    L("CompanyName"),
                    L("Exp"),
                    L("Name"),
                    L("Surname"),
                    L("Lastname"),
                    L("FullName"),
                    L("IdentityCard"),
                    L("Gender"),
                    L("BirthDate"),
                    L("Age"),
                    L("CenterCostAcronym"),
                    L("OccupationCode"),
                    L("OccupationDescription"),
                    L("OccupationResponsibility"),
                    L("OccupationCategoryAcronym"),
                    L("ComplexityGroup"),
                    L("Code"),
                    L("MadeOn"),
                    L("EffectiveSince"),
                    L("EffectiveUntil"),
                    L("Contract"),
                    L("Type"),
                    L("SubType"),
                    L("WorkPlacePaymentCode"),
                    L("FirstLevelDisplayName"),
                    L("SecondLevelDisplayName"),
                    L("ThirdLevelDisplayName"),
                    L("WorkShiftDisplayName"),
                    L("WorkRegimenDisplayName"),
                    L("Salary"),
                    L("Plus"),
                    L("TotalSalary"),
                    L("RatePerHour"),
                    L("EmployeeSalaryForm"),
                    L("Height"),
                    L("Race"),
                    L("CivilStatus"),
                    L("EyeColor"),
                    L("Scholarship"),
                    L("Title"),
                    L("ShirtSize"),
                    L("PantsSize"),
                    L("OverolSize"),
                    L("FootwearSize"),
                    L("ProtectionFootwearSize"),
                    L("GumBootsSize"),
                    L("Address")
                    );

                AddObjects(
                    sheet, workRelationshipDtos,
                    (o,_) => o.Company?.Organism,
                    (o,_) => o.Company?.CompanyName,
                    (o,_) => o.Exp,
                    (o,_) => o.Person?.Name,
                    (o,_) => o.Person?.Surname,
                    (o,_) => o.Person?.Lastname,
                    (o,_) => o.Person?.FullName,
                    (o,_) => o.Person?.IdentityCard,
                    (o,_) => o.Person?.Gender,
                    (o,_) => o.Person?.BirthDate,
                    (o,_) => o.Person?.Age,
                    (o,_) => o.CenterCost,
                    (o,_) => o.OccupationCode,
                    (o,_) => o.OccupationDescription,
                    (o,_) => o.OccupationResponsibility,
                    (o,_) => o.OccupationCategory,
                    (o,_) => o.ComplexityGroup,
                    (o,_) => o.Code,
                    (o,_) => o.MadeOn.HasValue && o.MadeOn < DateTime.MaxValue ? _timeZoneConverter.Convert(o.MadeOn, _kontecgSession.CompanyId, _kontecgSession.GetUserId()) : null,
                    (o,_) => o.EffectiveSince.HasValue && o.EffectiveSince < DateTime.MaxValue ? _timeZoneConverter.Convert(o.EffectiveSince, _kontecgSession.CompanyId, _kontecgSession.GetUserId()) : null,
                    (o,_) => o.EffectiveUntil.HasValue && o.EffectiveUntil < DateTime.MaxValue ? _timeZoneConverter.Convert(o.EffectiveUntil, _kontecgSession.CompanyId, _kontecgSession.GetUserId()) : null,
                    (o,_) => o.Contract,
                    (o,_) => o.Type,
                    (o,_) => o.SubType,
                    (o,_) => o.WorkPlaceUnit?.WorkPlacePaymentCode,
                    (o,_) => o.FirstLevelDisplayName,
                    (o,_) => o.SecondLevelDisplayName,
                    (o,_) => o.ThirdLevelDisplayName,
                    (o,_) => o.WorkShiftDisplayName,
                    (o,_) => o.WorkRegimenDisplayName,
                    (o,_) => o.Salary?.Amount,
                    (o,_) => o.Plus?.Amount,
                    (o,_) => o.TotalSalary?.Amount,
                    (o,_) => o.RatePerHour,
                    (o,_) => o.EmployeeSalaryForm != null ? L(o.EmployeeSalaryForm) : string.Empty,
                    (o,_) => o.Person?.Etnia?.Height,
                    (o,_) => o.Person?.Etnia?.Race,
                    (o,_) => o.Person?.Etnia?.CivilStatus,
                    (o,_) => o.Person?.Etnia?.EyeColor,
                    (o,_) => o.Person?.Scholarship,
                    (_,_) => string.Empty,
                    (o,_) => o.Person?.ClothingSize?.Shirt,
                    (o,_) => o.Person?.ClothingSize?.Pants,
                    (o,_) => o.Person?.ClothingSize?.Overol,
                    (o,_) => o.Person?.ClothingSize?.Footwear,
                    (o,_) => o.Person?.ClothingSize?.ProtectionFootwear,
                    (o,_) => o.Person?.ClothingSize?.GumBoots,
                    (o,_) => o.Person?.Address
                );

                for (var i = 1; i <= workRelationshipDtos.Count; i++)
                {
                    //Formatting cells
                    SetCellDataFormat(sheet.GetRow(i).Cells[9], "yyyy/mm/dd");
                    SetCellDataFormat(sheet.GetRow(i).Cells[18], "yyyy/mm/dd");
                    SetCellDataFormat(sheet.GetRow(i).Cells[19], "yyyy/mm/dd");
                    SetCellDataFormat(sheet.GetRow(i).Cells[20], "yyyy/mm/dd");
                }

                for (var i = 0; i < 48; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
            });
        }

        public async Task<FileDto> ExportToFileAsync(List<WorkRelationshipDto> workRelationshipDtos)
        {
            return await Task.FromResult(ExportToFile(workRelationshipDtos));
        }
    }
}

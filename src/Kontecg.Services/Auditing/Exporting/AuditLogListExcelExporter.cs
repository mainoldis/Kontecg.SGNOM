#if false
using System.Collections.Generic;
using Kontecg.Application.Features;
using Kontecg.Auditing.Dto;
using Kontecg.DataExporting.Excel.NPOI;
using Kontecg.Dto;
using Kontecg.Extensions;
using Kontecg.Features;
using Kontecg.MimeTypes;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Timing.Timezone;

namespace Kontecg.Auditing.Exporting
{
    [RequiresFeature(AppFeatures.ExportingExcelFeature)]
    public class AuditLogListExcelExporter : NpoiExcelExporterBase, IAuditLogListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IKontecgSession _kontecgSession;

        public AuditLogListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IKontecgSession kontecgSession,
            ITempFileCacheManager tempFileCacheManager,
            IMimeTypeMap mimeTypeMap)
            : base(tempFileCacheManager, mimeTypeMap)
        {
            _timeZoneConverter = timeZoneConverter;
            _kontecgSession = kontecgSession;
        }

        public FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos)
        {
            return CreateExcelPackage(
                "AuditLogs.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("AuditLogs"));

                    AddHeader(
                        sheet,
                        L("Time"),
                        L("UserName"),
                        L("Service"),
                        L("Action"),
                        L("Parameters"),
                        L("Duration"),
                        L("IpAddress"),
                        L("Client"),
                        L("ClientInfo"),
                        L("ErrorState")
                    );

                    AddObjects(
                        sheet, auditLogListDtos,
                        _ => _timeZoneConverter.Convert(_.ExecutionTime, _kontecgSession.CompanyId, _kontecgSession.GetUserId()),
                        _ => _.UserName,
                        _ => _.ServiceName,
                        _ => _.MethodName,
                        _ => _.Parameters,
                        _ => _.ExecutionDuration,
                        _ => _.ClientIpAddress,
                        _ => _.ClientName,
                        _ => _.ClientInfo,
                        _ => _.Exception.IsNullOrEmpty() ? L("Success") : _.Exception
                        );

                    for (var i = 1; i <= auditLogListDtos.Count; i++)
                    {
                        //Formatting cells
                        SetCellDataFormat(sheet.GetRow(i).Cells[0], "yyyy-mm-dd hh:mm:ss");
                    }

                    for (var i = 0; i < 10; i++)
                    {
                        if (i.IsIn(4, 9)) //Don't AutoFit Parameters and Exception
                        {
                            continue;
                        }

                        sheet.AutoSizeColumn(i);
                    }
                });
        }

        public FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos)
        {
            return CreateExcelPackage(
                "DetailedLogs.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("DetailedLogs"));

                    AddHeader(
                        sheet,
                        L("Action"),
                        L("Object"),
                        L("UserName"),
                        L("Time")
                    );

                    AddObjects(
                        sheet, entityChangeListDtos,
                        _ => _.ChangeType.ToString(),
                        _ => _.EntityTypeFullName,
                        _ => _.UserName,
                        _ => _timeZoneConverter.Convert(_.ChangeTime, _kontecgSession.CompanyId, _kontecgSession.GetUserId())
                    );

                    for (var i = 1; i <= entityChangeListDtos.Count; i++)
                    {
                        //Formatting cells
                        SetCellDataFormat(sheet.GetRow(i).Cells[3], "yyyy-mm-dd hh:mm:ss");
                    }

                    for (var i = 0; i < 4; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                });
        }
    }
}
#endif

using System.Collections.Generic;
using Kontecg.Auditing.Dto;
using Kontecg.Dto;

namespace Kontecg.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}

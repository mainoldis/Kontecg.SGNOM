using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Dto;
using Kontecg.WorkRelations.Dto;

namespace Kontecg.WorkRelations.Exporting
{
    public interface IWorkRelationshipExcelExporter
    {
		FileDto ExportToFile(List<WorkRelationshipDto> workRelationshipDtos);

        Task<FileDto> ExportToFileAsync(List<WorkRelationshipDto> workRelationshipDtos);
    }
}

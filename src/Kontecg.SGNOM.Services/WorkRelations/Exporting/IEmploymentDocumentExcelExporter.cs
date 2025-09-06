using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Dto;
using Kontecg.WorkRelations.Dto;

namespace Kontecg.WorkRelations.Exporting
{
    public interface IEmploymentDocumentExcelExporter
    {
		FileDto ExportToFile(string fileName, List<EmploymentDocumentInfoDto> employmentDocumentInfoDtos);

        Task<FileDto> ExportToFileAsync(string fileName, List<EmploymentDocumentInfoDto> employmentDocumentInfoDtos);
    }
}

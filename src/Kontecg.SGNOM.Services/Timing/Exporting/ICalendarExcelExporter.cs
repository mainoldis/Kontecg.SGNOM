using System.Threading.Tasks;
using Kontecg.Dto;

namespace Kontecg.Timing.Exporting
{
    public interface ICalendarExcelExporter
    {
        FileDto ExportToFile(string fileName, int year, bool resumed = true);

        Task<FileDto> ExportToFileAsync(string fileName, int year, bool resumed = true);
    }
}
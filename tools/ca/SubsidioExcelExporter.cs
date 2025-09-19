using System.Collections.Generic;
using Itenso.TimePeriod;

namespace Kontecg
{
    /// <summary>
    /// Provides functionality to export subsidy output data to Excel files.
    /// </summary>
    public class SubsidioExcelExporter : EpPlusExcelExporterBase<SubsidioOutputDto>
    {
        /// <summary>
        /// Exports a list of <see cref="SubsidioOutputDto"/> objects to an Excel file within the specified time range.
        /// </summary>
        /// <param name="output">The list of subsidy output data to export.</param>
        /// <param name="range">The time range for the export.</param>
        public void ExportToFile(List<SubsidioOutputDto> output, ITimeRange range)
        {
            CreateExcelPackage("ParaPeritar", excelPackage =>
            {
                var sheet = excelPackage.Workbook.Worksheets.Add("Tiempo");
                AddHeader(
                    sheet,
                    "IdPersona",
                    "Chapa",
                    "Trabajador",
                    "AreaN1",
                    "Area",
                    "Duration"
                );

                AddObjects(
                    sheet, 2,
                    output,
                    _ => _.IdPersona,
                    _ => _.Chapa,
                    _ => _.Trabajador,
                    _ => _.AreaN1,
                    _ => _.Area,
                    _ => _.Duration
                );

                sheet.Columns.BestFit = true;
            });
        }
    }
}

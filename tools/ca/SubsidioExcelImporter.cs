using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml.Style;

namespace Kontecg
{
    /// <summary>
    /// Provides functionality to import subsidy input data from Excel files.
    /// </summary>
    public class SubsidioExcelImporter : EpPlusExcelImporterBase<SubsidioInputDto>
    {
        /// <summary>
        /// Extracts a list of <see cref="SubsidioInputDto"/> objects from the provided Excel file bytes.
        /// </summary>
        /// <param name="fileBytes">The Excel file as a byte array.</param>
        /// <returns>A list of <see cref="SubsidioInputDto"/> objects parsed from the Excel file.</returns>
        public List<SubsidioInputDto> GetFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private SubsidioInputDto ProcessExcelRow(ExcelWorksheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet, row))
                return null;

            var exceptionMessage = new StringBuilder();
            var subsidio = new SubsidioInputDto();

            try
            {
                subsidio.IdSolicitud = GetRequiredValueFromRowOrNull(worksheet, row, 1, nameof(subsidio.IdSolicitud), exceptionMessage);
                subsidio.IdPersona = GetRequiredValueFromRowOrNull(worksheet, row, 2, nameof(subsidio.IdPersona), exceptionMessage);
                subsidio.Chapa = GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(subsidio.Chapa), exceptionMessage);
                subsidio.Trabajador = GetRequiredValueFromRowOrNull(worksheet, row, 4, nameof(subsidio.Trabajador), exceptionMessage);
                subsidio.AreaN1 = GetRequiredValueFromRowOrNull(worksheet, row, 5, nameof(subsidio.AreaN1), exceptionMessage);
                subsidio.Area = GetRequiredValueFromRowOrNull(worksheet, row, 6, nameof(subsidio.Area), exceptionMessage);
                subsidio.FechaInicio = GetRequiredValueFromRowOrNull(worksheet, row, 7, nameof(subsidio.FechaInicio), exceptionMessage);
                subsidio.FechaFinal = GetRequiredValueFromRowOrNull(worksheet, row, 8, nameof(subsidio.FechaFinal), exceptionMessage);
                subsidio.Dias = GetRequiredValueFromRowOrNull(worksheet, row, 9, nameof(subsidio.Dias), exceptionMessage);
                subsidio.Importe = GetRequiredValueFromRowOrNull(worksheet, row, 10, nameof(subsidio.Importe), exceptionMessage);
                subsidio.Tipo = GetRequiredValueFromRowOrNull(worksheet, row, 11, nameof(subsidio.Tipo), exceptionMessage) ;
                subsidio.Subtipo = GetRequiredValueFromRowOrNull(worksheet, row, 12, nameof(subsidio.Subtipo), exceptionMessage);
                subsidio.Porciento = GetOptionalValueFromRowOrNull(worksheet, row, 13, exceptionMessage);
                subsidio.DiasCarencia = GetOptionalValueFromRowOrNull(worksheet, row, 14, exceptionMessage);
            }
            catch (Exception exception)
            {
                subsidio.Exception = exception.Message;
            }

            return subsidio;
        }

        private string GetRequiredValueFromRowOrNull(
            ExcelWorksheet worksheet,
            int row,
            int column,
            string columnName,
            StringBuilder exceptionMessage,
            ExcelNumberFormat? format = null)
        {
            var cell = worksheet.Cells[row, column];

            if (format != null) cell.Style.Numberformat = format;

            var cellValue = cell.Value;
            if (cellValue != null)
                return cellValue.ToString();
            exceptionMessage.Append(columnName);
            return null;
        }

        private string GetOptionalValueFromRowOrNull(ExcelWorksheet worksheet, int row, int column,
            StringBuilder exceptionMessage, ExcelNumberFormat? format = null)
        {
            var cell = worksheet.Cells[row, column];
            if (cell == null)
                return string.Empty;

            if (format != null) cell.Style.Numberformat = format;

            var cellValue = cell.Value;
            return cellValue?.ToString() ?? string.Empty;
        }

        private bool IsRowEmpty(ExcelWorksheet worksheet, int row)
        {
            var cell = worksheet.Cells[row, 1];
            return cell == null || string.IsNullOrWhiteSpace(cell.Value.ToString());
        }
    }
}

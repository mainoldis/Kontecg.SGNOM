using System.Collections.Generic;
using System.IO;
using System;
using OfficeOpenXml;

namespace Kontecg
{
    /// <summary>
    /// Abstract base class for importing entities from Excel files using EPPlus.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to import.</typeparam>
    public abstract class EpPlusExcelImporterBase<TEntity>
    {
        /// <summary>
        /// Processes an Excel file and returns a list of entities by applying the specified row processing function.
        /// </summary>
        /// <param name="fileBytes">The Excel file as a byte array.</param>
        /// <param name="processExcelRow">A function to process each row of the worksheet.</param>
        /// <returns>A list of entities extracted from the Excel file.</returns>
        public List<TEntity> ProcessExcelFile(byte[] fileBytes, Func<ExcelWorksheet, int, TEntity> processExcelRow)
        {
            var entities = new List<TEntity>();

            using (var stream = new MemoryStream(fileBytes))
            {
                using (var excelPackage = new ExcelPackage(stream))
                {
                    foreach (var worksheet in excelPackage.Workbook.Worksheets)
                    {
                        var entitiesInWorksheet = ProcessWorksheet(worksheet, processExcelRow);

                        entities.AddRange(entitiesInWorksheet);
                    }
                }
            }

            return entities;
        }

        private List<TEntity> ProcessWorksheet(ExcelWorksheet worksheet, Func<ExcelWorksheet, int, TEntity> processExcelRow)
        {
            var entities = new List<TEntity>();

            for (var i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                try
                {
                    var entity = processExcelRow(worksheet, i);

                    if (entity != null)
                    {
                        entities.Add(entity);
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            return entities;
        }
    }
}

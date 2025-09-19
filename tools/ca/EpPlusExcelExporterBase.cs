using OfficeOpenXml;
using System.Collections.Generic;
using System;
using Kontecg.Collections.Extensions;

namespace Kontecg
{
    /// <summary>
    /// Abstract base class for exporting entities to Excel files using EPPlus.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to export.</typeparam>
    public abstract class EpPlusExcelExporterBase<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EpPlusExcelExporterBase{TEntity}"/> class.
        /// </summary>
        protected EpPlusExcelExporterBase()
        {
        }

        /// <summary>
        /// Creates an Excel package and applies the specified creation action.
        /// </summary>
        /// <param name="fileName">The name of the file to create (without extension).</param>
        /// <param name="creator">The action to perform on the Excel package.</param>
        protected void CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = fileName + ".xlsx";

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                Save(excelPackage, file);
            }
        }

        /// <summary>
        /// Adds a header row to the specified worksheet.
        /// </summary>
        /// <param name="sheet">The worksheet to add the header to.</param>
        /// <param name="headerTexts">The header text values.</param>
        protected void AddHeader(ExcelWorksheet sheet, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                AddHeader(sheet, i + 1, headerTexts[i]);
            }
        }

        /// <summary>
        /// Adds a header cell to the specified worksheet at the given column index.
        /// </summary>
        /// <param name="sheet">The worksheet to add the header to.</param>
        /// <param name="columnIndex">The column index for the header cell.</param>
        /// <param name="headerText">The header text value.</param>
        protected void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
        }

        /// <summary>
        /// Adds the specified objects to the worksheet starting at the given row index.
        /// </summary>
        /// <typeparam name="T">The type of the objects to add.</typeparam>
        /// <param name="sheet">The worksheet to add the objects to.</param>
        /// <param name="startRowIndex">The row index to start adding objects.</param>
        /// <param name="items">The collection of objects to add.</param>
        /// <param name="propertySelectors">The property selectors to determine which properties to add.</param>
        protected void AddObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        /// <summary>
        /// Saves the Excel package to the specified file.
        /// </summary>
        /// <param name="excelPackage">The Excel package to save.</param>
        /// <param name="filename">The name of the file to save to.</param>
        protected void Save(ExcelPackage excelPackage, string filename)
        {
            excelPackage.SaveAs(filename);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Itenso.TimePeriod;
using Kontecg.Application.Features;
using Kontecg.DataExporting.Excel.NPOI;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Dto;
using Kontecg.Extensions;
using Kontecg.Features;
using Kontecg.MimeTypes;
using Kontecg.Storage;
using Kontecg.Timing.Dto;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Kontecg.Timing.Exporting
{
    public class CalendarExcelExporter : NpoiExcelExporterBase, ICalendarExcelExporter
    {
        private readonly ITimeCalendarProvider _timeCalendarProvider;
        private readonly IWorkShiftRepository _workShiftRepository;

        public CalendarExcelExporter(ITempFileCacheManager tempFileCacheManager, IMimeTypeMap mimeTypeMap,
            ITimeCalendarProvider timeCalendarProvider, 
            IWorkShiftRepository workShiftRepository)
            : base(tempFileCacheManager, mimeTypeMap)
        {
            _timeCalendarProvider = timeCalendarProvider;
            _workShiftRepository = workShiftRepository;
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        [RequiresFeature(CoreFeatureNames.ExportingExcelFeature)]
        public FileDto ExportToFile(string fileName, int year, bool resumed = true)
        {
            return CreateExcelPackage(fileName + "_" + (resumed ? L("Summary") + "_" : "") + year.ToString() + ".xlsx", excelPackage =>
            {
                var workShifts = GetAllWorkShift();

                if (workShifts is {Count: 0})
                {
                    var sheetRegimen = excelPackage.CreateSheet(L("WorkingCalendar"));
                    sheetRegimen.CreateRow(0);
                    return;
                }

                var specialDates = GetSpecialDateInfos();
                var calendar = _timeCalendarProvider.GetWorkTimeCalendar();

                var groupedByLegalName = workShifts.GroupBy(k => k.Regime.LegalName).ToDictionary(k => k.Key, w => w.OrderBy(o => o.VisualOrder).ToList());
                foreach (var key in groupedByLegalName.Keys)
                {
                    var sheetRegimen = excelPackage.CreateSheet(key);
                    var paterns = groupedByLegalName[key].Select(w => w).ToArray();
                    var rowIndex = 0;

                    var boldFont = sheetRegimen.Workbook.CreateFont();
                    boldFont.IsBold = true;

                    var boldCellStyle = sheetRegimen.Workbook.CreateCellStyle();
                    boldCellStyle.Alignment = HorizontalAlignment.Center;
                    boldCellStyle.BorderTop = BorderStyle.Thin;
                    boldCellStyle.BorderBottom = BorderStyle.Thin;
                    boldCellStyle.BorderLeft = BorderStyle.Thin;
                    boldCellStyle.BorderRight = BorderStyle.Thin;
                    boldCellStyle.SetFont(boldFont);
                    boldCellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    boldCellStyle.FillPattern = FillPattern.SolidForeground;

                    var regularfont = sheetRegimen.Workbook.CreateFont();
                    var regularCellStyle = sheetRegimen.Workbook.CreateCellStyle();
                    regularCellStyle.Alignment = HorizontalAlignment.Center;
                    regularCellStyle.BorderTop = BorderStyle.Thin;
                    regularCellStyle.BorderBottom = BorderStyle.Thin;
                    regularCellStyle.BorderLeft = BorderStyle.Thin;
                    regularCellStyle.BorderRight = BorderStyle.Thin;
                    regularCellStyle.SetFont(regularfont);

                    var filledCellStyle = sheetRegimen.Workbook.CreateCellStyle();
                    filledCellStyle.Alignment = HorizontalAlignment.Center;
                    filledCellStyle.BorderTop = BorderStyle.Thin;
                    filledCellStyle.BorderBottom = BorderStyle.Thin;
                    filledCellStyle.BorderLeft = BorderStyle.Thin;
                    filledCellStyle.BorderRight = BorderStyle.Thin;
                    filledCellStyle.FillForegroundColor = IndexedColors.LightBlue.Index;
                    filledCellStyle.FillPattern = FillPattern.SolidForeground;
                    filledCellStyle.SetFont(regularfont);

                    var crazyWorkShiftTimeCellStyle = sheetRegimen.Workbook.CreateCellStyle();
                    crazyWorkShiftTimeCellStyle.Alignment = HorizontalAlignment.Center;
                    crazyWorkShiftTimeCellStyle.BorderTop = BorderStyle.Thin;
                    crazyWorkShiftTimeCellStyle.BorderBottom = BorderStyle.Thin;
                    crazyWorkShiftTimeCellStyle.BorderLeft = BorderStyle.Thin;
                    crazyWorkShiftTimeCellStyle.BorderRight = BorderStyle.Thin;
                    crazyWorkShiftTimeCellStyle.FillForegroundColor = IndexedColors.Yellow.Index;
                    crazyWorkShiftTimeCellStyle.FillPattern = FillPattern.SolidForeground;
                    crazyWorkShiftTimeCellStyle.SetFont(regularfont);

                    var breakTimeCellStyle = sheetRegimen.Workbook.CreateCellStyle();
                    breakTimeCellStyle.Alignment = HorizontalAlignment.Center;
                    breakTimeCellStyle.BorderTop = BorderStyle.Thin;
                    breakTimeCellStyle.BorderBottom = BorderStyle.Thin;
                    breakTimeCellStyle.BorderLeft = BorderStyle.Thin;
                    breakTimeCellStyle.BorderRight = BorderStyle.Thin;
                    breakTimeCellStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
                    breakTimeCellStyle.FillPattern = FillPattern.SolidForeground;
                    breakTimeCellStyle.SetFont(regularfont);


                    sheetRegimen.CreateRow(rowIndex++);
                    sheetRegimen.AddMergedRegion(new CellRangeAddress(rowIndex - 1, rowIndex - 1, 0, 35));

                    var cell = sheetRegimen.GetRow(rowIndex - 1).CreateCell(0);
                    cell.SetCellValue(L("WorkingCalendar"));
                    cell.CellStyle = boldCellStyle;

                    sheetRegimen.CreateRow(rowIndex++);
                    cell = sheetRegimen.GetRow(rowIndex - 1).CreateCell(0);
                    cell.SetCellValue(year);
                    cell.CellStyle = boldCellStyle;

                    sheetRegimen.CreateRow(rowIndex++);

                    for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
                    {
                        var start = new DateTime(year, monthIndex, 1);
                        var totalDaysInMonth = start.TotalDaysInMonth();

                        sheetRegimen.CreateRow(rowIndex++);
                        sheetRegimen.CreateRow(rowIndex++);

                        for (int columnIndex = 0; columnIndex < 36; columnIndex++)
                        {
                            var cell1 = sheetRegimen.GetRow(rowIndex - 2).CreateCell(columnIndex);
                            var cell2 = sheetRegimen.GetRow(rowIndex - 1).CreateCell(columnIndex);

                            if (columnIndex == 0)
                            {
                                cell1.SetCellValue(L(((YearMonth)monthIndex).ToString()));
                                cell1.CellStyle = boldCellStyle;
                                cell1.CellStyle.Alignment = HorizontalAlignment.Left;
                                cell2.SetCellValue(L("WorkShiftDisplayName"));
                                cell2.CellStyle = boldCellStyle;
                                cell2.CellStyle.Alignment = HorizontalAlignment.Left;
                            }
                            else if (columnIndex > 0 && columnIndex <= totalDaysInMonth)
                            {
                                cell1.SetCellValue(L(start.DayOfWeek.ToString()).Substring(0, 1));
                                cell1.CellStyle = boldCellStyle;
                                cell1.CellStyle.Alignment = HorizontalAlignment.Center;
                                cell2.SetCellValue(start.Day);
                                cell2.CellStyle = boldCellStyle;
                                cell2.CellStyle.Alignment = HorizontalAlignment.Center;
                                start = start.AddDays(1);
                            }
                            else if (columnIndex == 33)
                            {
                                cell2.SetCellValue("H/T");
                                cell2.CellStyle = boldCellStyle;
                                cell2.CellStyle.Alignment = HorizontalAlignment.Center;
                            }

                            else if (columnIndex == 34)
                            {
                                cell2.SetCellValue("H/D");
                                cell2.CellStyle = boldCellStyle;
                                cell2.CellStyle.Alignment = HorizontalAlignment.Center;
                            }

                            else if (columnIndex == 35)
                            {
                                cell2.SetCellValue("R");
                                cell2.CellStyle = boldCellStyle;
                                cell2.CellStyle.Alignment = HorizontalAlignment.Center;
                            }
                        }

                        for (int paternIndex = 0; paternIndex < paterns.Length; paternIndex++)
                        {
                            var schedule = new WorkYear(year, paterns[paternIndex].ToWorkPattern(calendar));

                            foreach (var nonWorkingDay in specialDates)
                                schedule.AddDecorator(nonWorkingDay.Date, nonWorkingDay.Cause);

                            var workingPeriods = schedule.WorkingPeriods.Select(w => ObjectMapper.Map<WorkingHoursDto>(w as WorkingHours)).ToList();
                            start = new DateTime(year, monthIndex, 1);
                            sheetRegimen.CreateRow(rowIndex++);
                            var ht = workingPeriods.Where(w => w.Start.Month == monthIndex).Sum(h => h.Duration.TotalHours);
                            var hd = totalDaysInMonth * 24 - ht;
                            var r = ht != 0 ? hd / ht : 0;

                            for (int columnIndex = 0; columnIndex < 36; columnIndex++)
                            {
                                var cell1 = sheetRegimen.GetRow(rowIndex - 1).CreateCell(columnIndex);

                                if (columnIndex == 0)
                                {
                                    cell1.SetCellValue(paterns[paternIndex].DisplayName);
                                    cell1.CellStyle = boldCellStyle;
                                }
                                else if (columnIndex > 0 && columnIndex <= totalDaysInMonth)
                                {
                                    var match = workingPeriods.Where(w => w.Start.Day == start.Day && w.Start.Month == start.Month).ToList();
                                    if (match is {Count: > 0})
                                    {
                                        if (match.Any(w => w.Decorators.Any(d => d.Decorator == "CrazyWorkShiftTime")))
                                        {
                                            cell1.SetCellValue("TL");
                                            cell1.CellStyle = crazyWorkShiftTimeCellStyle;
                                        }

                                        else if (match.Any(w => w.Decorators.Any(d => d.Decorator == "EarlyNightTime" || d.Decorator == "LateNightTime")))
                                        {
                                            cell1.SetCellValue(
                                                $"{match.Sum(w => w.Decorators.Where(d => d.Decorator == "EarlyNightTime" || d.Decorator == "LateNightTime").Select(w => w.Range.Duration.TotalHours).Sum())}n");
                                            cell1.CellStyle = regularCellStyle;
                                        }
                                        else
                                        {
                                            cell1.SetCellValue($"{match.Sum(w => w.Duration.TotalHours)}");
                                            cell1.CellStyle = regularCellStyle;
                                        }

                                        //if (!resumed && match.Any(w => w.Decorators.Any(d => d.Decorator == "NationalCelebrationDayTime" ||
                                        //        d.Decorator == "NationalHolidayTime" || d.Decorator == "BreakTime") && d.Range.Start.Day == start.Day && d.Range.Start.Month == start.Month) !=
                                        //    null && cell1.StringCellValue != "TL")
                                        //{
                                        //    cell1.CellStyle = breakTimeCellStyle;
                                        //}
                                    }
                                    else
                                    {
                                        cell1.SetCellValue("D");
                                        cell1.CellStyle = filledCellStyle;
                                    }

                                    start = start.AddDays(1);
                                }
                                else if (columnIndex == 33)
                                {
                                    cell1.SetCellValue(ht.ToString("F2"));
                                    cell1.CellStyle = regularCellStyle;
                                }

                                else if (columnIndex == 34)
                                {
                                    cell1.SetCellValue(hd.ToString("F2"));
                                    cell1.CellStyle = regularCellStyle;
                                }

                                else if (columnIndex == 35)
                                {
                                    cell1.SetCellValue(r.ToString("F2"));
                                    cell1.CellStyle = regularCellStyle;
                                }
                            }
                        }

                        sheetRegimen.CreateRow(rowIndex++);
                    }

                    for (var i = 0; i < 36; i++)
                    {
                        sheetRegimen.AutoSizeColumn(i);
                    }
                }

                var host = !KontecgSession.CompanyId.HasValue;

                for (int index = 0; index < workShifts.Count; index++)
                {
                    var sheet = excelPackage.CreateSheet($"{workShifts[index].DisplayName}{(host?workShifts[index].CompanyId:"")}");

                    WorkYear schedule = new(year, workShifts[index].ToWorkPattern(calendar));
                    foreach (var nonWorkingDay in specialDates)
                        schedule.AddDecorator(nonWorkingDay.Date, nonWorkingDay.Cause);

                    var workingPeriods = schedule.WorkingPeriods.Select(w => ObjectMapper.Map<WorkingHoursDto>(w as WorkingHours)).ToList();

                    List<string> headerTexts =
                    [
                        L("Start"),
                        L("End"),
                        L("Duration")
                    ];

                    var dynamicHeaderText = workingPeriods
                        .Select(ed => ed.Decorators.Select(epi => epi)).SelectMany(d => d).OrderBy(o => o.Order)
                        .Select(d => d.Decorator).Distinct().Select(L)
                        .ToList();

                    headerTexts.AddRange(dynamicHeaderText);

                    AddHeader(sheet, headerTexts.ToArray());

                    List<Func<WorkingHoursDto, int, object>> propertySelectors =
                    [
                        (o, _) => o.Start,
                        (o, _) => o.End + o.FreeTime,
                        (o, _) => $"{o.Duration.TotalHours}{(resumed ? "" : "hrs")}"
                    ];

                    for (int i = 0; i < dynamicHeaderText.Count; i++)
                    {
                        propertySelectors.Add((o, columnIndex) => o.Decorators
                            .Where(p => L(p.Decorator) == headerTexts[columnIndex]).Select(p =>
                                (p.Range.Duration.TotalHours == 0
                                    ? ""
                                    : (resumed
                                        ? $"{p.Range.Duration.TotalHours}"
                                        : $"{p.Range.Start:yyyy/MM/dd HH:mm} - {(p.Range.End + p.Range.FreeTime):yyyy/MM/dd HH:mm} | {p.Range.Duration.TotalHours}hrs")))
                            .FirstOrDefault());
                    }

                    AddObjects(
                        sheet, workingPeriods,
                        propertySelectors.ToArray()
                    );

                    var indexOfStart = headerTexts.IndexOf(L("Start"));
                    var indexOfEnde = headerTexts.IndexOf(L("End"));

                    for (var i = 1; i <= workingPeriods.Count; i++)
                    {
                        //Formatting cells
                        SetCellDataFormat(sheet.GetRow(i).Cells[indexOfStart], "yyyy/MM/dd HH:mm");
                        SetCellDataFormat(sheet.GetRow(i).Cells[indexOfEnde], "yyyy/MM/dd HH:mm");
                    }

                    for (var i = 0; i < headerTexts.Count; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                }

            });
        }

        [RequiresFeature(CoreFeatureNames.ExportingExcelFeature)]
        public async Task<FileDto> ExportToFileAsync(string fileName, int year, bool resumed = true)
        {
            return await Task.FromResult(ExportToFile(fileName, year, resumed));
        }

        private IReadOnlyList<SpecialDateInfo> GetSpecialDateInfos()
        {
            return _timeCalendarProvider.GetSpecialDates()
                .Where(n =>
                    n.Cause != DayDecorator.NationalHoliday &&
                    n.Cause != DayDecorator.NationalCelebrationDay)
                .OrderBy(n => n.Date).ToList();
        }

        private IReadOnlyList<WorkShift> GetAllWorkShift()
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    return _workShiftRepository.GetAllIncluding(w => w.Regime).Where(w => w.IsActive).OrderBy(o => o.VisualOrder).ToList();
                }
            });
        }
    }
}
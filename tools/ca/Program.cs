using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itenso.TimePeriod;
using Kontecg.Timing;
using OfficeOpenXml;

namespace Kontecg
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.License.SetNonCommercialOrganization("ECG");
            var importer = new SubsidioExcelImporter();
            using var package = new ExcelPackage(new FileInfo("cm.xlsx"));
            var subsidios = importer.GetFromExcel(package.GetAsByteArray());

            var agrupado = subsidios.Where(s => s.Tipo == "Invalidez Temporal").GroupBy(s => s.IdPersona).Select(g => new {IdPersona = g.Key, Subsidios = subsidios.Where(s => s.Tipo == "Invalidez Temporal" && s.IdPersona == g.Key).ToList()} ).ToList();

            var calendar = WorkCalendarTool.New();

            //agrupado = agrupado.Where(p => p.IdPersona == "102955").ToList();

            var lastSixMonths = new Month(Clock.Now, calendar);
            for (int j = 0; j < 6; j++)
            {
                lastSixMonths = lastSixMonths.GetPreviousMonth();
            }

            var extremos = new TimeRange(lastSixMonths.Start - TimeSpan.FromDays(3), Clock.Now);
            Console.WriteLine($"Últimos 6 meses: {extremos}");
            
            TimeGapCalculator<TimeRange> gapCalculator = new TimeGapCalculator<TimeRange>(calendar);

            for (int i = 0; i < agrupado.Count; i++)
            {
                var range = new TimePeriodCollection(agrupado[i].Subsidios.Select(s =>
                    new TimeRange(DateTime.Parse(s.FechaInicio), DateTime.Parse(s.FechaFinal))));

                var gaps = gapCalculator.GetGaps(range, extremos);
                foreach (var gap in gaps)
                {
                    if (gap.GetDuration(new DurationProvider()).TotalDays < 4)
                    {
                        var newSubsidio = new SubsidioInputDto()
                        {
                            IdPersona = agrupado[i].IdPersona,
                            FechaInicio = gap.Start.ToString(),
                            FechaFinal = gap.End.ToString()
                        };
                        agrupado[i].Subsidios.Add(newSubsidio);
                        Console.WriteLine($"Key = {agrupado[i].IdPersona}, Gap = {gap}");
                    }
                }
            }

            List<SubsidioOutputDto> output = new();
            for (int i = 0; i < agrupado.Count; i++)
            {
                var periods = new TimePeriodCollection(agrupado[i].Subsidios.Select(s =>
                    new TimeRange(DateTime.Parse(s.FechaInicio), DateTime.Parse(s.FechaFinal))));

                var intersectionPeriods = periods.IntersectionPeriods(extremos);
                if (intersectionPeriods.Count > 0)
                {
                    var meta = agrupado[i].Subsidios.First();
                    var subsidio = new SubsidioOutputDto()
                    {
                        IdPersona = meta.IdPersona,
                        Chapa = meta.Chapa,
                        Trabajador = meta.Trabajador,
                        AreaN1 = meta.AreaN1,
                        Area = meta.Area,
                        Duration = intersectionPeriods.DurationDescription
                    };

                    if (intersectionPeriods.TotalDuration.TotalDays >= 150)
                        output.Add(subsidio);
                    Console.WriteLine($"{subsidio}");
                }
            }

            var exporter = new SubsidioExcelExporter();
            output.Sort(new SubsidioOutputComparer());
            exporter.ExportToFile(output, extremos);

            Console.ReadLine();
        }
    }
}

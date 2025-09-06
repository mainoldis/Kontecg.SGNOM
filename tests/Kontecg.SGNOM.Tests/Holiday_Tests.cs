using System;
using System.Linq;
using Itenso.TimePeriod;
using Kontecg.Accounting;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.HistoricalData;
using Kontecg.Holidays;
using Kontecg.Runtime.Session;
using Kontecg.Timing;
using Kontecg.Workflows;
using Kontecg.WorkRelations;
using NMoneys;
using Shouldly;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class Holiday_Tests : SGNOMModuleTestBase
    {
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IRepository<HolidayDocument, long> _holidayDocumentRepository;
        private readonly IHolidayHistogramRepository _holidayHistogramRepository;
        private readonly IHolidaySettingStore _holidaySettings;
        
        public Holiday_Tests()
        {
            _employmentRepository = LocalIocManager.Resolve<IEmploymentRepository>();
            _holidayDocumentRepository = LocalIocManager.Resolve<IRepository<HolidayDocument, long>>();
            _holidayHistogramRepository = LocalIocManager.Resolve<IHolidayHistogramRepository>();
            _holidaySettings = LocalIocManager.Resolve<IHolidaySettingStore>();
            KontecgSession.Use(GetDefaultCompany().Id, GetDefaultCompanyAdmin().Id);
        }

        [Theory]
        [InlineData(22631, "2025-09-10", "2025-09-13")]
        public void Should_create_an_holiday_document_Test(int exp, string since, string until)
        {
            //RULE: El inicio debe comenzar a primera hora
            var sinceDate = TimeTool.SetTimeOfDay(DateTime.Parse(since), 0, 0);
            //RULE: El final debe terminar al final del día
            var untilDate = TimeTool.SetTimeOfDay(DateTime.Parse(until).AddDays(1), 0, 0);
            
            WithUnitOfWork( () =>
            {
                var docs = _employmentRepository.LastByExp(exp);

                (_, _, decimal baseTime, Money baseAmount) = _holidayHistogramRepository.AggregateByPersonId(docs[0].PersonId, docs[0].GroupId);

                var schedule = docs[0].GetSchedule(sinceDate);

                var holidayDoc = new HolidayDocument(1, docs[0].PersonId, docs[0].Id, docs[0].GroupId);

                var interception = schedule.WorkingPeriods.IntersectionPeriods(new TimeRange(sinceDate, untilDate, true));

                interception.ShouldNotBeEmpty();
                bool isSameDay = TimeCompare.IsSameDay(interception[0].Start, sinceDate);

                isSameDay.ShouldBe(true, "Las solicitudes de vacaciones deben comenzar en un día laborable.");

                int maxAllowedDays = _holidaySettings.GetMaxAllowedDays(KontecgSession.GetCompanyId());
                interception.Count.ShouldBeLessThanOrEqualTo(maxAllowedDays, $"No se pueden solicitar mas de {maxAllowedDays} días.");

                holidayDoc.Code = "AUTO";
                holidayDoc.Days = interception.Count;
                holidayDoc.Hours = new decimal(interception.TotalDuration.TotalHours);
                holidayDoc.Since = interception.Start;
                holidayDoc.Until = interception.End;
                holidayDoc.Return = schedule.GetNextWorkingTimeRange(interception.Last()).Start;
                
                baseTime.ShouldBeGreaterThanOrEqualTo(holidayDoc.Hours, "No tiene tiempo acumulado.");

                holidayDoc.RatePerDay = decimal.Divide(baseAmount.Amount, baseTime);
                holidayDoc.Amount = new Money(new decimal(interception.TotalDuration.TotalHours) * holidayDoc.RatePerDay, baseAmount.CurrencyCode);
                holidayDoc.Currency = baseAmount.CurrencyCode;

                baseTime -= holidayDoc.Hours;
                baseAmount -= holidayDoc.Amount;

                //baseTime.ShouldBe(12,8M, "El tiempo rebajado del submayor debió ser 44 horas");

                WorkCalendarTool.GetMonthOf(holidayDoc.Since, out int _, out YearMonth sinceMonth);
                WorkCalendarTool.GetMonthOf(holidayDoc.Until, out int _, out YearMonth untilMonth);

                var sincePeriod = schedule.GetMonth(sinceMonth);
                var sincePeriodInterception = sincePeriod.WorkingPeriods.IntersectionPeriods(holidayDoc.ToTimePeriod());

                holidayDoc.Notes.Add(new HolidayNote()
                {
                    Since = holidayDoc.Since,
                    Until = holidayDoc.Until <= sincePeriod.End ? holidayDoc.Until : sincePeriod.End,
                    Hours = new decimal(sincePeriodInterception.TotalDuration.TotalHours),
                    Days = sincePeriodInterception.Count,
                    Amount = new Money(new decimal(sincePeriodInterception.TotalDuration.TotalHours) * holidayDoc.RatePerDay, baseAmount.CurrencyCode),
                    Currency = baseAmount.CurrencyCode
                });

                if (sinceMonth != untilMonth)
                {
                    var untilPeriod = schedule.GetMonth(untilMonth);
                    var untilPeriodInterception = untilPeriod.WorkingPeriods.IntersectionPeriods(holidayDoc.ToTimePeriod());
                    holidayDoc.Notes.Add(new HolidayNote()
                    {
                        Since = untilPeriod.Start,
                        Until = holidayDoc.Until,
                        Hours = new decimal(untilPeriodInterception.TotalDuration.TotalHours),
                        Days = untilPeriodInterception.Count,
                        Amount = new Money(new decimal(untilPeriodInterception.TotalDuration.TotalHours) * holidayDoc.RatePerDay, baseAmount.CurrencyCode),
                        Currency = baseAmount.CurrencyCode
                    });
                }

                _holidayDocumentRepository.Insert(holidayDoc);

            }, new UnitOfWorkOptions() { IsTransactional = false });
        }

        [Theory]
        [InlineData(22631)]
        public void Should_liquidated_an_holiday_document_Test(int exp)
        {
            WithUnitOfWork(() =>
            {
                var docs = _employmentRepository.LastByExp(exp);

                (_, _, decimal baseTime, Money baseAmount) = _holidayHistogramRepository.AggregateByPersonId(docs[0].PersonId, docs[0].GroupId);

                var holidayDoc = new HolidayDocument(1, docs[0].PersonId, docs[0].Id, docs[0].GroupId, HolidayType.Liquidation)
                    {
                        Code = "AUTO",
                        GroupId = docs[0].GroupId,
                        Days = Math.Truncate(baseTime / docs[0].WorkShift.AverageHoursPerShift).To<int>(),
                        Hours = baseTime
                    };

                baseTime.ShouldBeGreaterThanOrEqualTo(holidayDoc.Hours, "No tiene tiempo acumulado.");

                holidayDoc.RatePerDay = baseAmount.Amount;
                holidayDoc.Amount = baseAmount;
                holidayDoc.Currency = baseAmount.CurrencyCode;
                holidayDoc.Review = ReviewStatus.Confirmed;

                baseTime -= holidayDoc.Hours;
                baseAmount -= holidayDoc.Amount;

                holidayDoc.Notes.Add(new HolidayNote()
                {
                    Since = holidayDoc.Since,
                    Until = holidayDoc.Until,
                    Days = holidayDoc.Days,
                    Hours = holidayDoc.Hours,
                    Amount = holidayDoc.Amount,
                    Currency = baseAmount.CurrencyCode
                });

                _holidayDocumentRepository.Insert(holidayDoc);

            }, new UnitOfWorkOptions() { IsTransactional = false });
        }

        [Theory]
        [InlineData(22631, 1, 2024, 7, 36.26, 2326.87)]
        [InlineData(22631,1, 2024, 8,9.6, 615.93)]
        public void Should_create_some_data_on_holiday_histogram_Test(int exp, int documentId, int year, int month, decimal hours, decimal import)
        {
            WithUnitOfWork(() =>
            {
                var period = new Month(year, (YearMonth)month, WorkCalendarTool.New());

                var docs = _employmentRepository.LastByExp(exp);

                (_, _, decimal baseTime, Money baseAmount) = _holidayHistogramRepository.AggregateByPersonId(docs[0].PersonId, docs[0].GroupId);

                var holidayHistogramEntry = new HolidayHistogram(docs[0].CompanyId, 0, documentId, docs[0].PersonId,
                    docs[0].GroupId, period.Start, period.End, hours, new Money(import, CurrencyIsoCode.CUP))
                    {
                        Status = AccountingNoteStatus.Made
                    };

                _holidayHistogramRepository.Insert(holidayHistogramEntry);
            });
        }

        [Fact]
        public void Should_resume_holiday_doc_Test()
        {
            WithUnitOfWork(() =>
            {
                var holidayHistogram = _holidayHistogramRepository.Aggregate();
            });
        }
    }
}

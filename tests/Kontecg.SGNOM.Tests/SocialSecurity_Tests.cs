using System;
using System.Linq;
using Itenso.TimePeriod;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.HistoricalData;
using Kontecg.Salary;
using Kontecg.SocialSecurity;
using Kontecg.Timing;
using Kontecg.Workflows;
using Kontecg.WorkRelations;
using NMoneys;
using Shouldly;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class SocialSecurity_Tests : SGNOMModuleTestBase
    {
        private readonly IDocumentDefinitionRepository _documentDefinitionRepository;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IRepository<SubsidyDocument, long> _subsidyRepository;
        private readonly ISocialSecuritySettingStore _socialSecuritySettings;
        private readonly IPaymentHistogramRepository _paymentHistogramRepository;
        private readonly IRepository<SubsidyPaymentDefinition> _subsidyPaymentDefinitionRepository;
        private readonly IRepository<Period> _periodRepository;
        private readonly ITimeCalendar _calendar;
        private readonly SpecialDateInfo[] _specialDates;

        /// <inheritdoc />
        public SocialSecurity_Tests()
        {
            var timeCalendarProvider = LocalIocManager.Resolve<ITimeCalendarProvider>();
            _calendar = timeCalendarProvider.GetWorkTimeCalendar();
            _specialDates = timeCalendarProvider.GetSpecialDates()
                                                .Where(n =>
                                                    n.Cause != DayDecorator.NationalHoliday &&
                                                    n.Cause != DayDecorator.NationalCelebrationDay)
                                                .OrderBy(n => n.Date).ToArray();

            _documentDefinitionRepository = LocalIocManager.Resolve<IDocumentDefinitionRepository>();
            _periodRepository = LocalIocManager.Resolve<IRepository<Period>>();
            _employmentRepository = LocalIocManager.Resolve<IEmploymentRepository>();
            _workShiftRepository = LocalIocManager.Resolve<IWorkShiftRepository>();
            _subsidyRepository = LocalIocManager.Resolve<IRepository<SubsidyDocument, long>>();
            _subsidyPaymentDefinitionRepository = LocalIocManager.Resolve<IRepository<SubsidyPaymentDefinition>>();
            _paymentHistogramRepository = LocalIocManager.Resolve<IPaymentHistogramRepository>();
            _socialSecuritySettings = LocalIocManager.Resolve<ISocialSecuritySettingStore>();
            KontecgSession.Use(GetDefaultCompany().Id, GetDefaultCompanyAdmin().Id);
        }

        [Theory]
        [InlineData(22631, 12)]
        public void Get_average_Test(int exp, int months)
        {
            WithUnitOfWork(() =>
            {
                var docs = _employmentRepository.LastByExp(exp);
                docs.ShouldNotBeEmpty();

                var average = _paymentHistogramRepository.AverageByPersonId(docs[0].PersonId, docs[0].GroupId, WageAdjuster.Average, months);
            });
        }

        [Theory]
        //[InlineData(22631, "2025-03-24", "2025-04-06", 0, true)]
        //[InlineData(21000, "2025-04-01", "2025-04-30",3, false)]
        //[InlineData(22163, "2025-04-01", "2025-06-30", 0, false)]
        //[InlineData(24857, "2025-04-01", "2025-06-30", 0, false)]
        //[InlineData(24470, "2025-04-01", "2025-06-30", 0, false)]
        //[InlineData(22000, "2025-04-01", "2025-06-30", 0, false)]
        [InlineData(24821, "2025-04-01", "2025-12-31", 3, false)]
        public void Should_create_an_temporal_disability_document_Test(int exp, string since, string until, int waitingPeriodDays, bool hosp)
        {
            var sinceDate = TimeTool.SetTimeOfDay(DateTime.Parse(since), 0, 0);
            //RULE: El final debe terminar al final del día
            var untilDate = TimeTool.SetTimeOfDay(DateTime.Parse(until), 23,59, 59, 999);

            WithUnitOfWork(() =>
            {
                WorkCalendarTool.GetMonthOf(sinceDate, out int sinceYear, out YearMonth sinceMonth);

                var docDef = _documentDefinitionRepository.GetByReference("DOCSUB");
                docDef.ShouldNotBeNull();

                var docs = _employmentRepository.LastByExp(exp);

                var subPayment = _subsidyPaymentDefinitionRepository.GetAllIncluding(p => p.PaymentDefinition)
                                                                    .SingleOrDefault(p =>
                                                                        p.DisplayName == "ENFERMEDAD COMÚN" && p.IsActive);
                subPayment.ShouldNotBeNull();

                var year = sinceYear;
                var month = sinceMonth;

                var period = _periodRepository.FirstOrDefault(p => p.ReferenceGroup == "SGNOM" && p.Year == year && p.Month == month);
                period.ShouldNotBeNull();

                //Schedule en base al regimen calendario Normal por el cual se paga subsidios
                var socialSecuritySchedule = _workShiftRepository.GetWorkShiftByName("Normal").ToSchedule(_calendar, sinceDate);

                foreach (var nonWorkingDay in _specialDates)
                    socialSecuritySchedule.AddDecorator(nonWorkingDay.Date, nonWorkingDay.Cause);

                //Rango para cálculos
                var range = new TimeRange(sinceDate, untilDate, true);

                //Schedule en base al regimen calendario del trabajador, destacar que no se puede hacer movimientos de nóminas mientras la persona esta de subsidio
                var workSchedule = docs[0].GetSchedule(sinceDate);
                
                var periodForSocialSecurity = socialSecuritySchedule.WorkingPeriods.IntersectionPeriods(range);
                
                var days = periodForSocialSecurity.Count - waitingPeriodDays;

                var basePercent = subPayment.BasePercent;

                var average = _paymentHistogramRepository
                              .AverageByPersonId(docs[0].PersonId, docs[0].GroupId, WageAdjuster.Average,
                                  subPayment.PaymentDefinition.AverageMonths).Perform(x => x / 24)
                              .Round(MidpointRounding.AwayFromZero); //Fundamental el redondeo

                var subsidyDoc = new TemporalDisability(docDef.Id, docs[0].PersonId, docs[0].Id, docs[0].GroupId, days,
                    basePercent,
                    average.Amount, CurrencyIsoCode.CUP, TimeTool.GetDate(sinceDate), TimeTool.GetDate(untilDate), MedicalExpertiseStatus.None,
                    TemporalDisabilityStatus.OnSetOfDisease, waitingPeriodDays, hosp)
                {
                    SubsidyPaymentDefinitionId = subPayment.Id
                };

                if (subsidyDoc.Hospitalized) subsidyDoc.Percent -= 10; //Disminuir 10% por hospitalización

                var index = 0;
                while (sinceDate <= untilDate)
                {
                    WorkCalendarTool.GetMonthOf(sinceDate, out sinceYear, out sinceMonth);
                    var socialSecurityMonth = socialSecuritySchedule.GetMonth(sinceMonth);
                    var sincePeriodInterception = socialSecurityMonth.WorkingPeriods.IntersectionPeriods(range);
                    
                    var workMonth = workSchedule.GetMonth(sinceMonth);
                    var periodForWorkInterception = workMonth.WorkingPeriods.IntersectionPeriods(range);

                    var currency = subsidyDoc.Currency;

                    var noteStart = subsidyDoc.Since >= socialSecurityMonth.Start
                        ? subsidyDoc.Since
                        : socialSecurityMonth.Start;

                    var noteEnd = subsidyDoc.Until <= socialSecurityMonth.End
                        ? subsidyDoc.Until
                        : socialSecurityMonth.End;

                    var periodId = workMonth.GetRelation(period.ToTimePeriod()) == PeriodRelation.ExactMatch ? period.Id : (int?) null;

                    var hours = new decimal(periodForWorkInterception.TotalDuration.TotalHours);

                    var subsidizedDays = index == 0
                        ? sincePeriodInterception.Count - subsidyDoc.WaitingPeriodDays
                        : sincePeriodInterception.Count;

                    var subsidizedAmount =
                        new Money(
                            subsidyDoc.AverageAmount.Amount *
                            (sincePeriodInterception.Count - (index == 0 ? subsidyDoc.WaitingPeriodDays : 0)) *
                            subsidyDoc.Percent * 0.01M, subsidyDoc.Currency).Round(MidpointRounding.AwayFromZero);

                    var reservedForHoliday = subPayment.PaymentDefinition.SumHoursForHolidayHistogram ? subsidizedDays * 8 * 0.0909M : 0;
                    var amountReservedForHoliday = subPayment.PaymentDefinition.SumAmountForHolidayHistogram ? new Money(subsidizedAmount.Amount * 0.0909M, currency) : Money.Zero(currency);

                    if (subsidizedAmount > Money.Zero(currency))
                        subsidyDoc.Notes.Add(new SubsidyNote
                        {
                            Since = noteStart,
                            Until = noteEnd,
                            PeriodId = periodId,
                            Hours = hours,
                            Days = subsidizedDays,
                            Amount = subsidizedAmount,
                            ReservedForHoliday = reservedForHoliday,
                            AmountReservedForHoliday = amountReservedForHoliday,
                            Currency = currency
                        });

                    sinceDate = socialSecurityMonth.End;
                    index++;
                }

                subsidyDoc.Amount = Money.Total(subsidyDoc.Notes.Select(n => n.Amount));

                var monetaryDiff = subsidyDoc.Notes.Select(n => decimal.Truncate(n.Amount.Amount)).Sum() - subsidyDoc.Amount.Amount;
                //if (monetaryDiff > 0) 
                //    subsidyDoc.Notes[0].Amount = Money.Add(subsidyDoc.Notes[0].Amount, monetaryDiff);

                _subsidyRepository.Insert(subsidyDoc);

            }, new UnitOfWorkOptions() { IsTransactional = false });
        }
    }
}
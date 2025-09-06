using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Itenso.TimePeriod;
using Kontecg.Accounting;
using Kontecg.Data;
using Kontecg.Dependency;
using Kontecg.Domain;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Organizations;
using Kontecg.Runtime.Session;
using Kontecg.Salary;
using Kontecg.Timing;
using Kontecg.WorkRelations;
using NMoneys;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class TimeDistributions_Tests : SGNOMModuleTestBase
    {
        private readonly IPaymentSettingStore _paymentSettings;
        private readonly IRepository<Incident, Guid> _incidentRepository;
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly ITimeCalendarProvider _timeCalendarProvider;
        private readonly IRepository<Period> _periodRepository;
        private readonly PeriodManager _periodManager;
        private readonly IRepository<WorkPlacePayment, long> _workPlacePaymentRepository;
        private readonly IRepository<TimeDistributionDocument> _timeDistributionDocumentRepository;
        private readonly IRepository<TimeDistribution, long> _timeDistributionRepository;
        private readonly IRepository<PaymentDefinition> _paymentDefinitionRepository;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly ICodeGenerator _codeGenerator;

        public TimeDistributions_Tests()
        {
            _periodRepository = LocalIocManager.Resolve<IRepository<Period>>();
            _paymentSettings = LocalIocManager.Resolve<IPaymentSettingStore>();
            _incidentRepository = LocalIocManager.Resolve< IRepository<Incident, Guid>>();
            _workShiftRepository = LocalIocManager.Resolve<IWorkShiftRepository>();
            _timeCalendarProvider = LocalIocManager.Resolve<ITimeCalendarProvider>();
            _periodManager = LocalIocManager.Resolve<PeriodManager>();
            _workPlacePaymentRepository = LocalIocManager.Resolve<IRepository<WorkPlacePayment, long>>();
            _timeDistributionRepository = LocalIocManager.Resolve<IRepository<TimeDistribution, long>>();
            _timeDistributionDocumentRepository = LocalIocManager.Resolve<IRepository<TimeDistributionDocument>>();
            _paymentDefinitionRepository = LocalIocManager.Resolve<IRepository<PaymentDefinition>>();
            _employmentRepository = LocalIocManager.Resolve<IEmploymentRepository>();
            _codeGenerator = LocalIocManager.Resolve<ICodeGenerator>();

            KontecgSession.Use(GetDefaultCompany().Id, GetDefaultCompanyAdmin().Id);
        }

        [Fact]
        public void Should_create_an_incident_Test()
        {
            var incident = new Incident(3428, 2372, "549", new DateTime(2024, 4, 25, 7, 30, 0, Clock.Kind), 4M, "Curso");

            WithUnitOfWork(1, () =>
            {
                _incidentRepository.Insert(incident);
            }, new UnitOfWorkOptions(){IsTransactional = false});
        }

        [Fact]
        public void Should_generate_time_distribution_Test()
        {
            var stopwatch = Stopwatch.StartNew();

            //RULE: Obtener el periodo contable a pagar?
            PeriodInfo period = null;

            using IDisposableDependencyObjectWrapper<IUnitOfWorkManager> uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>();
            using IUnitOfWorkCompleteHandle uow = uowManager.Object.Begin(new UnitOfWorkOptions(){IsTransactional = false});
            using (uowManager.Object.Current.SetCompanyId(KontecgSession.CompanyId))
            {
                period = _periodManager.GetCurrentPeriod("SGNOM") ?? PeriodInfo.Create("SGNOM", Clock.Now.Year, YearMonth.August, WorkCalendarTool.New());
                List<SpecialDateInfo> nonWorkingDays = _timeCalendarProvider.GetSpecialDates()
                    .Where(n =>
                        n.Cause != DayDecorator.NationalHoliday &&
                        n.Cause != DayDecorator.NationalCelebrationDay)
                    .OrderBy(n => n.Date).ToList();
                //RULE: Obtenemos la configuración de los pagos
                var paymentSettings = _paymentSettings.GetPaymentDefinition(KontecgSession.GetCompanyId());
                var normalWorkShiftName = _paymentSettings.GetNormalWorkShiftName(KontecgSession.GetCompanyId());

                //RULE: Creamos todos los calendarios, es mas eficiente de una vez en un diccionario, es mejor que estar calculado siempre.
                Dictionary<string, WorkYear> schedules =
                    _workShiftRepository.GetAllIncluding(w => w.Regime).ToDictionary(n => n.DisplayName,
                        n => new WorkYear(period.Year, n.ToWorkPattern()));

                foreach (var schedule in schedules.Values)
                    nonWorkingDays.ForEach(d => schedule.AddDecorator(d.Date, d.Cause));
                        
                //RULE: Para la generación se debe tener en cuenta cada tipo de pago como está configurado en cuanto a aporte de tiempo
                IReadOnlyList<PaymentDefinition> tps = _paymentDefinitionRepository.GetAllList();

                //RULE: Obtener el área de pago
                var workPlacePayments = _workPlacePaymentRepository.GetAllIncluding(wp => wp.WorkPlaces).ToList();

                //RULE: Obtener las incidencias
                var incidents = _incidentRepository.GetAllList(i => i.Start >= period.Since && i.Start < period.Until);

                var relationship = _employmentRepository.RelationshipByPeriod(new FilterRequest {Year = period.Year, Month = period.Month});

                foreach (WorkPlacePayment workPlacePayment in workPlacePayments)
                {
                    //RULE: Se debe obtener la relación laboral de acuerdo al área de pago correspondiente
                    var relationshipByWorkPlace = relationship.Where(r => workPlacePayment.WorkPlaces.Exists(wp => wp.Id == r.OrganizationUnitId)).ToArray();
                    if (relationshipByWorkPlace.Any())
                    {
                        //RULE: Un documento corresponde a un área de pago?
                        var document = new TimeDistributionDocument(0, workPlacePayment.Id, period.Id, $"{workPlacePayment.Code} INCIDENCIAS {period.Year} {period.Month}",
                            period.Since, period.Until)
                        {
                            Code = _codeGenerator.CreateTimeDistributionDocumentCode(Clock.Now)
                        };

                        var docId = _timeDistributionDocumentRepository.Insert(document);
                        uowManager.Object.Current.SaveChanges();

                        foreach (EmploymentDocument currentJob in relationshipByWorkPlace)
                        {
                            //RULE: Obtengo el schedule precargado
                            var schedule = schedules[currentJob.WorkShift.DisplayName];
                            //RULE: Tener en cuenta la forma de pago (Mensual, Quincenal)
                            var cyclesToPay = schedule.GetMonth(period.Month);

                            //RULE: Si se garantiza que las fechas de los movimientos están acotadas antes de la primera jornada y la última, solo se analizaría los contenidos dentro del rango
                            var cycles =
                                cyclesToPay.WorkingPeriods.IntersectionPeriods(currentJob.ToWorkingPeriod());

                            //RULE: Decoramos con las vacaciones

                            //RULE: Insertamos primero las incidencias y luego lo que se puede generar por calendario, aquí hay que evaluar todos los tipos de pagos
                            var incidentsForCurrentJobGroupedByKey = incidents.Where(i =>
                                    i.PersonId == currentJob.PersonId && i.EmploymentId == currentJob.Id &&
                                    i.CompanyId == currentJob.CompanyId)
                                .GroupBy(i => i.Key).ToDictionary(i => i.Key, e => e.Sum(incident => incident.Hours));

                            PaymentDefinition selectedPaymentDefinition = null;
                            //RULE: Al evaluar los tipos de pagos, si son exclusivos deducen a la 504, pero siempre que estén dentro de los ciclos de trabajo.

                            decimal hours = 0;
                            foreach (KeyValuePair<string, decimal> valuePair in incidentsForCurrentJobGroupedByKey)
                            {
                                selectedPaymentDefinition = tps.First(tp => tp.Name == valuePair.Key);
                                hours += valuePair.Value;
                                TimeDistribution distribution = new()
                                {
                                    DocumentId = docId.Id,
                                    PersonId = currentJob.PersonId,
                                    Employment = currentJob,
                                    GroupId = currentJob.GroupId,
                                    CenterCost = currentJob.CenterCost,
                                    PaymentDefinition = selectedPaymentDefinition,
                                    Kind = GenerationSystemData.Reported,
                                    //RULE: La diferencia de horas debe determinarse a partir de las incidencias y por cada tipo de pago en cuanto a tiempo
                                    Hours = valuePair.Value,
                                };

                                distribution.SetRatePerHour();
                                _timeDistributionRepository.Insert(distribution);
                            }
                            
                            Dictionary<CalendarTimeDecorator, TimeSpan> tpSummary = cycles?.Summarize() ?? new Dictionary<CalendarTimeDecorator, TimeSpan>();
                            foreach (CalendarTimeDecorator key in Enum.GetValues(typeof(CalendarTimeDecorator)))
                            {
                                
                                switch (key)
                                {
                                    case CalendarTimeDecorator.BreakTime:
                                        selectedPaymentDefinition = tps.First(tp =>
                                            tp.Name == (currentJob.WorkShift.DisplayName == normalWorkShiftName
                                                ? paymentSettings.NormalBreakTime
                                                : paymentSettings.SpecialBreakTime));
                                        break;
                                    case CalendarTimeDecorator.NationalCelebrationDayTime:
                                        selectedPaymentDefinition =
                                            tps.First(tp =>
                                                tp.Name == (currentJob.WorkShift.DisplayName == normalWorkShiftName
                                                    ? paymentSettings.NormalNationalCelebrationDayTime
                                                    : paymentSettings.SpecialNationalCelebrationDayTime));
                                        break;
                                    case CalendarTimeDecorator.NationalHolidayTime:
                                        selectedPaymentDefinition =
                                            tps.First(tp =>
                                                tp.Name == (currentJob.WorkShift.DisplayName == normalWorkShiftName
                                                    ? paymentSettings.NormalNationalHolidayTime
                                                    : paymentSettings.SpecialNationalHolidayTime));
                                        break;
                                    case CalendarTimeDecorator.EarlyNightTime:
                                        selectedPaymentDefinition =
                                            tps.First(tp => tp.Name == paymentSettings.EarlyNightTime);
                                        break;
                                    case CalendarTimeDecorator.LateNightTime:
                                        selectedPaymentDefinition =
                                            tps.First(tp => tp.Name == paymentSettings.LateNightTime);
                                        break;
                                    case CalendarTimeDecorator.HolidayTime:
                                        selectedPaymentDefinition =
                                            tps.First(tp => tp.Name == paymentSettings.HolidayTime);
                                        break;
                                    case CalendarTimeDecorator.SubsidizedTime:
                                        selectedPaymentDefinition =
                                            tps.First(tp => tp.Name == paymentSettings.SubsidizedTime);
                                        break;
                                    case CalendarTimeDecorator.CrazyWorkShiftTime:
                                        selectedPaymentDefinition =
                                            tps.First(tp => tp.Name == paymentSettings.CrazyWorkingTime);
                                        break;
                                    case CalendarTimeDecorator.WorkingTime:
                                        selectedPaymentDefinition =
                                            tps.First(tp => tp.Name == paymentSettings.WorkingTime);
                                        break;
                                }

                                if (tpSummary.TryGetValue(key, out TimeSpan value) && selectedPaymentDefinition != null)
                                {
                                    switch (key)
                                    {
                                        case CalendarTimeDecorator.BreakTime:
                                        case CalendarTimeDecorator.NationalCelebrationDayTime:
                                        case CalendarTimeDecorator.NationalHolidayTime:
                                        case CalendarTimeDecorator.CrazyWorkShiftTime:
                                        case CalendarTimeDecorator.HolidayTime:
                                        case CalendarTimeDecorator.SubsidizedTime:
                                            hours += new decimal(value.TotalHours);
                                            break;
                                    }

                                    TimeDistribution distribution = new()
                                    {
                                        DocumentId = docId.Id,
                                        PersonId = currentJob.PersonId,
                                        Employment = currentJob,
                                        GroupId = currentJob.GroupId,
                                        CenterCost = currentJob.CenterCost,
                                        PaymentDefinition = selectedPaymentDefinition,
                                        Kind = GenerationSystemData.Generated,
                                        //RULE: La diferencia de horas debe determinarse a partir de las incidencias y por cada tipo de pago en cuanto a tiempo
                                        Hours = selectedPaymentDefinition.Name != "504" ? new decimal(value.TotalHours) : new decimal(value.TotalHours) - hours,
                                    };

                                    distribution.SetRatePerHour();
                                    _timeDistributionRepository.Insert(distribution);
                                }
                            }
                        }
                        uowManager.Object.Current.SaveChanges();
                    }
                }

                uow.Complete();
            }
            stopwatch.Stop();

            var milliseconds = stopwatch.ElapsedMilliseconds;
        }

        [Fact]
        public void Should_resume_time_distribution_Test()
        {
            var stopwatch = Stopwatch.StartNew();

            using IDisposableDependencyObjectWrapper<IUnitOfWorkManager> uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>();
            using IUnitOfWorkCompleteHandle uow = uowManager.Object.Begin(new UnitOfWorkOptions() { IsTransactional = false });
            using (uowManager.Object.Current.SetCompanyId(KontecgSession.CompanyId))
            {
                //RULE: Para la generación se debe tener en cuenta cada tipo de pago como está configurado en cuanto a aporte de tiempo
                IReadOnlyList<PaymentDefinition> tps = _paymentDefinitionRepository.GetAllList();

                var timeDistributionToAnalyze = _timeDistributionRepository.GetAllIncluding(t => t.Employment, t => t.Plus, t => t.PaymentDefinition)
                    .Where(e => e.Status == AccountingNoteStatus.ToAnalyze).ToList();

                for (int i = 0; i < timeDistributionToAnalyze.Count; i++)
                {
                    var toAnalyze = timeDistributionToAnalyze[i];
                    var paymentDefinitionMathType = toAnalyze.PaymentDefinition.MathType;

                    switch (paymentDefinitionMathType)
                    {
                        case MathType.MinimumWage:
                            toAnalyze.Amount = new Money(2100, CurrencyIsoCode.CUP);
                            toAnalyze.Currency = toAnalyze.Amount.Value.CurrencyCode;
                            toAnalyze.ReservedForHoliday = toAnalyze.Hours * 0.0909M;
                            toAnalyze.AmountReservedForHoliday = new Money(2100 * 0.0909M, CurrencyIsoCode.CUP);
                            break;
                        case MathType.Average:
                            break;
                        case MathType.Percent:
                            toAnalyze.Amount = new Money(toAnalyze.Hours * (toAnalyze.RatePerHour ?? 0) * toAnalyze.PaymentDefinition.Factor / 100M, CurrencyIsoCode.CUP);
                            toAnalyze.Currency = toAnalyze.Amount.Value.CurrencyCode;
                            toAnalyze.ReservedForHoliday = toAnalyze.Hours * 0.0909M;
                            toAnalyze.AmountReservedForHoliday = new Money((toAnalyze.Amount?.Amount ?? 0) * 0.0909M);
                            break;
                        case MathType.RatePerHour:
                            toAnalyze.Amount = new Money(toAnalyze.Hours * (toAnalyze.PaymentDefinition.Factor), CurrencyIsoCode.CUP);
                            toAnalyze.Currency = toAnalyze.Amount.Value.CurrencyCode;
                            toAnalyze.ReservedForHoliday = toAnalyze.Hours * 0.0909M;
                            toAnalyze.AmountReservedForHoliday = new Money((toAnalyze.Amount?.Amount ?? 0) * 0.0909M);
                            break;
                        case MathType.Formula:
                            break;
                    }

                    toAnalyze.Status = AccountingNoteStatus.ToMake;

                    _timeDistributionRepository.Update(toAnalyze);
                    uowManager.Object.Current.SaveChanges();
                }

                uow.Complete();
            }

            stopwatch.Stop();

            var milliseconds = stopwatch.ElapsedMilliseconds;
        }
    }
}

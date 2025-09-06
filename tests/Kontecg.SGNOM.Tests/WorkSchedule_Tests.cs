using System;
using System.Collections.Generic;
using System.Linq;
using Itenso.TimePeriod;
using Kontecg.Domain.Repositories;
using Kontecg.EntityHistory;
using Kontecg.Runtime.Remoting;
using Kontecg.Timing;
using Shouldly;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class WorkSchedule_Tests : SGNOMModuleTestBase
    {
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IRepository<Period> _periodRepository;
        private readonly ITimeCalendarProvider _timeCalendarProvider;

        public WorkSchedule_Tests()
        {
            _workShiftRepository = LocalIocManager.Resolve<IWorkShiftRepository>();
            _timeCalendarProvider = LocalIocManager.Resolve<ITimeCalendarProvider>();
            _periodRepository = LocalIocManager.Resolve<IRepository<Period>>();

            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;
        }

        [Fact]
        public void Create_WorkYear_Test()
        {
            WorkYear schedule = null;
            WorkShift workShift = null;

            List<SpecialDateInfo> nonWorkingDays = _timeCalendarProvider.GetSpecialDates()
                .Where(n =>
                    n.Cause != DayDecorator.NationalHoliday &&
                    n.Cause != DayDecorator.NationalCelebrationDay)
                .OrderBy(n => n.Date).ToList();

            WithUnitOfWork(1, () =>
            {
                workShift = _workShiftRepository.GetWorkShiftByName("D");
            });

            workShift.ShouldNotBeNull("Se necesita un turno existente para que ejecutar el resto de las pruebas");

            schedule = new WorkYear(Now.Today, workShift.ToWorkPattern());

            foreach (var nonWorkingDay in nonWorkingDays)
                schedule.AddDecorator(nonWorkingDay.Date, nonWorkingDay.Cause);

            var cycles = schedule.WorkingPeriods;
            cycles.ShouldNotBeEmpty();
            var totalDuration = cycles.GetTotalDuration(new DurationProvider());
            var december = schedule.GetMonth(YearMonth.December);

            var summary = december.SummaryTime;
            var timePeriodCollection = december.GetNextWorkingTimeRange(december.WorkingPeriods[^1]);

        }

        [Fact]
        public void Calculate_Hours_On_December_2024_Shift_B_Test()
        {
            WorkShift workShift = null;
            List<SpecialDateInfo> nonWorkingDays = _timeCalendarProvider.GetSpecialDates()
                .Where(n =>
                    n.Cause != DayDecorator.NationalHoliday &&
                    n.Cause != DayDecorator.NationalCelebrationDay)
                .OrderBy(n => n.Date).ToList();

            WithUnitOfWork(1, () =>
            {
                workShift = _workShiftRepository.GetWorkShiftByName("K");
            });

            workShift.ShouldNotBeNull("Se necesita un turno existente para que ejecutar el resto de las pruebas");

            WorkYear schedule = new(2024, workShift.ToWorkPattern());
            foreach (var nonWorkingDay in nonWorkingDays)
                schedule.AddDecorator(nonWorkingDay.Date, nonWorkingDay.Cause);

            var december = schedule.GetMonth(YearMonth.September);
            var summary = december.SummaryTime;

            summary.ShouldContainKey(CalendarTimeDecorator.WorkingTime);
        }

        [Fact]
        public void Calculate_Hours_On_2024_Shift_A_Test()
        {
            WorkShift workShift = null;
            List<SpecialDateInfo> nonWorkingDays = _timeCalendarProvider.GetSpecialDates().Where(n =>
                    n.Cause != DayDecorator.NationalHoliday &&
                    n.Cause != DayDecorator.NationalCelebrationDay)
                .OrderBy(n => n.Date).ToList();

            WithUnitOfWork(1, () =>
            {
                workShift = _workShiftRepository.GetWorkShiftByName("A");
            });

            workShift.ShouldNotBeNull("Se necesita un turno existente para que ejecutar el resto de las pruebas");

            WorkYear schedule = new(2024, workShift.ToWorkPattern());
            foreach (var nonWorkingDay in nonWorkingDays)
                schedule.AddDecorator(nonWorkingDay.Date, nonWorkingDay.Cause);

            var summary = schedule.SummaryTime;

            summary.ShouldContainKey(CalendarTimeDecorator.WorkingTime);
        }

        [Fact]
        public void Create_Schedule_with_Period_Test()
        {
            var opm = LocalIocManager.Resolve<PeriodManager>();
            var periodContext = LocalIocManager.Resolve<DataContextAmbientScopeProvider<PeriodInfo>>();

            WorkYear schedule = null;
            WorkShift workShift = null;
            PeriodInfo period = opm.GetCurrentPeriod("SGNOM");

            WithUnitOfWork(1, () => workShift = _workShiftRepository.GetWorkShiftByName("B"));

            using var scope = periodContext.BeginScope("Kontecg.Timing.Reason.Override", period);
            workShift.ShouldNotBeNull("Se necesita un turno existente para que ejecutar el resto de las pruebas");
            
            schedule = new WorkYear(Clock.Now.Year, workShift.ToWorkPattern());
            period = periodContext.GetValue("Kontecg.Timing.Reason.Override");
            period.ShouldNotBeNull("Se necesita un período de trabajo abierto");

            var journals = schedule.WorkingPeriods.IntersectionPeriods(period.ToTimePeriod());
        }

        [Fact]
        public void Save_all_period_of_2024_Test()
        {
            Year schedule = null;
            var calendar = Resolve<ITimeCalendarProvider>();

            WithUnitOfWork(1, () =>
            {
                schedule = new Year(2024, calendar.GetWorkTimeCalendar());
                var months = schedule.GetMonths();
                foreach (var month in months)
                {
                    var operationPeriod = new Period("SGNOM", calendar.GetWorkTimeCalendar(), month.Start, month.End);
                    _periodRepository.Insert(operationPeriod);
                }
            });
        }

        [Fact]
        [UseCase(Description = "Creando un nuevo Turno de Trabajo")]
        public void Create_new_workShift_Test()
        {
            WithUnitOfWork(1, () =>
            {
                var workShift = new WorkShift("X", 2, new DateTime(2025, 6, 16), "[07:00-16:18]*[07:00-16:18]*[07:00-16:18]*[07:00-16:18]*[07:00-16:18]*[00:00-00:00]*[00:00-00:00]", "00:30", 8.8M, "5x2 Extra", 99);
                var id = _workShiftRepository.InsertAndGetId(workShift);
            });
        }

        [Fact]
        [UseCase(Description = "Actualizando un nuevo Turno de Trabajo de Prueba")]
        public void Update_workShift_Test()
        {
            WithUnitOfWork(1, () =>
            {
                var workShift = _workShiftRepository.GetWorkShiftByName("X");
                workShift.Legal = "8.8hrs";
                _workShiftRepository.Update(workShift);
            });
        }
    }
}

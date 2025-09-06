using System;
using System.Collections.Generic;
using System.Linq;
using Itenso.TimePeriod;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;

namespace Kontecg.Timing
{
    internal class DefaultWorkTimeCalendarProvider : ITimeCalendarProvider
    {
        private static readonly TimeCalendar InternalTimeCalendarInstance = WorkCalendarTool.New();

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<SpecialDate> _specialDateRepository;

        public DefaultWorkTimeCalendarProvider(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<SpecialDate> specialDateRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _specialDateRepository = specialDateRepository;
        }

        public TimeCalendar GetWorkTimeCalendar()
        {
            return InternalTimeCalendarInstance;
        }

        /// <param name="forWorkCalculus"></param>
        /// <inheritdoc />
        public TimeCalendar GetCalendar(bool forWorkCalculus = true)
        {
            if (forWorkCalculus)
                return InternalTimeCalendarInstance;

            TimeCalendarConfig config = new()
            {
                YearType = YearType.SystemYear,
                DayNameType = CalendarNameType.Abbreviated,
                MonthNameType = CalendarNameType.Abbreviated,
            };

            return new TimeCalendar(config);
        }

        public IReadOnlyList<SpecialDateInfo> GetSpecialDates()
        {
            return _unitOfWorkManager.WithUnitOfWork(
                () => _specialDateRepository.GetAllList()
                    .Select(s => new SpecialDateInfo() {Date = s.Date, Cause = s.Cause}).ToList(),
                new UnitOfWorkOptions() {IsTransactional = false});
        }
    }
}

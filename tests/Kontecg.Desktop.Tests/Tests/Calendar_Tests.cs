using System.Threading.Tasks;
using Itenso.TimePeriod;
using Kontecg.Application.Services.Dto;
using Kontecg.Storage.Blobs;
using Kontecg.Timing;
using Kontecg.Timing.Dto;
using Kontecg.Timing.Exporting;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Calendar_Tests : DesktopTestModuleTestBase
    {
        private readonly ITimeCalendarProvider _timeCalendarProvider;
        private readonly ICalendarExcelExporter _calendarExcelExporter;

        public Calendar_Tests()
        {
            _timeCalendarProvider = Resolve<ITimeCalendarProvider>();
            _calendarExcelExporter = Resolve<ICalendarExcelExporter>();

            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;
        }

        [Fact]
        public async Task Insert_nonworking_day_Test()
        {
            var service = Resolve<SpecialDateAppService>();

            var saturday = await service.CreateAsync(new SpecialDateDto
                {Date = new Date(2022, 6, 11).ToDateTime(0), Cause = DayDecorator.BreakSaturday.ToString() });

            saturday.Cause.ShouldBe(DayDecorator.BreakSaturday.ToString());
        }

        [Fact]
        public async Task Get_all_nonworking_days_Test()
        {
            var service = Resolve<SpecialDateAppService>();

            var nonworking = await service.GetAllAsync(new PagedAndSortedResultRequestDto() {Sorting = "Date ASC"});

            nonworking.Items.ShouldContain(n => n.Cause == DayDecorator.BreakSaturday.ToString());
        }

        [Fact]
        public void Create_normal_year_Test()
        {
            
            var calendar = _timeCalendarProvider.GetWorkTimeCalendar();

            Year currentYear = new(2023, calendar);

            var sixMonthsAgo = WorkCalendarTool.GetStartOfLastSixMonths(Now.Today);
            ITimePeriod searchLimits = new TimeRange(sixMonthsAgo, WorkCalendarTool.GetStartOfMonth(Now.Today), true);
        }

        [Fact]
        public async Task Export_calendars_Test()
        {
            var year = 2024;
            var fileDto = await _calendarExcelExporter.ExportToFileAsync($"{HumanResourcesContainer.Calendars}/{year}/Calendario", year);
        }
    }
}

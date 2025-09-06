using Castle.Core.Logging;
using Kontecg.BlobStoring;
using Kontecg.Dependency;
using Kontecg.Storage;
using Kontecg.Storage.Blobs;
using Kontecg.Timing.Exporting;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Kontecg.Extensions;
using Kontecg.Runtime.Session;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Kontecg.Data;

namespace Kontecg.SampleApp
{
    public class CalendarOptionExecuter : ITransientDependency
    {
        private readonly IDbPerCompanyConnectionStringResolver _connectionStringResolver;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBlobContainer<HumanResourcesContainer> _blobContainer;
        private readonly ICalendarExcelExporter _calendarExcelExporter;

        public CalendarOptionExecuter(
            ITempFileCacheManager tempFileCacheManager, 
            IBlobContainer<HumanResourcesContainer> blobContainer, 
            ICalendarExcelExporter calendarExcelExporter, 
            IDbPerCompanyConnectionStringResolver connectionStringResolver)
        {
            _tempFileCacheManager = tempFileCacheManager;
            _blobContainer = blobContainer;
            _calendarExcelExporter = calendarExcelExporter;
            _connectionStringResolver = connectionStringResolver;
            KontecgSession = NullKontecgSession.Instance;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        public async Task RunAsync(bool skipConnVerification)
        {
            var hostConnStr =
                await _connectionStringResolver.GetNameOrConnectionStringAsync(
                    new ConnectionStringResolveArgs(MultiCompanySides.Company));
            if (hostConnStr.IsNullOrWhiteSpace())
            {
                Logger.Warn("Configuration file should contain a connection string named 'Default'");
                return;
            }

            Logger.Info("Host database: " + ConnectionStringHelper.GetConnectionString(hostConnStr));
            Logger.Info("Year..?: ");
            if (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, new NumberFormatInfo(), out int year))
                throw new FormatException("Invalid number");


            var fileDto = await _calendarExcelExporter.ExportToFileAsync($"{HumanResourcesContainer.Calendars}/{year}/Calendario", year, false);
            await _blobContainer.SaveAsync(fileDto.FileName, _tempFileCacheManager.GetFile(fileDto.FileToken), true);
        }
    }
}
using System;
using Kontecg.Auditing;
using Kontecg.Auditing.Dto;
using Kontecg.IO;
using Kontecg.Logging;
using Kontecg.Storage;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Logging_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public void Get_latest_logs_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var logService = LocalIocManager.Resolve<ILogAppService>();

            var latestLogs = logService.GetLatestLogs();
            latestLogs.ShouldNotBeNull();
        }

        [Fact]
        public void Exporting_log_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var logService = LocalIocManager.Resolve<ILogAppService>();
            var tempFileCacheManager = LocalIocManager.Resolve<ITempFileCacheManager>();

            var downloadLogs = logService.DownloadLogs();
            downloadLogs.ShouldNotBeNull();

            var bytes = tempFileCacheManager.GetFile(downloadLogs.FileToken);
            bytes.ShouldNotBeNull();
            AppFileHelper.SaveToFile(downloadLogs.FileName, bytes);}

        [Fact]
        public async void Exporting_auditlogs_to_excel_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);

            var auditService = LocalIocManager.Resolve<IAuditLogAppService>();
            var tempFileCacheManager = LocalIocManager.Resolve<ITempFileCacheManager>();

            var fileToExportDto =
                await auditService.GetAuditLogsToExcel(new GetAuditLogsInput {EndDate = DateTime.Now});

            fileToExportDto.ShouldNotBeNull();

            var bytes = tempFileCacheManager.GetFile(fileToExportDto.FileToken);
            bytes.ShouldNotBeNull();
            AppFileHelper.SaveToFile(fileToExportDto.FileName, bytes);
        }
    }
}

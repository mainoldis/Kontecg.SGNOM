using Kontecg.Application.Services;
using Kontecg.Dto;
using Kontecg.Logging.Dto;

namespace Kontecg.Logging
{
    public interface ILogAppService : IApplicationService
    {
        GetLatestLogsOutput GetLatestLogs();

        FileDto DownloadLogs();
    }
}

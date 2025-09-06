using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Kontecg.Authorization;
using Kontecg.Dto;
using Kontecg.IO;
using Kontecg.Logging.Dto;
using Kontecg.MimeTypes;
using Kontecg.Storage;

namespace Kontecg.Logging
{
    [KontecgAuthorize(PermissionNames.AdministrationHostMaintenance)]
    public class LogAppService : KontecgAppServiceBase, ILogAppService
    {
        private readonly IContentFolders _appFolders;
        private readonly IMimeTypeMap _mimeTypeMap;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        public LogAppService(IMimeTypeMap mimeTypeMap, IContentFolders appFolders,
            ITempFileCacheManager tempFileCacheManager)
        {
            _mimeTypeMap = mimeTypeMap;
            _appFolders = appFolders;
            _tempFileCacheManager = tempFileCacheManager;
        }

        public GetLatestLogsOutput GetLatestLogs()
        {
            var directory = new DirectoryInfo(_appFolders.LogsFolder);
            if (!directory.Exists) return new GetLatestLogsOutput {LatestLogLines = new List<string>()};

            var lastLogFile = directory.GetFiles("*.txt", SearchOption.AllDirectories).MaxBy(f => f.LastWriteTime);

            if (lastLogFile == null) return new GetLatestLogsOutput();

            var lines = AppFileHelper.ReadLines(lastLogFile.FullName).Reverse().Take(KontecgCoreConsts.DefaultPageSize)
                .ToList();
            var logLineCount = 0;
            var lineCount = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("DEBUG") ||
                    line.StartsWith("INFO") ||
                    line.StartsWith("WARN") ||
                    line.StartsWith("ERROR") ||
                    line.StartsWith("FATAL"))
                    logLineCount++;

                lineCount++;

                if (logLineCount == 100) break;
            }

            return new GetLatestLogsOutput
            {
                LatestLogLines = lines.Take(lineCount).Reverse().ToList()
            };
        }

        public FileDto DownloadLogs()
        {
            //Create temporary copy of logs
            var logFiles = GetAllLogFiles();

            //Create the zip file
            var zipFileDto = new FileDto("Logs.zip", _mimeTypeMap.GetMimeType("Logs.zip"));

            using var outputZipFileStream = new MemoryStream();
            using (var zipStream = new ZipArchive(outputZipFileStream, ZipArchiveMode.Create))
            {
                foreach (var logFile in logFiles)
                {
                    var entry = zipStream.CreateEntry(logFile.Name);
                    using var entryStream = entry.Open();
                    using var fs = new FileStream(logFile.FullName, FileMode.Open, FileAccess.Read,
                        FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan);
                    fs.CopyTo(entryStream);
                    entryStream.Flush();
                }
            }

            _tempFileCacheManager.SetFile(zipFileDto.FileToken, outputZipFileStream.ToArray());

            return zipFileDto;
        }

        private List<FileInfo> GetAllLogFiles()
        {
            var directory = new DirectoryInfo(_appFolders.LogsFolder);
            return directory.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
        }
    }
}

using System.Threading.Tasks;
using Kontecg.Desktop;
using Velopack.Sources;
using Xunit;

namespace Kontecg.Tests.Web.Http
{
    public class HttpClient_Tests : DesktopTestModuleTestBase
    {
        public HttpClient_Tests()
        {
        }

        [Theory]
        [InlineData("https://www.google.com/", "google.html")]
        [InlineData("http://ecgmobile.ecg.moa.minbas.cu/updates/", "updates.html")]
        public async Task Try_to_download_file(string url, string targetFile)
        {
            int progress = 0;

            var downloader = LocalIocManager.Resolve<IFileDownloader>();
            await downloader.DownloadFile(url, targetFile, i =>
            {
                var p = i;
            });
        }
    }
}

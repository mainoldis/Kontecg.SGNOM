using Castle.Facilities.Logging;
using Kontecg.Castle.Logging.Log4Net;
using Kontecg.IO;
using Kontecg.PlugIns;
using Kontecg.Threading;
using Kontecg.Timing;

namespace Kontecg
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    _serviceProvider = services.AddKontecg<MainModule>(options =>
                    {
                        var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        DirectoryHelper.CreateIfNotExists(Path.GetFullPath(Path.Combine(localApplicationData, KontecgCoreConsts.DefaultReferenceGroup, KontecgCoreConsts.DefaultDataFolderName)));
                        DirectoryHelper.CreateIfNotExists(KontecgCoreConsts.LogsFolderName);
                        DirectoryHelper.CreateIfNotExists(KontecgCoreConsts.ExtensionsFolderName);

                        //Configure Log4Net logging
                        options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                            f => f.UseKontecgLog4Net().WithConfig("log4net.config")
                        );

                        options.PlugInSources.AddFolder(Path.GetFullPath(KontecgCoreConsts.ExtensionsFolderName), SearchOption.AllDirectories);

                        ThreadCultureSanitizer.Sanitize();
                        //Setting default clock provider
                        Clock.Provider = ClockProviders.Local;

                    }, removeConventionalInterceptors: false);

                    services.AddHostedService<Worker>();
                })
                .Build();

            using var kontecgBootstrapper = _serviceProvider.GetRequiredService<KontecgBootstrapper>();
            kontecgBootstrapper.Initialize();
            host.Run();
        }
    }
}

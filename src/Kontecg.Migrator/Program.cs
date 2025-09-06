using System;
using Castle.Facilities.Logging;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Threading;
using Kontecg.Timing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kontecg.Migrator
{
    internal static class Program
    {
        private static bool _skipConnVerification;

        private static void Main(string[] args)
        {
            ParseArgs(args);

            using var bootstrapper = KontecgBootstrapper.Create<KontecgMigratorModule>();
            bootstrapper.IocManager.IocContainer
                .AddFacility<LoggingFacility>(f => f.LogUsing<InternalConsoleLoggerFactory>());

            //Fix cultureInfo
            ThreadCultureSanitizer.Sanitize();
            //Setting default clock provider
            Clock.Provider = ClockProviders.Local;
            bootstrapper.Initialize();

            using (var migrateExecuter = bootstrapper.IocManager.ResolveAsDisposable<MultiCompanyMigrateExecuter>())
            {
                migrateExecuter.Object.Run(_skipConnVerification);
            }

            if (_skipConnVerification) return;

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }

        private static void ParseArgs(string[] args)
        {
            if (args.IsNullOrEmpty()) return;

            foreach (var arg in args)
                if (arg == "-s")
                    _skipConnVerification = true;
        }

#if DEBUG
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddKontecg<KontecgMigratorModule>(options =>
                {
                    ThreadCultureSanitizer.Sanitize();
                    //Setting default clock provider
                    Clock.Provider = ClockProviders.Local;

                }, removeConventionalInterceptors: false);
            });
#endif
    }
}

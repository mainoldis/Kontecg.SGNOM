using System;
using System.Threading.Tasks;
using Castle.Facilities.Logging;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.EFCore;
using Kontecg.PlugIns;
using Kontecg.Threading;
using Kontecg.Timing;

namespace Kontecg.SampleApp
{
    internal class Program
    {
        private static bool _skipConnVerification;
        private static bool _withSpecificLogin;
        private static int _optionNumber = 1;

        private static async Task Main(string[] args)
        {
            ParseArgs(args);

            using var bootstrapper = KontecgBootstrapper.Create<MainModule>();
            bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.LogUsing<InternalConsoleLoggerFactory>());
            bootstrapper.PlugInSources.AddTypeList(typeof(SGNOMCoreModule), typeof(SGNOMDataModule), typeof(SGNOMServicesModule), typeof(SGNOMPresentationModule));
            ThreadCultureSanitizer.Sanitize();
            Clock.Provider = ClockProviders.Local;
            bootstrapper.Initialize();

            var loginExecuter = bootstrapper.IocManager.Resolve<LoginExecuter>();
            var result = await loginExecuter.RunAsync(_withSpecificLogin);

            if(!result) return;

            switch (_optionNumber)
            {
                case 1:
                    using (var exporter = bootstrapper.IocManager.ResolveAsDisposable<CalendarOptionExecuter>())
                    {
                        await exporter.Object.RunAsync(_skipConnVerification);
                    }
                    break;
                case 2:
                    break;
            }

            if (_skipConnVerification) return;

            await Console.Out.WriteLineAsync("Press ENTER to exit...");
            await Console.In.ReadLineAsync();
        }

        private static void ParseArgs(string[] args)
        {
            if (args.IsNullOrEmpty()) return;

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-s":
                        _skipConnVerification = true;
                        break;
                    case "-l":
                        _withSpecificLogin = true;
                        break;
                    case "-o1":
                        _optionNumber = 1;
                        break;
                    case "-o2":
                        _optionNumber = 2;
                        break;
                }
            }
        }
    }
}
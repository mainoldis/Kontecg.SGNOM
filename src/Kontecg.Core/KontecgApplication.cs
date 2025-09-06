using Castle.Facilities.Logging;
using Kontecg.Modules;
using System.IO;
using System;
using Kontecg.Castle.Logging.Log4Net;
using Kontecg.Dependency;
using Kontecg.PlugIns;
using Kontecg.Threading;
using Kontecg.Timing;
using NMoneys;

namespace Kontecg
{
    public sealed class KontecgApplication<TStartupModule> where TStartupModule : KontecgModule
    {
        /// <summary>
        ///     Gets a reference to the <see cref="KontecgBootstrapper" /> instance.
        /// </summary>
        public static KontecgBootstrapper KontecgBootstrapper { get; } = KontecgBootstrapper.Create<TStartupModule>();

        public void Start(Action<IIocResolver> startupCallback = null, params Type[] plugins)
        {
            KontecgBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseKontecgLog4Net().WithConfig($"{GetAppPath()}log4net.config")
            );

            //Adding plugins folder to bootstrapper
            KontecgBootstrapper.PlugInSources.AddFolder($"{GetAppPath()}{KontecgCoreConsts.ExtensionsFolderName}", SearchOption.AllDirectories);
            if (plugins is {Length: > 0}) KontecgBootstrapper.PlugInSources.AddTypeList(plugins);

            //Fix cultureInfo
            ThreadCultureSanitizer.Sanitize();
            //Setting default clock provider
            Clock.Provider = ClockProviders.Local;
            Currency.InitializeAllCurrencies();
            KontecgBootstrapper.Initialize();

            startupCallback?.Invoke(KontecgBootstrapper.IocManager);
        }

        private static string GetAppPath()
        {
            string appPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            if (appPath != null && !appPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                appPath += Path.DirectorySeparatorChar;
            return appPath;
        }

        public void End()
        {
            KontecgBootstrapper.Dispose();
        }
    }
}
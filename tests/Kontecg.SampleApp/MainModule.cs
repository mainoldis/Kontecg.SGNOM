using System.Reflection;
using Kontecg.Baseline;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.DependencyInjection;
using Kontecg.EFCore;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Kontecg.Runtime;
using Kontecg.Timing;
using Microsoft.Extensions.Configuration;

namespace Kontecg.SampleApp
{
    [DependsOn(
        typeof(KontecgCoreModule),
        typeof(KontecgBaselineModule),
        typeof(KontecgDataModule),
        typeof(SGNOMCoreModule),
        typeof(SGNOMDataModule),
        typeof(SGNOMServicesModule),
        typeof(SGNOMPresentationModule))]
    public class MainModule : KontecgModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        /// <summary>
        ///     MainModule is a Kontecg Module that is used to configure and initialize the application.
        ///     It sets up the background jobs, the content folders, and the workflows.
        ///     It also enables updates and auditing if required. It also configures languages, views and skins.
        /// </summary>
        public MainModule()
        {
            _appConfiguration = AppConfigurations.Get(
                typeof(MainModule).GetAssembly().GetDirectoryPathOrNull(),
                addUserSecrets: true
            );
        }

        /// <inheritdoc />
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(KontecgCoreConsts.ConnectionStringName);
            Configuration.EventBus.UseDefaultEventBus = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.Updates.IsUpdateCheckEnabled = false;

            Configuration.Modules.UseBaseline().LanguageManagement.EnableDbLocalization();
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            WinFormsRuntimeContext.ServiceProvider = ServiceCollectionRegistrar.Register(IocManager);
            WinFormsRuntimeContext.Calendar = WorkCalendarTool.New();
        }

        /// <inheritdoc />
        public override void PostInitialize()
        {
        }
    }
}
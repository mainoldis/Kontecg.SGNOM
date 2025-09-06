using Kontecg.BackgroundJobs;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using System.Reflection;
using Kontecg.DependencyInjection;
using Kontecg.Application.Features;
using Kontecg.Currencies;
using Kontecg.EFCore;
using Kontecg.Features;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Updates;

namespace Kontecg
{
    [DependsOn(
        typeof(KontecgAppModule),
        typeof(KontecgDataModule))]
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
            Configuration.ExceptionHandling.SendDetailedExceptionsToSupport = true;
            Configuration.ExceptionHandling.PropagatedHandledExceptions = false;

            Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
            Configuration.Updates.IsUpdateCheckEnabled = false;
            Configuration.Modules.UseBaseline().LanguageManagement.EnableDbLocalization();
            //This goes here due an error getting database connection early on run
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
                Configuration.ReplaceService<IBackgroundJobManager, KontecgBackgroundJobManager>();
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }

        /// <inheritdoc />
        public override void PostInitialize()
        {
            IFeatureChecker featureChecker = IocManager.Resolve<IFeatureChecker>();

            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();

            if (featureChecker.IsEnabled(CoreFeatureNames.CurrencyExchangeRateFeature))
                workManager.Add(IocManager.Resolve<ExternalExchangeRateProviderWorker>() as IBackgroundWorker);

            if (Configuration.Updates.IsUpdateCheckEnabled)
                workManager.Add(IocManager.Resolve<UpdateCheckerWorker>() as IBackgroundWorker);
        }
    }
}

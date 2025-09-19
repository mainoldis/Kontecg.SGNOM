using System;
using System.Reflection;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Authorization.Users;
using Kontecg.Authorization.Users.Password;
#if !DEBUG
using Kontecg.Baseline.Configuration;
#endif
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Currencies;
using Kontecg.Dependency;
using Kontecg.DependencyInjection;
using Kontecg.Features;
using Kontecg.Hangfire.Configuration;
using Kontecg.Localization.Dictionaries.Xml;
using Kontecg.Localization.Dictionaries;
using Kontecg.MassTransit;
using Kontecg.Modules;
using Kontecg.RealTime;
using Kontecg.Reflection.Extensions;
using Kontecg.Runtime;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Timing;
using Kontecg.Updates;
using Kontecg.Views;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;

namespace Kontecg
{
    [DependsOn(
        typeof(KontecgWinFormsModule), typeof(KontecgMassTransitModule))]
    public class MainModule : KontecgModule
    {
        private readonly IConfigurationRoot _appConfiguration;
        private IBusControl _busControl;

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

            Configuration.BackgroundJobs.IsJobExecutionEnabled = _appConfiguration.GetValue<bool>("App:BackgroundJobs:IsJobExecutionEnabled"); ;
            Configuration.Updates.IsUpdateCheckEnabled = _appConfiguration.GetValue<bool>("App:Update:IsEnabled");

            Configuration.Modules.UseMassTransit().Options.Host = _appConfiguration.GetValue<string>("App:RabbitMq:Host");

            Configuration.Modules.UseCore(options =>
            {
                options.IgnoredRecurrentJobs = _appConfiguration.GetValue<bool>("App:BackgroundJobs:IgnoredRecurrentJobs");
            });

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource("Launcher",
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MainModule).GetAssembly(),
                        "Kontecg.Localization.Sources"
                    )
                )
            );
#if DEBUG
            Configuration.Modules.UseCore(options => options.EnableDbLocalization());
#else
            Configuration.Modules.UseBaseline().LanguageManagement.EnableDbLocalization();
#endif
            Configuration.Modules.UseWinForms().MainType = typeof(ForTestingPurpose);
            //This goes here due an error getting database connection early on run
            //if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            //    Configuration.ReplaceService<IBackgroundJobManager, KontecgBackgroundJobManager>();

            //Configuration.Caching.UseRedis(options =>
            //{
            //    options.ConnectionString = _appConfiguration["App:RedisCache:ConnectionString"];
            //    options.DatabaseId = _appConfiguration.GetValue<int>("App:RedisCache:DatabaseId");
            //});
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            WinFormsRuntimeContext.ServiceProvider = ServiceCollectionRegistrar.Register(IocManager);
            WinFormsRuntimeContext.Calendar = IocManager.ResolveAsDisposable<ITimeCalendarProvider>().Object.GetWorkTimeCalendar();
            _busControl = MassTransitRegistrar.RegisterUsingRabbitMq(IocManager);
        }

        /// <inheritdoc />
        public override void PostInitialize()
        {
            SetupBackgroundWorks();
            SetupWorkflowServer();
            RegisterClient();

            _busControl?.Start();
        }

        /// <inheritdoc />
        public override void Shutdown()
        {
            _busControl?.Stop();
        }

        private void SetupWorkflowServer()
        {

        }

        private void SetupBackgroundWorks()
        {
            var featureChecker = IocManager.Resolve<IFeatureChecker>();
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
                Configuration.BackgroundJobs.UseHangfire(options => { });

            if (featureChecker.IsEnabled(CoreFeatureNames.CurrencyExchangeRateFeature))
                workManager.Add(IocManager.Resolve<ExternalExchangeRateProviderWorker>());

            workManager.Add(IocManager.Resolve<PasswordExpirationBackgroundWorker>());
            workManager.Add(IocManager.Resolve<MakeInactiveUsersPassiveWorker>());

            if (Configuration.Updates.IsUpdateCheckEnabled)
                workManager.Add(IocManager.Resolve<UpdateCheckerWorker>());
        }

        private void RegisterClient()
        {
            try
            {
                using var clientManager = IocManager.ResolveAsDisposable<ClientManager>();
                clientManager.Object.Register();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex.InnerException ?? ex);
            }
        }
    }
}

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
using Kontecg.Localization;
using Kontecg.Modules;
using Kontecg.RealTime;
using Kontecg.Reflection.Extensions;
using Kontecg.Runtime;
using Kontecg.Runtime.Caching.Redis;
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
        typeof(KontecgWinFormsModule))]
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

            Configuration.BackgroundJobs.IsJobExecutionEnabled = _appConfiguration.GetValue<bool>("App:BackgroundJobs:IsJobExecutionEnabled"); ;
            Configuration.Updates.IsUpdateCheckEnabled = _appConfiguration.GetValue<bool>("App:Update:IsEnabled");

            Configuration.Modules.UseCore(options =>
            {
                options.IgnoredRecurrentJobs = _appConfiguration.GetValue<bool>("App:BackgroundJobs:IgnoredRecurrentJobs");
                options.MassTransitOptions.Host = _appConfiguration["App:RabbitMq:Host"];
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
        }

        /// <inheritdoc />
        public override void PostInitialize()
        {
            IFeatureChecker featureChecker = IocManager.Resolve<IFeatureChecker>();

            //SetupWorkflowServer();
            //StartMassTransit();

            if(Configuration.BackgroundJobs.IsJobExecutionEnabled) 
                Configuration.BackgroundJobs.UseHangfire(options => {});

            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();

            if(featureChecker.IsEnabled(CoreFeatureNames.CurrencyExchangeRateFeature))
                workManager.Add(IocManager.Resolve<ExternalExchangeRateProviderWorker>());

            workManager.Add(IocManager.Resolve<PasswordExpirationBackgroundWorker>());
            workManager.Add(IocManager.Resolve<MakeInactiveUsersPassiveWorker>());

            if (Configuration.Updates.IsUpdateCheckEnabled)
                workManager.Add(IocManager.Resolve<UpdateCheckerWorker>());

            RegisterClient();
        }

        private void SetupWorkflowServer()
        {

        }

        private void StartMassTransit()
        {
            var busControl = IocManager.Resolve<IBusControl>();

            RetryPolicy<BusHealthResult> retryPolicy = Policy
                .Handle<Exception>(exception =>
                {
                    Logger.Error(
                        $"Can't check for RabbitMQ endpoint, it seems there is a problem. Exception: {exception}");
                    return true;
                })
                .Or<TaskCanceledException>()
                .OrResult<BusHealthResult>(r => r.Status == BusHealthStatus.Healthy)
                .WaitAndRetry(5, retryCount => TimeSpan.FromMilliseconds(5000),
                    (result, timeSpan, retryCount, context) =>
                    {
                        Logger.Warn(
                            $"MassTransit service starting attempt {retryCount} failed, next attempt in {timeSpan.TotalMilliseconds} ms. Result: {result.Result.Status}");
                    });

            var healthResult = retryPolicy.Execute(() => busControl.CheckHealth());

            try
            {
                if (healthResult.Status != BusHealthStatus.Healthy)
                    busControl.Start();
            }
            catch (RabbitMqConnectionException e)
            {
                Logger.Error(e.Message, e.InnerException ?? e);
            }

            IocManager.Release(busControl);
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

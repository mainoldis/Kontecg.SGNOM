using System;
using System.IO;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Hardware.Info;
using Kontecg.Accounting;
using Kontecg.Auditing;
using Kontecg.Authorization.Delegation;
using Kontecg.Authorization.Ldap;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.AutoMapper;
using Kontecg.BackgroundJobs.Hangfire;
using Kontecg.Baseline;
using Kontecg.Baseline.Configuration;
using Kontecg.Baseline.Ldap;
using Kontecg.Baseline.Ldap.Configuration;
using Kontecg.BlobStoring;
using Kontecg.BlobStoring.FileSystem;
using Kontecg.Castle.Logging.MsLogging;
using Kontecg.Castle.MsAdapter;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Currencies;
using Kontecg.Debugging;
using Kontecg.Dependency;
using Kontecg.DynamicEntityProperties;
using Kontecg.EmbeddedResources;
using Kontecg.Features;
using Kontecg.Hangfire;
using Kontecg.Localization;
using Kontecg.MailKit;
using Kontecg.Modules;
using Kontecg.MultiCompany;
using Kontecg.Net.Mail;
using Kontecg.Net.Mail.Smtp;
using Kontecg.Notifications;
using Kontecg.RealTime;
using Kontecg.Reflection.Extensions;
using Kontecg.Runtime.Caching.Redis;
using Kontecg.Runtime.Session;
using Kontecg.Runtime.Validation.Interception;
using Kontecg.Storage.Blobs;
using Kontecg.Threading;
using Kontecg.Timing;
using Kontecg.Workflows;
using Kontecg.WorkRelations;
using MailKit.Security;
using Microsoft.Extensions.FileProviders;
using Microsoft.IO;

namespace Kontecg
{
    [DependsOn(
        typeof(KontecgCastleMsAdapterModule),
        typeof(KontecgCastleMsLoggingModule),
        typeof(KontecgAutoMapperModule),
        typeof(KontecgBaselineModule),
        typeof(KontecgLdapModule),
        typeof(KontecgBlobStoringFileSystemModule),
        typeof(KontecgMailKitModule),
        typeof(KontecgHangfireModule),
        typeof(KontecgWorkflowModule),
        typeof(KontecgRedisCacheModule))]
    public class KontecgCoreModule : KontecgModule
    {
        public override void PreInitialize()
        {
            //workaround for issue: https://github.com/aspnet/EntityFrameworkCore/issues/9825
            //related github issue: https://github.com/aspnet/EntityFrameworkCore/issues/10407
            AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Issue9825", true);

            AppContext.SetSwitch("Microsoft.Data.SqlClient.UseSystemDefaultSecureProtocols", true);

            IocManager.Register<IKontecgCoreConfiguration, KontecgCoreConfiguration>();
            IocManager.Register<ICancellationTokenProvider, DefaultCancellationTokenProvider>();
            IocManager.Register<ITimeCalendarProvider, DefaultWorkTimeCalendarProvider>();
            IocManager.Register<IHardwareInfo, HardwareInfo>();
            IocManager.Register<IExchangeRateFactory, DefaultExchangeRateFactory>();
            IocManager.Register<IOnlineClientInfoProvider, OnlineClientInfoProvider>();
            IocManager.Register<IAppConfigurationAccessor, DefaultAppConfigurationAccessor>();
            IocManager.Register<IAppConfigurationWriter, DefaultAppConfigurationWriter>();
            
            Configuration.ReplaceService<IKontecgSession, AppSession>();
            Configuration.ReplaceService<IClientInfoProvider, EnvironmentClientInfoProvider>();

            //Auditing configuration
            Configuration.Auditing.SaveReturnValues = DebugHelper.IsDebug;
            Configuration.Auditing.IsEnabledForAnonymousUsers = DebugHelper.IsDebug;

            //Adding file providers as EmbeddedResources
            IocManager.RegisterIfNot<IFileProvider, EmbeddedResourceImageFileProvider>();
            //Declare entity types
            Configuration.Modules.UseBaseline().EntityTypes.Role = typeof(Role);
            Configuration.Modules.UseBaseline().EntityTypes.User = typeof(User);
            Configuration.Modules.UseBaseline().EntityTypes.Company = typeof(Company);

            KontecgLocalizationConfigurer.Configure(Configuration.Localization);

            //Adding feature providers
            Configuration.Features.Providers.Add<CoreFeatureProvider>();

            //Adding setting providers
            Configuration.Settings.Providers.Add<AppSettingProvider>();
            //Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = KontecgCoreConsts.DefaultPassPhrase;

            //Adding notification providers
            Configuration.Notifications.Providers.Add<AppNotificationProvider>();

            //Enable this line to create a multi-company application.
            Configuration.MultiCompany.IsEnabled = KontecgCoreConsts.MultiCompanyEnabled;
            Configuration.MultiCompany.IgnoreFeatureCheckForHostUsers = true;
            Configuration.MultiCompany.Resolvers.Add<DomainCompanyResolveContributor>();

            //Enable LDAP authentication
            if (!DebugHelper.IsDebug) Configuration.Modules.UseLdap().Enable(typeof(LdapAuthenticationSource));

            //Adding DynamicEntityParameters definition providers
            Configuration.DynamicEntityProperties.Providers.Add<AppDynamicEntityPropertyDefinitionProvider>();

            // MailKit configuration
            Configuration.Modules.UseMailKit().SecureSocketOption = SecureSocketOptions.Auto;
            Configuration.ReplaceService<IMailKitSmtpBuilder, KontecgMailKitSmtpBuilder>(DependencyLifeStyle.Transient);

            //Configure roles
            AppRoleConfig.Configure(Configuration.Modules.UseBaseline().RoleManagement);

            if (DebugHelper.IsDebug)
                //Disabling email sending in debug mode
                Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);

            Configuration.ReplaceService(typeof(IEmailSenderConfiguration), () =>
            {
                Configuration.IocManager.IocContainer.Register(
                    Component.For<IEmailSenderConfiguration, ISmtpEmailSenderConfiguration>()
                        .ImplementedBy<KontecgSmtpEmailSenderConfiguration>()
                        .LifestyleTransient()
                );
            });

            Configuration.Notifications.Notifiers.Add<EmailRealTimeNotifier>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgCoreModule).GetAssembly());

            //Ensure our input classes are validated when used
            IocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        public override void PostInitialize()
        {
            RegisterMissingComponents();
            SetContentFolders();
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;

            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var v = asm.GetName().Version?.ToString();
            IocManager.Resolve<AppVersion>().Version = v;
        }

        private void SetContentFolders()
        {
            var contentFolders = IocManager.Resolve<IContentFolders>();
            contentFolders.ExtensionsFolder = Path.GetFullPath(KontecgCoreConsts.ExtensionsFolderName);

            var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var temp = Environment.GetEnvironmentVariable("TEMP");

            contentFolders.LogsFolder = Path.GetFullPath(KontecgCoreConsts.LogsFolderName);
            contentFolders.DataFolder = Path.GetFullPath(Path.Combine(localApplicationData, KontecgCoreConsts.DefaultReferenceGroup, KontecgCoreConsts.DefaultDataFolderName));
            contentFolders.TempFolder = Path.GetFullPath(temp);


            //Configure BLOB Storing
            Configuration.Modules.UseBlobStoring().Containers.ConfigureAll((name, option) =>
            {
                option.UseFileSystem(fileSystem => fileSystem.BasePath = contentFolders.DataFolder);
            });

            Configuration.Modules.UseBlobStoring().Containers.Configure<NassanContainer>(option => option.UseFileSystem(fileSystem =>
            {
                fileSystem.AppendContainerNameToBasePath = false;
                fileSystem.BasePath = Path.GetFullPath(@"\\nassan\Documentos\");
            }));
        }

        private void RegisterMissingComponents()
        {
            IocManager.RegisterIfNot<IPersonalAccountingInfoStore, SimplePersonalAccountingInfoStore>();
            IocManager.RegisterIfNot<IPersonalAccountingInfoProvider, DefaultPersonalAccountingInfoProvider>();
            IocManager.RegisterIfNot<IWorkRelationshipProvider, DefaultWorkRelationshipProvider>();
            IocManager.RegisterIfNot<IUserDelegationConfiguration, UserDelegationConfiguration>();
            IocManager.RegisterIfNot<RecyclableMemoryStreamManager>();
        }

        /// <summary>
        ///     Method called to allow us to perform some additional processing when a component is registered via IoC
        /// </summary>
        /// <param name="aKey"></param>
        /// <param name="aHandler"></param>
        private void Kernel_ComponentRegistered(string aKey, IHandler aHandler)
        {
            if (typeof(IHangfireParamsInputBase).GetTypeInfo().IsAssignableFrom(aHandler.ComponentModel.Implementation))
                //Add validation to the inputs for jobs
                aHandler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(ValidationInterceptor)));
        }
    }
}

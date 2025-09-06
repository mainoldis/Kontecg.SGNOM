using Castle.MicroKernel.Registration;
using Kontecg.Configuration;
using Kontecg.DependencyInjection;
using Kontecg.EFCore;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Kontecg.TestBase;
using Kontecg.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Kontecg.Baseline;
using Kontecg.Configuration.Startup;
using Kontecg.EntityHistory;
using Kontecg.Runtime.Session;

namespace Kontecg.SGNOM.Tests
{
    [DependsOn(
        typeof(KontecgCoreModule),
        typeof(KontecgBaselineModule),
        typeof(KontecgDataModule),
        typeof(SGNOMCoreModule),
        typeof(SGNOMDataModule),
        typeof(SGNOMServicesModule),
        typeof(SGNOMPresentationModule),
        typeof(KontecgTestBaseModule))]
    public class SGNOMTestModule : KontecgModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public SGNOMTestModule(SGNOMDataModule dataModule)
        {
            dataModule.SkipDbContextRegistration = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(SGNOMDataModule).GetAssembly().GetDirectoryPathOrNull(),
                addUserSecrets: true
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString =
                _appConfiguration.GetConnectionString(KontecgCoreConsts.ConnectionStringName);
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService<IEntityChangeSetReasonProvider, TestEntityChangeSetReasonProvider>();
            RegisterKontecgCoreDbContext();
            RegisterSGNOMDbContext();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SGNOMTestModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }

        public override void PostInitialize()
        {
            SetAppFolders();
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<IContentFolders>();
            appFolders.ExtensionsFolder = Path.GetFullPath(KontecgCoreConsts.ExtensionsFolderName);
            appFolders.LogsFolder = Path.GetFullPath(KontecgCoreConsts.LogsFolderName);
        }

        private void RegisterKontecgCoreDbContext()
        {
            var builder = new DbContextOptionsBuilder<KontecgCoreDbContext>();

            KontecgCoreDbContextConfigurer.Configure(builder,
                "Server=localhost; Database=Kontecg; User Id=sql; Password=sql;TrustServerCertificate=true;");

            if (!IocManager.IsRegistered<DbContextOptions<KontecgCoreDbContext>>())
            {
                IocManager.IocContainer.Register(
                    Component
                        .For<DbContextOptions<KontecgCoreDbContext>>()
                        .Instance(builder.Options)
                        .LifestyleSingleton()
                );
                new KontecgCoreDbContext(builder.Options).Database.EnsureCreated();
            }
        }

        private void RegisterSGNOMDbContext()
        {
            var builder = new DbContextOptionsBuilder<SGNOMDbContext>();

            SGNOMDbContextConfigurer.Configure(builder, "Server=localhost; Database=SGNOM; User Id=sql; Password=sql;TrustServerCertificate=true;");

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<SGNOMDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            new SGNOMDbContext(builder.Options).Database.EnsureCreated();
        }
    }
}

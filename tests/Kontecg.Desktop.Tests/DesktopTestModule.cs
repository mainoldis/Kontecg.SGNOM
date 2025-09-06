using System.IO;
using Castle.MicroKernel.Registration;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.DependencyInjection;
using Kontecg.Desktop.Views;
using Kontecg.EFCore;
using Kontecg.IO;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Kontecg.TestBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kontecg.Desktop
{
    [DependsOn(typeof(KontecgWinFormsModule), typeof(KontecgTestBaseModule))]
    public class DesktopTestModule : KontecgModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public DesktopTestModule(KontecgDataModule kontecgDataModule)
        {
            _appConfiguration = AppConfigurations.Get(
                typeof(DesktopTestModule).GetAssembly().GetDirectoryPathOrNull(),
                addUserSecrets: false
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString =
                _appConfiguration.GetConnectionString(KontecgCoreConsts.ConnectionStringName);
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            Configuration.Modules.UseBaseline().LanguageManagement.EnableDbLocalization();
            Configuration.Modules.UseWinForms().MainType = typeof(TestMain);
            Configuration.Modules.UseWinForms().Providers.Add<TestViewProvider>();

            RegisterKontecgCoreDbContext();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DesktopTestModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }

        public override void PostInitialize()
        {
            SetAppFolders();
            DirectoryHelper.CreateIfNotExists(KontecgCoreConsts.LogsFolderName);
            DirectoryHelper.CreateIfNotExists(KontecgCoreConsts.DefaultDataFolderName);
        }

        private void RegisterKontecgCoreDbContext()
        {
            var builder = new DbContextOptionsBuilder<KontecgCoreDbContext>();

            KontecgCoreDbContextConfigurer.Configure(builder, "Server=127.0.0.1; Database=Kontecg; User Id=sql; Password=sql;TrustServerCertificate=true;");

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

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<IContentFolders>();
            appFolders.ExtensionsFolder = Path.GetFullPath(KontecgCoreConsts.ExtensionsFolderName);
            appFolders.LogsFolder = Path.GetFullPath(KontecgCoreConsts.LogsFolderName);
            appFolders.DataFolder = Path.GetFullPath(KontecgCoreConsts.DefaultDataFolderName);
        }
    }
}

using Castle.MicroKernel.Registration;
using Kontecg.Configuration;
using Kontecg.EFCore;
using Kontecg.Events.Bus;
using Kontecg.Migrator.DependencyInjection;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Microsoft.Extensions.Configuration;

namespace Kontecg.Migrator
{
    [DependsOn(
        typeof(KontecgDataModule),
        typeof(SGNOMDataModule))]
    public class KontecgMigratorModule : KontecgModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public KontecgMigratorModule()
        {
            _appConfiguration = AppConfigurations.Get(
                typeof(KontecgMigratorModule).GetAssembly().GetDirectoryPathOrNull(),
                addUserSecrets: true
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString =
                _appConfiguration.GetConnectionString(KontecgCoreConsts.ConnectionStringName);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(typeof(IEventBus), () =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}

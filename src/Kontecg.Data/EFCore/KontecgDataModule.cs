using Kontecg.Baseline.EFCore;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Uow;
using Kontecg.EFCore.Configuration;
using Kontecg.EntityHistory;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg.EFCore
{
    [DependsOn(
        typeof(KontecgCoreModule),
        typeof(KontecgBaselineEFCoreModule))]
    public class KontecgDataModule : KontecgModule
    {
        /* Used it tests to skip DbContext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public override void PreInitialize()
        {
            Configuration.ReplaceService<IConnectionStringResolver, DbPerContextConnectionStringResolver>();

            if (!SkipDbContextRegistration)
                Configuration.Modules.UseEfCore().AddDbContext<KontecgCoreDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                        KontecgCoreDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    else
                        KontecgCoreDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                });
            
            // Set this setting to true for enabling entity history.
            Configuration.EntityHistory.IsEnabled = true;

            // Uncomment below line to write change logs for the entities below:
            Configuration.EntityHistory.Selectors.Add("KontecgBaselineEntities", EntityHistoryCoreHelper.TrackedTypes);
            Configuration.CustomConfigProviders.Add(new EntityHistoryConfigProvider(Configuration));

            // Set this to enable query compiler of Kontecg
            Configuration.Modules.UseEfCore().UseKontecgQueryCompiler = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgDataModule).GetAssembly());
        }
    }
}

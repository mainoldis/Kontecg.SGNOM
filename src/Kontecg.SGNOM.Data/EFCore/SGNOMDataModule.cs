using Kontecg.Baseline.EFCore;
using Kontecg.EFCore.Configuration;
using Kontecg.EntityHistory;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg.EFCore
{
    [DependsOn(
        typeof(SGNOMCoreModule),
        typeof(KontecgBaselineEFCoreModule))]
    public class SGNOMDataModule : KontecgModule
    {
        /* Used it tests to skip DbContext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.UseEfCore().AddDbContext<SGNOMDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                        SGNOMDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    else
                        SGNOMDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                });
            }
            // Uncomment below line to write change logs for the entities below:
            Configuration.EntityHistory.Selectors.Add(SGNOMEntityHistoryHelper.EntityHistoryConfigurationName, SGNOMEntityHistoryHelper.TrackedTypes);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SGNOMDataModule).GetAssembly());
        }
    }
}

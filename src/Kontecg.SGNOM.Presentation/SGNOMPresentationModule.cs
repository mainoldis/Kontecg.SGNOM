using Kontecg.Data;
using Kontecg.Modules;
using Kontecg.Navigation;
using Kontecg.Reflection.Extensions;
using Kontecg.Threading.BackgroundWorkers;

namespace Kontecg
{
    [DependsOn(
        typeof(KontecgWinFormsModule),
        typeof(SGNOMServicesModule))]
    public class SGNOMPresentationModule : KontecgModule
    {
        public override void PreInitialize()
        {
            Configuration.Navigation.Providers.Add<SGNOMModuleNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SGNOMPresentationModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<DataCollectorBackgroundWorker>());
        }
    }
}

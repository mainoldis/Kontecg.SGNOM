using Kontecg.EmbeddedResources;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg
{
    [DependsOn(typeof(KontecgKernelModule))]
    public class Skins : KontecgModule
    {
        public override void PreInitialize()
        {
            //Adding EmbeddedResources from current assembly
            KontecgEmbeddedResourcesConfigurer.Configure(Configuration.EmbeddedResources);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(Skins).GetAssembly());
        }
    }
}

using Kontecg.Reflection.Extensions;
using Kontecg.Resources.Embedded;

namespace Kontecg.EmbeddedResources
{
    internal static class KontecgWinformsResourcesConfigurer
    {
        public static void Configure(IEmbeddedResourcesConfiguration embeddedResourcesConfiguration)
        {
            embeddedResourcesConfiguration.Sources.AddRange(new EmbeddedResourceSet[]
                {
                    new("/", typeof(KontecgWinFormsModule).GetAssembly(), "Kontecg.Resources"),
                }
            );
        }
    }
}

using Kontecg.Reflection.Extensions;
using Kontecg.Resources.Embedded;

namespace Kontecg.EmbeddedResources
{
    internal static class KontecgEmbeddedResourcesConfigurer
    {
        public static void Configure(IEmbeddedResourcesConfiguration embeddedResourcesConfiguration)
        {
            embeddedResourcesConfiguration.Sources.AddRange(new EmbeddedResourceSet[]
                {
                    new("/", typeof(Skins).GetAssembly(), "Kontecg.Images"),
                }
            );
        }
    }
}

using Kontecg.Configuration.Startup;
using Kontecg.Localization.Dictionaries;
using Kontecg.Localization.Dictionaries.Xml;
using Kontecg.Reflection.Extensions;

namespace Kontecg.Localization
{
    public static class SGNOMLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    SGNOMConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(SGNOMLocalizationConfigurer).GetAssembly(),
                        "Kontecg.Localization.Sources"
                    )
                )
            );
        }
    }
}

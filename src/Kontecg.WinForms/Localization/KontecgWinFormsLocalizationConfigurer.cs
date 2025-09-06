using Kontecg.Configuration.Startup;
using Kontecg.Localization.Dictionaries.Xml;
using Kontecg.Localization.Dictionaries;
using Kontecg.Reflection.Extensions;

namespace Kontecg.Localization
{
    internal static class KontecgWinFormsLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    KontecgWinFormsConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(KontecgWinFormsLocalizationConfigurer).GetAssembly(),
                        "Kontecg.Localization.Sources"
                    )
                )
            );
        }
    }
}

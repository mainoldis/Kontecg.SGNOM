using Kontecg.Configuration.Startup;
using Kontecg.Localization.Dictionaries;
using Kontecg.Localization.Dictionaries.Xml;
using Kontecg.Reflection.Extensions;

namespace Kontecg.Localization
{
    internal static class KontecgLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    KontecgCoreConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(KontecgLocalizationConfigurer).GetAssembly(),
                        "Kontecg.Localization.Sources"
                    )
                )
            );

            localizationConfiguration.Languages.Add(new LanguageInfo("es", "Español (Spanish)", "es", true));
            localizationConfiguration.HumanizeTextIfNotFound = false;
        }
    }
}

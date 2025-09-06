#if DEBUG
using System.Collections.Generic;
using System.Globalization;
namespace Kontecg.Localization
{
    public class DebugLocalizationSource : MultiCompanyLocalizationSource
    {
        /// <inheritdoc />
        public DebugLocalizationSource(string name, MultiCompanyLocalizationDictionaryProvider dictionaryProvider) 
            : base(name, dictionaryProvider)
        {
        }

        /// <inheritdoc />
        protected override string ReturnGivenNameOrThrowException(string name, CultureInfo culture)
        {
            return DebugLocalizationHelper.ReturnGivenNameAndSaveToXml(
                LocalizationConfiguration,
                Name,
                name,
                culture
            );
        }

        /// <inheritdoc />
        protected override List<string> ReturnGivenNamesOrThrowException(List<string> names, CultureInfo culture)
        {
            return DebugLocalizationHelper.ReturnGivenNamesAndSaveToXml(
                LocalizationConfiguration,
                Name,
                names,
                culture
            );
        }
    }
}
#endif

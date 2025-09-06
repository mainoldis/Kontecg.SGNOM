using System;
using AutoMapper;

namespace Kontecg.Localization
{
    public class LocalizedEnumConverter : ITypeConverter<Enum, string>
    {
        private readonly ILocalizationManager _localizationManager;

        public LocalizedEnumConverter(ILocalizationManager localizationManager)
        {
            _localizationManager = localizationManager;
        }

        /// <inheritdoc />
        public string Convert(Enum source, string destination, ResolutionContext context)
        {
            if (source == null) return null;

            var resourceKey = $"{source.GetType().Name}.{source}";

            foreach (var localizationSource in _localizationManager.GetAllSources())
            {
                if (localizationSource.GetStringOrNull(resourceKey, false) != null)
                {
                    var localizedString = new LocalizableString(resourceKey, localizationSource.Name);
                    return localizedString.Localize(_localizationManager);
                }
            }
            
            return new LocalizableString(resourceKey, KontecgCoreConsts.LocalizationSourceName).Localize(_localizationManager);
        }
    }
}
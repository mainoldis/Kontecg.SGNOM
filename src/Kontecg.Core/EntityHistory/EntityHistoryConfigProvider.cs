using System.Collections.Generic;
using System.Linq;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;

namespace Kontecg.EntityHistory
{
    public class EntityHistoryConfigProvider : ICustomConfigProvider
    {
        private readonly IKontecgStartupConfiguration _kontecgStartupConfiguration;

        public EntityHistoryConfigProvider(IKontecgStartupConfiguration kontecgStartupConfiguration)
        {
            _kontecgStartupConfiguration = kontecgStartupConfiguration;
        }

        public Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext)
        {
            if (!_kontecgStartupConfiguration.EntityHistory.IsEnabled)
                return new Dictionary<string, object>
                {
                    {
                        EntityHistoryCoreHelper.EntityHistoryConfigurationName,
                        new EntityHistoryUiSetting
                        {
                            IsEnabled = false
                        }
                    }
                };

            var entityHistoryEnabledEntities = new List<string>();

            foreach (var type in EntityHistoryCoreHelper.TrackedTypes)
                if (_kontecgStartupConfiguration.EntityHistory.Selectors.Any(s => s.Predicate(type)))
                    entityHistoryEnabledEntities.Add(type.FullName);

            return new Dictionary<string, object>
            {
                {
                    EntityHistoryCoreHelper.EntityHistoryConfigurationName,
                    new EntityHistoryUiSetting
                    {
                        IsEnabled = true,
                        EnabledEntities = entityHistoryEnabledEntities
                    }
                }
            };
        }
    }
}

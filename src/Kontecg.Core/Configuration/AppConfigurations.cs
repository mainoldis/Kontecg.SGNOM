using System.Collections.Concurrent;
using Kontecg.Extensions;
using Kontecg.Reflection.Extensions;
using Microsoft.Extensions.Configuration;

namespace Kontecg.Configuration
{
    public static class AppConfigurations
    {
        private static readonly ConcurrentDictionary<string, IConfigurationRoot> ConfigurationCache;

        static AppConfigurations()
        {
            ConfigurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();
        }

        public static IConfigurationRoot Get(string path, string environmentName = null, bool addUserSecrets = false)
        {
            var cacheKey = path + "#" + environmentName + "#" + addUserSecrets;
            return ConfigurationCache.GetOrAdd(
                cacheKey,
                _ => BuildConfiguration(path, environmentName, addUserSecrets)
            );
        }

        private static IConfigurationRoot BuildConfiguration(string path, string environmentName = null,
            bool addUserSecrets = false)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("config.json", true, true);

            if (!environmentName.IsNullOrWhiteSpace())
                builder = builder.AddJsonFile($"config.{environmentName}.json", true);

            builder = builder.AddEnvironmentVariables();

            if (addUserSecrets) builder.AddUserSecrets(typeof(AppConfigurations).GetAssembly());

            return builder.Build();
        }
    }
}

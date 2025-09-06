using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Localization;
using Kontecg.Localization.Dictionaries;

namespace Kontecg.Configuration
{
    internal class KontecgCoreConfiguration : IKontecgCoreConfiguration
    {
        private readonly IKontecgStartupConfiguration _configuration;

        private readonly IIocManager _iocManager;

        public KontecgCoreConfiguration(IIocManager iocManager, IKontecgStartupConfiguration configuration)
        {
            _iocManager = iocManager;
            _configuration = configuration;

            Logger = NullLogger.Instance;

            IgnoredRecurrentJobs = true;

            IgnoredFileExtensions = new HashSet<string>
            {
                "config"
            };

            DomainFormat = "{0}.moa.minbas.cu;{0}.moa.minem.cu";

            MassTransitOptions = new MassTransitOptions();
        }

        public ILogger Logger { get; set; }

        public bool IgnoredRecurrentJobs {get; set;}

        /// <inheritdoc />
        public string DomainFormat { get; set; }

        /// <inheritdoc />
        public string ClientId { get; set; }

        /// <inheritdoc />
        public HashSet<string> IgnoredFileExtensions { get; }

        public MassTransitOptions MassTransitOptions { get; }
#if DEBUG
        public void EnableDbLocalization()
        {
            _iocManager.Register<ILanguageProvider, ApplicationLanguageProvider>(DependencyLifeStyle.Transient);

            List<IDictionaryBasedLocalizationSource> sources = _configuration
                                                               .Localization
                                                               .Sources
                                                               .Where(s => s is IDictionaryBasedLocalizationSource)
                                                               .Cast<IDictionaryBasedLocalizationSource>()
                                                               .ToList();

            foreach (IDictionaryBasedLocalizationSource source in sources)
            {
                _configuration.Localization.Sources.Remove(source);
                _configuration.Localization.Sources.Add(
                    new DebugLocalizationSource(
                        source.Name,
                        new MultiCompanyLocalizationDictionaryProvider(
                            source.DictionaryProvider,
                            _iocManager
                        )
                    )
                );

                Logger.DebugFormat("Converted {0} ({1}) to DebugLocalizationSource", source.Name,
                    source.GetType());
            }
        }
#endif
    }
}

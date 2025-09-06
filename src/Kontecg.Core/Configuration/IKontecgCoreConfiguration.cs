using System.Collections.Generic;

namespace Kontecg.Configuration
{
    public interface IKontecgCoreConfiguration
    {
        bool IgnoredRecurrentJobs { get; set; }

        /// <summary>
        ///     List of file extensions (without dot) to ignore for embedded resources.
        ///     Default extensions: config.
        /// </summary>
        HashSet<string> IgnoredFileExtensions { get; }

        string DomainFormat { get; set; }

        MassTransitOptions MassTransitOptions { get; }

#if DEBUG
        void EnableDbLocalization();
#endif
    }
}

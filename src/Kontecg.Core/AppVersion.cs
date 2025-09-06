using Kontecg.Dependency;

namespace Kontecg
{
    public class AppVersion : ISingletonDependency
    {
        /// <summary>
        ///     Gets the version of the application.
        /// </summary>
        public string Version { get; set; }
    }
}
using System;
using Kontecg.Dependency;

namespace Kontecg.Timing
{
    public class AppTimes : ISingletonDependency
    {
        /// <summary>
        ///     Gets the startup time of the application.
        /// </summary>
        public DateTime StartupTime { get; set; }
    }
}

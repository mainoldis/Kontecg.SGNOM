using System.IO;
using Microsoft.Extensions.Configuration;

namespace Kontecg.Configuration
{
    public class DefaultAppConfigurationAccessor : IAppConfigurationAccessor
    {
        public DefaultAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(Directory.GetCurrentDirectory());
        }

        public IConfigurationRoot Configuration { get; }
    }
}

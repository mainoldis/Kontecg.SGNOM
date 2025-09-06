using Microsoft.Extensions.Configuration;

namespace Kontecg.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}

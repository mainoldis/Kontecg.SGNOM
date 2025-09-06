using Kontecg.Auditing;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Environment_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public void Get_environment_info_Test()
        {
            var clientInfoProvider = LocalIocManager.Resolve<IClientInfoProvider>();

            var clientInfo = clientInfoProvider.ClientInfo;
            clientInfo.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Get_ip_address_Test()
        {
            var clientInfoProvider = LocalIocManager.Resolve<IClientInfoProvider>();

            var clientIpAddress = clientInfoProvider.ClientIpAddress;
            clientIpAddress.ShouldNotBeNullOrEmpty();
        }
    }
}

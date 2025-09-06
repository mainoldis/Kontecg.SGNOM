using Kontecg.Auditing;
using Kontecg.Runtime.Session;

namespace Kontecg.RealTime
{
    public class OnlineClientInfoProvider : IOnlineClientInfoProvider
    {
        private readonly IClientInfoProvider _clientInfoProvider;

        public OnlineClientInfoProvider(IClientInfoProvider clientInfoProvider)
        {
            _clientInfoProvider = clientInfoProvider;
            KontecgSession = NullKontecgSession.Instance;
        }

        public IOnlineClient CreateClientForCurrentConnection()
        {
            return new OnlineClient(
                _clientInfoProvider.ClientId,
                _clientInfoProvider.ClientIpAddress,
                KontecgSession.CompanyId,
                KontecgSession.UserId);
        }

        public IKontecgSession KontecgSession { get; set; }
    }
}

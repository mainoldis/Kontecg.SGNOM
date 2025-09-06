using Kontecg.Runtime;
using System.Threading;

namespace Kontecg.Threading
{
    internal class DefaultCancellationTokenProvider : CancellationTokenProviderBase
    {
        public DefaultCancellationTokenProvider(
            IAmbientScopeProvider<CancellationTokenOverride> cancellationTokenOverrideScopeProvider)
            : base(cancellationTokenOverrideScopeProvider)
        {
        }

        public override CancellationToken Token => OverridedValue?.CancellationToken ?? CancellationToken.None;
    }
}

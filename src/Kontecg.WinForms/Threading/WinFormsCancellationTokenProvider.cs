using System;
using Kontecg.Runtime;
using System.Threading;

namespace Kontecg.Threading
{
    public class WinFormsCancellationTokenProvider : CancellationTokenProviderBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(180);

        public WinFormsCancellationTokenProvider(
            IAmbientScopeProvider<CancellationTokenOverride> cancellationTokenOverrideScopeProvider)
            : base(cancellationTokenOverrideScopeProvider)
        {
            _cancellationTokenSource = new CancellationTokenSource(_timeout);
        }

        public override CancellationToken Token => OverridedValue?.CancellationToken ?? _cancellationTokenSource.Token;
    }
}

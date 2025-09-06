using Castle.Core.Logging;
using DevExpress.XtraSplashScreen;
using Kontecg.Dependency;
using Kontecg.Runtime.Session;
using Kontecg.Timing;

namespace Kontecg.Runtime.Events
{
    public class IdleStateHandler : IIdleStateHandler, ISingletonDependency
    {
        private readonly IWinFormsRuntime _winFormsRuntime;
        private IOverlaySplashScreenHandle _handle;

        public ILogger Logger { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        public IdleStateHandler(IWinFormsRuntime winFormsRuntime)
        {
            _winFormsRuntime = winFormsRuntime;
            Logger = NullLogger.Instance;
            KontecgSession = NullKontecgSession.Instance;
        }

        public void HandleEvent(IdleStateEntering eventData)
        {
            if (_winFormsRuntime.RunningContext && KontecgSession.UserId != null)
                _handle = SplashScreenManager.ShowOverlayForm(eventData.Owner, eventData.Options);

            Logger.Debug($"Idle elapsed time: {(Clock.Now - eventData.EventTime).TotalMilliseconds}ms");
        }
    }
}

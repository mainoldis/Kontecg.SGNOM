using Kontecg.Dependency;
using Kontecg.Extensions;
using Kontecg.Services;

namespace Kontecg.Runtime.Events
{
    public class StartupProcessHandler : IStartupProcessHandler, ITransientDependency
    {
        private readonly IWaitingViewService _waitingViewService;

        public StartupProcessHandler(IWaitingViewService waitingViewService)
        {
            _waitingViewService = waitingViewService;
        }

        public void HandleEvent(StartupProcessChanged eventData)
        {
            if (eventData.Status.IsNullOrEmpty()) return;
            _waitingViewService.ShowSplash(eventData.Status);
        }

        public void HandleEvent(StartupProcessCompleted eventData)
        {
            _waitingViewService.CloseSplash();
        }
    }
}

using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using System.Threading.Tasks;
using Kontecg.ExceptionHandling;
using Kontecg.Services;

namespace Kontecg.ViewModels.Shared
{
    public class TestViewModel : KontecgViewModelBase
    {
        private readonly SnapshotManager _snapshotManager;
        private readonly IWaitingViewService _waitingViewService;

        public TestViewModel(SnapshotManager snapshotManager, 
            IWaitingViewService waitingViewService)
        {
            _snapshotManager = snapshotManager;
            _waitingViewService = waitingViewService;
        }

        protected ICurrentWindowService CurrentWindowService =>
            // using the GetService<> extension method for obtaining service instance
            this.GetService<ICurrentWindowService>();

        public void Grab()
        {
            _waitingViewService.BeginWaiting(null, null);
            var tempFileInfo = _snapshotManager.GrabSnapshot();
            Snapshot = tempFileInfo.File;
            this.RaisePropertyChanged(x => x.Snapshot);
            _waitingViewService.EndWaiting();
        }

        public async Task ClickAsync()
        {
            var dispatcher = this.GetService<IDispatcherService>();
            var asyncCommand = this.GetAsyncCommand(x => x.ClickAsync());
            for (int i = 0; i <= 100; i++)
            {
                // cancellation check
                if (asyncCommand.IsCancellationRequested)
                    break;
                // do some work here
                await Task.Delay(25);
                await UpdateProgressOnUIThreadAsync(dispatcher, i);
            }
            await UpdateProgressOnUIThreadAsync(dispatcher, 0);
        }

        // Property for progress
        public int Progress
        {
            get;
            private set;
        }

        public object Snapshot { get; set; }

        private async Task UpdateProgressOnUIThreadAsync(IDispatcherService dispatcher, int progress)
        {
            await dispatcher.BeginInvoke(() => {
                Progress = progress;
                this.RaisePropertyChanged(x => x.Progress);
            });
        }
    }
}
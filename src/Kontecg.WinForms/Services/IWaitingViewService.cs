using System.Windows.Forms;

namespace Kontecg.Services
{
    public interface IWaitingViewService
    {
        void ShowSplash(string status);

        void CloseSplash();

        void BeginWaiting(IWin32Window owner, object parameter);

        void EndWaiting();
    }
}
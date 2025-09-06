namespace Kontecg.Services
{
    public interface IWaitingViewService
    {
        void ShowSplash(string status);

        void CloseSplash();

        void BeginWaiting(System.Windows.Forms.UserControl owner, object parameter);

        void EndWaiting();
    }
}
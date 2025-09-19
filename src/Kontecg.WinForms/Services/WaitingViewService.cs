using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using Kontecg.Dependency;
using Kontecg.Extensions;
using Kontecg.Runtime;
using Kontecg.Views;

namespace Kontecg.Services
{
    public class WaitingViewService : IWaitingViewService, ITransientDependency
    {
        private Form _owner;

        public WaitingViewService(IWinFormsRuntime winFormsRuntime)
        {
            _owner = winFormsRuntime.MainForm?.Target as Form;
            SplashScreenManager.ActivateParentOnWaitFormClosing = false;
        }


        /// <inheritdoc />
        public void ShowSplash(string status)
        {
            if (SplashScreenManager.Default == null)
                SplashScreenManager.ShowForm(_owner, typeof(KontecgSplashScreen), true, true);
            else
                SplashScreenManager.Default.SendCommand(KontecgSplashScreen.UpdateSplashCommand.Description, status);
        }

        /// <inheritdoc />
        public void CloseSplash()
        {
            SplashScreenManager.CloseForm(false, 1500, _owner);
        }

        /// <inheritdoc />
        public void BeginWaiting(IWin32Window owner, object parameter)
        {
            ShowWaitView(owner, DevExpress.XtraEditors.EnumDisplayTextHelper.GetDisplayText(parameter));
        }

        /// <inheritdoc />
        public void EndWaiting()
        {
            CloseWaitView();
        }

        private void ShowWaitView(IWin32Window owner = null, string caption = null, string description = null)
        {
            if (SplashScreenManager.Default != null) return;

            _owner = owner switch
                     {
                         Form form => form,
                         BaseUserControl userControl => (Form) userControl.GuessOwner(),
                         _ => _owner
                     };

            SplashScreenManager.ShowForm(_owner, typeof(KontecgWaitForm), false, false, false, ParentFormState.Unlocked);
            SplashScreenManager splashScreenManager = SplashScreenManager.Default;
            if (!caption.IsNullOrEmpty()) splashScreenManager?.SetWaitFormCaption(caption);
            if (description.IsNullOrEmpty()) return;
            splashScreenManager?.SetWaitFormDescription(description);
        }

        private void CloseWaitView()
        {
            var ssm = SplashScreenManager.Default;
            if (ssm is { ActiveSplashFormTypeInfo.Mode: Mode.WaitForm })
                SplashScreenManager.CloseForm(false, 750, _owner);
        }
    }
}
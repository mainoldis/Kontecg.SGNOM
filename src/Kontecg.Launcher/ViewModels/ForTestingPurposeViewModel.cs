using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Kontecg.Configuration;
using Kontecg.Features;
using System;
using System.Windows.Forms;
using Kontecg.Views;

namespace Kontecg.ViewModels
{
    public class ForTestingPurposeViewModel : KontecgViewModelBase
    {
        /// <inheritdoc />
        public ForTestingPurposeViewModel()
        {
        }

        private void RegisterLocalServices()
        {
            ISupportServices localServices = (ISupportServices)this;
        }

        public Form Owner { get; set; }

        [Command]
        public virtual void Exit()
        {
            RaiseTerminate(new FormClosingEventArgs(CloseReason.UserClosing, false));
        }

        public event EventHandler<FormClosingEventArgs> Terminate;

        protected virtual void OnTerminate(FormClosingEventArgs eventArgs)
        {
            RaiseTerminate(eventArgs);
        }

        protected virtual void RaiseTerminate(FormClosingEventArgs eventArgs)
        {
            if (eventArgs.Cancel) return;
            Terminate?.Invoke(this, eventArgs);
        }

        public object GetModuleCaption(object viewToAttach)
        {
            return viewToAttach is BaseUserControl uc ? uc.Text : "Vista Genérica";
        }

        #region Theme

        public virtual bool IsThemeFeatureEnabled => IsEnabled(WinFormsFeatureNames.ChangeThemeFeature) &&
                                                     SettingManager.GetSettingValue<bool>(WinFormsSettings.Theme.AllowChangeTheme);

        #endregion
    }
}
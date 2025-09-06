using DevExpress.Utils;
using Kontecg.Domain;
using Kontecg.Runtime;
using Kontecg.ViewModels;
using Kontecg.Views;

namespace Kontecg.Services.Forms
{
    public partial class DetailForm : BaseRibbonForm, IRibbonOwner
    {
        public DetailForm()
        {
            InitializeComponent();
            IconOptions.SvgImage = AppHelper.AppIcon;

            _mainViewModel = ((ISupportViewModel) AppHelper.MainForm).ViewModel as MainViewModel;
            if (_mainViewModel != null)
                BindCommands(_mainViewModel);
        }

        private readonly MainViewModel _mainViewModel;

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ScreenManager.SetFormToCurrentScreen(this);
            Bounds = PlacementHelper.Arrange(Size, Owner?.Bounds ?? ScreenManager.CurrentScreen.Bounds, System.Drawing.ContentAlignment.MiddleCenter);
        }

        protected override void OnShown(System.EventArgs e)
        {
            base.OnShown(e);
            if (ActiveForm != this)
                Activate();
        }

        private void BindCommands(MainViewModel viewModel)
        {
            biAbout.BindCommand(() => viewModel.ShowInfo(), viewModel);
        }

        protected override void OnControlAdded(System.Windows.Forms.ControlEventArgs e)
        {
            base.OnControlAdded(e);
            EnsureRibbonModule(e.Control);
        }

        private void EnsureRibbonModule(object view)
        {
            if (view is IRibbonOwner ribbonModule)
            {
                Ribbon.Pages[0].Text = ribbonModule.Ribbon.Pages[0].Text;
                Ribbon.MergeRibbon(ribbonModule.Ribbon);
                Ribbon.StatusBar.MergeStatusBar(ribbonModule.Ribbon.StatusBar);
                Text = string.Format("{1} - {0}", "KONTECG", ribbonModule.Ribbon.ApplicationDocumentCaption);
            }
        }

        #region IRibbonOwner

        DevExpress.XtraBars.Ribbon.RibbonControl IRibbonOwner.Ribbon => ribbonControl;

        #endregion
    }
}

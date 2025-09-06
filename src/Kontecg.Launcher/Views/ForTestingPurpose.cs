using DevExpress.Utils.MVVM;
using Kontecg.Behaviors;
using Kontecg.Runtime.Events;
using Kontecg.ViewModels;
using System;
using Kontecg.Views.HumanResources;
using System.Threading;
using DevExpress.Utils.DPI;
using DevExpress.XtraBars.Ribbon;
using Kontecg.Domain;
using System.Windows.Forms;
using DevExpress.Utils;
using Kontecg.Timing;
using System.ComponentModel;
using Kontecg.Dependency;

namespace Kontecg.Views
{
    public partial class ForTestingPurpose : BaseRibbonForm, ISupportViewModel
    {
        private int _loading = 0;
        private readonly AppTimes _appTimes;
        private readonly MVVMContext _context;
        private readonly IIocResolver _iocResolver;

        public ForTestingPurpose()
        {
            InitializeComponent();
        }

        public ForTestingPurpose(AppTimes appTimes, IIocResolver iocResolver)
        {
            InitializeComponent();
            _appTimes = appTimes;
            _iocResolver = iocResolver;
            _context = new MVVMContext(components);
            _context.ContainerControl = this;
            _context.ViewModelType = typeof(ForTestingPurposeViewModel);
            _context.AttachBehavior<FormCloseBehavior>(this);
            ViewModel.Owner = this;
            ViewModel.Terminate += ViewModel_OnTerminate;

            ribbonControl.Manager.HideBarsWhenMerging = false;
            ribbonControl.MinimizedChanged += Ribbon_MinimizedChanged;
            ribbonStatusBar.HideWhenMerging = DefaultBoolean.False;
            ribbonControl.ForceInitialize();
            
        }

        public ForTestingPurposeViewModel ViewModel => _context.GetViewModel<ForTestingPurposeViewModel>();

        object ISupportViewModel.ViewModel => ViewModel;

        /// <inheritdoc />
        void ISupportViewModel.ParentViewModelAttached() { }

        /// <inheritdoc />
        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode)
                return;

            _loading++;
            try
            {
                EventBus.Trigger(this, new StartupProcessChanged(L("PopulatingUI")));
                base.OnLoad(e);
                UpdateUserScreen();
            }
            catch (KontecgException ex)
            {
                Logger.Warn("An error was happened loading MainWindow", ex);
            }
            finally
            {
                EventBus.Trigger(this, new StartupProcessChanged(L("InitializationCompleted")));
                _loading--;
            }
        }

        /// <inheritdoc />
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            EventBus.Trigger(this, new StartupProcessCompleted(Clock.Now - _appTimes.StartupTime));
        }

        /// <inheritdoc />
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_loading > 0) e.Cancel = true;
            base.OnClosing(e);
        }

        private void ViewModel_OnTerminate(object sender, FormClosingEventArgs e)
        {
            
        }

        /// <inheritdoc />
        protected override void OnClosed(EventArgs e)
        {
            ViewModel.Terminate -= ViewModel_OnTerminate;

            Thread.CurrentPrincipal = null;
            base.OnClosed(e);
        }

        private void UpdateUserScreen()
        {

            if (!DpiAwarenessHelper.Default.IsPerMonitor())
                SuspendLayout();

            var viewToAttach = _iocResolver.Resolve<PersonsView>();
            viewToAttach.Dock = DockStyle.Fill;
            viewToAttach.Parent = this;
            ViewModelHelper.EnsureModuleViewModel(viewToAttach, ViewModel, null);

            if (!DpiAwarenessHelper.Default.IsPerMonitor())
                ResumeLayout();

            Text = $@"KONTECG - {ViewModel.GetModuleCaption(viewToAttach)}";

            if (viewToAttach is IRibbonOwner ribbonModuleControl)
            {
                Ribbon.MergeRibbon(ribbonModuleControl.Ribbon);
                Ribbon.StatusBar.MergeStatusBar(ribbonModuleControl.Ribbon.StatusBar);

                if (Ribbon.MergedPages.Count > 0)
                {
                    RibbonPage pageByText = Ribbon.TotalPageCategory.GetPageByText(ribbonModuleControl.Ribbon.SelectedPage.Text);
                    if (pageByText != null)
                        Ribbon.SelectedPage = pageByText;
                }
            }
            else
            {
                Ribbon.UnMergeRibbon();
                Ribbon.StatusBar.UnMergeStatusBar();
            }
        }

        private void Ribbon_MinimizedChanged(object sender, EventArgs e)
        {
        }
    }
}
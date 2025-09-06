using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Castle.Core.Logging;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using Kontecg.Events.Bus;
using Kontecg.Extensions;
using Kontecg.Localization.Sources;
using Kontecg.Localization;
using System.ComponentModel;
using DevExpress.Utils.MVVM;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashForm;
using Kontecg.Domain;
using Kontecg.ViewModels;
using Kontecg.Services;

namespace Kontecg.Views
{
    public class BaseUserControl : XtraUserControl, ISupportViewModel, IMustHaveContext
    {
        protected MVVMContext MvvmContext;

        private const string TitleId = "_Title";

        private ILocalizationSource _localizationSource;

        #region Component Designer generated code

        protected BaseUserControl()
        {
            InitializeComponent();

            if (DesignMode) return;
            LocalizationSourceName = KontecgWinFormsConsts.LocalizationSourceName;
            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MvvmContext = new DevExpress.Utils.MVVM.MVVMContext(this.components);
            this.SuspendLayout();
            this.MvvmContext.ContainerControl = this;
            this.Name = "BaseModuleControl";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ResumeLayout(false);
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseMvvmContext();
                OnDisposing();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region ViewModel injection

        protected BaseUserControl(Type viewModelType, object viewModel)
            :this()
        {
            this.MvvmContext.SetViewModel(viewModelType, viewModel);
            this.BindingContext = new BindingContext();
            OnInitServices();
        }

        protected BaseUserControl(Type viewModelType)
            : this()
        {
            this.MvvmContext.ViewModelType = viewModelType;
            this.BindingContext = new BindingContext();
            OnInitServices();
        }

        protected BaseUserControl(Type viewModelType, object[] viewModelConstructorParameters)
            : this()
        {
            this.MvvmContext.ViewModelType = viewModelType;
            this.MvvmContext.ViewModelConstructorParameters = viewModelConstructorParameters;
            this.BindingContext = new BindingContext();
            OnInitServices();
        }

        #endregion

        /// <summary>
        ///     Guess owner for this user control.
        /// </summary>
        protected internal IWin32Window GuessOwner()
        {
            var form = Form.ActiveForm;
            if (form?.InvokeRequired != false)
                return null;
            if (form is SplashFormBase && form.Owner != null)
                return form.Owner;
            while (form is {Owner: not null, ShowInTaskbar: false, TopMost: false} &&
                   form.Owner.Location != ControlHelper.InvalidPoint)
            {
                form = form.Owner;
            }
            return form;
        }

        /// <inheritdoc />
        public override Color BackColor
        {
            get => GetBackGroundColor();
            set => base.BackColor = value;
        }

        public BackstageViewControl BackstageView => Parent?.Parent as BackstageViewControl;

        /// <inheritdoc />
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (BackstageView != null)
                BackstageViewPainter.DrawBackstageViewImage(e, this, BackstageView);
        }

        private Color GetBackGroundColor()
        {
            var parent = Parent as BackstageViewClientControl;
            return parent?.GetBackgroundColor() ?? Color.Transparent;
        }


        /// <inheritdoc />
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <inheritdoc />
        protected override void OnFirstLoad()
        {
            if (!IsAncestorSiteInDesignMode)
            {
                SuspendLayout();
                LocalizeIsolatedItems();
                ResumeLayout();
            }
        }

        #region Localization

        protected virtual void LocalizeIsolatedItems()
        {
            Text = !Text.IsNullOrEmpty() ? $@"{L(Text.Replace(' ', '_') + TitleId)}" : $@"{L(Name + TitleId)}";
        }

        /// <summary>
        ///     Gets/sets name of the localization source that is used in this application service.
        ///     It must be set in order to use <see cref="L(string)" /> and <see cref="L(string,CultureInfo)" /> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        ///     Gets localization source.
        ///     It's valid if <see cref="LocalizationSourceName" /> is set.
        /// </summary>
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (DesignMode)
                    LocalizationSourceName = KontecgWinFormsConsts.LocalizationSourceName;

                if (LocalizationSourceName == null && !DesignMode)
                    throw new KontecgException(
                    "Must set LocalizationSourceName before, in order to get LocalizationSource");

                if (_localizationSource == null || _localizationSource.Name != LocalizationSourceName)
                    _localizationSource = LocalizationManager.GetSource(LocalizationSourceName);

                return _localizationSource;
            }
        }

        /// <summary>
        ///     Reference to the localization manager.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ILocalizationManager LocalizationManager { protected get; set; }

        /// <summary>
        ///     Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationSource?.GetString(name);
        }

        /// <summary>
        ///     Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, params object[] args)
        {
            return LocalizationSource?.GetString(name, args);
        }

        /// <summary>
        ///     Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationSource?.GetString(name, culture);
        }

        /// <summary>
        ///     Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource?.GetString(name, culture, args);
        }

        #endregion

        /// <summary>
        ///     Reference to the logger to write logs.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ILogger Logger { get; set; }

        /// <summary>
        ///     Reference to the event bus.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public IEventBus EventBus { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public IModuleLocator ModuleLocator { get; set; }

        protected virtual void OnInitServices() { }

        protected virtual void OnDisposing() { }

        protected virtual void OnMVVMContextReleasing() { }

        protected void ReleaseModule()
        {
            ModuleLocator?.ReleaseModuleControl(this);
        }

        /// <inheritdoc />
        protected override void OnHandleDestroyed(EventArgs e)
        {
            ReleaseMvvmContext();
            base.OnHandleDestroyed(e);
        }

        private void ReleaseMvvmContext()
        {
            if (MvvmContext.IsViewModelCreated)
            {
                ReleaseModule();
                OnMVVMContextReleasing();
                MvvmContext.Dispose();
            }
        }

        void ISupportViewModel.ParentViewModelAttached()
        {
            OnParentViewModelAttached();
        }

        protected virtual void OnParentViewModelAttached() { }

        /// <inheritdoc />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        object ISupportViewModel.ViewModel => Context.GetViewModel<object>();

        protected TViewModel GetViewModel<TViewModel>()
        {
            return Context.GetViewModel<TViewModel>();
        }
        protected TViewModel GetParentViewModel<TViewModel>()
        {
            return Context.GetParentViewModel<TViewModel>();
        }

        /// <inheritdoc />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public MVVMContext Context => MvvmContext;
    }
}

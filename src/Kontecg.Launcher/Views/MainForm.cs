using DevExpress.Utils.MVVM;
using Kontecg.Timing;
using System;
using System.Drawing;
using Kontecg.Runtime.Events;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Animation;
using DevExpress.Utils.DPI;
using DevExpress.XtraBars.Navigation;
using Kontecg.Domain;
using Kontecg.ViewModels;
using DevExpress.XtraBars.Ribbon;
using Kontecg.Presenters;
using Control = System.Windows.Forms.Control;
using DevExpress.XtraBars;
using Kontecg.Navigation;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.ToastNotifications;
using Kontecg.Runtime;
using System.Threading;
using DevExpress.Utils.VisualEffects;
using Kontecg.Behaviors;

namespace Kontecg.Views
{
    public partial class MainForm : BaseRibbonForm, ISupportViewModel, IPeekViewHost, ISupportLayout, ISupportTransitions
    {
        private int _loading = 0;
        private readonly OutlookReadingModeBehavior _outlookBehavior;
        private readonly ZoomLevelManager _zoomLevelManager;
        private readonly DockPanelManager _dockPanelManager;
        private readonly MVVMContext _context;
        private readonly AppTimes _appTimes;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(
            AppTimes appTimes,
            ZoomLevelManager zoomLevelManager,
            DockPanelManager dockPanelManager)
        {
            InitializeComponent();
            _context = new MVVMContext(components);
            _context.ContainerControl = this;
            _context.ViewModelType = typeof(MainViewModel);

            _context.AttachBehavior<FormCloseBehavior>(this);

            ViewModel.Owner = this;
            ViewModel.ModuleControlAdded += ViewModel_ViewAdded;
            ViewModel.ModuleControlRemoved += ViewModel_ViewRemoved;
            ViewModel.SelectedModuleTypeChanged += ViewModel_SelectedModuleTypeChanged;
            ViewModel.Print += ViewModel_Print;
            ViewModel.ShowAllFolders += ViewModel_ShowAllFolders;
            ViewModel.IsReadingModeChanged += ViewModel_IsReadingModeChanged;
            ViewModel.IsShowingGuidesChanged += ViewModel_OnIsShowingGuidesChanged;
            ViewModel.Terminate += ViewModel_OnTerminate;

            _appTimes = appTimes;

            _zoomLevelManager = zoomLevelManager;
            _zoomLevelManager.Initialize(beZoomLevel, bbiZoomDialog, ViewModel);
            _dockPanelManager = dockPanelManager;
            _dockPanelManager.Initialize(dockManager, ViewModel);

            ribbonControl.Manager.HideBarsWhenMerging = false;
            ribbonControl.MinimizedChanged += Ribbon_MinimizedChanged;
            ribbonStatusBar.HideWhenMerging = DefaultBoolean.False;
            
            ribbonControl.ForceInitialize();

            _outlookBehavior = new OutlookReadingModeBehavior(navBar, officeNavigationBar);
            officeNavigationBar.SynchronizeNavigationClientSelectedItem += OfficeNavigationBar_SynchronizeNavigationClientSelectedItem;
            officeNavigationBar.QueryPeekFormContent += OfficeNavigationBar_QueryPeekFormContent;
            officeNavigationBar.PopupMenuShowing += OfficeNavigationBar_PopupMenuShowing;
            navBar.ActiveGroupChanged += NavBar_ActiveGroupChanged;

            backstageViewControl.SelectedTabChanged += BackstageViewControl_SelectedTabChanged;
            backstageViewControl.Shown += BackstageViewControl_Shown;
            backstageViewControl.Hidden += BackstageViewControl_Hidden;
            backstageViewControl.Office2013StyleOptions.HeaderBackColor = DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
            backstageViewControl.BackstageViewShowRibbonItems = BackstageViewShowRibbonItems.None;

            adorneruiManager.QueryGuideFlyoutControl += AdorneruiManager_OnQueryGuideFlyoutControl;

            BindsCommands();
            BindFiltersVisibility();
            InitNotifications();

            LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
        }

        private void BindsCommands()
        {
            var bindings = _context.OfType<MainViewModel>();

            bindings.BindCommand(biInformacion, x => x.ShowInfo());
            bindings.BindCommand(bbiNormal, x => x.TurnOffReadingMode());
            bindings.BindCommand(bbiReading, x => x.TurnOnReadingMode());

            bindings.BindCommand(biBackstageViewAbout, x => x.ShowInfo());

            bindings.SetBinding(adorneruiManager, a => a.ShowGuides, x => x.IsShowingGuides,
                b => b ? DefaultBoolean.True : DefaultBoolean.False,
                b => b is DefaultBoolean.True);

            bindings.KeyToCommand(this, Keys.F1, x => x.ToggleGuides());

            _context.SetBinding<BarStaticItem, string, MainViewModel, string>(bsiUserInfo, e => e.Caption, x => x.UserInformation);
            

            var btnDockThePeek = (PeekFormButton)officeNavigationBar.OptionsPeekFormButtonPanel.Buttons[0];
            btnDockThePeek.BindCommand(() => ViewModel.DockPeekModule(null), ViewModel, GetActivePeekModule);
        }

        private void BindFiltersVisibility()
        {
            navBar.NavPaneStateChanged += NavBar_NavPaneStateChanged;
            ViewModel.FiltersVisibilityChanged += ViewModel_FiltersVisibilityChanged;
            bmiFolderNormal.BindCommand(() => ViewModel.ShowFilters(), ViewModel);
            bmiFolderMinimized.BindCommand(() => ViewModel.MinimizeFilters(), ViewModel);
            bmiFolderOff.BindCommand(() => ViewModel.HideFilters(), ViewModel);
        }

        public MainViewModel ViewModel => _context.GetViewModel<MainViewModel>();

        object ISupportViewModel.ViewModel => ViewModel;

        /// <inheritdoc />
        void ISupportViewModel.ParentViewModelAttached()
        {
        }

        /// <inheritdoc />
        protected override void OnLoad(EventArgs e)
        {
            if(DesignMode)
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

        private void UpdateUserScreen()
        {
            _dockPanelManager.RegisterPeekPanels();
            var modules = ViewModel.TopNavigation;
            ViewModel.SelectedModuleType = modules.FirstOrDefault();
            barNavigationItem.ClearLinks();
            navBar.Groups.Clear();
            navBar.Items.Clear();
            barSecurityItem.ClearLinks();

            RegisterNavigationModules(barNavigationItem, modules);
            RegisterNavPanes(navBar, modules);
            RegisterSecurityItem();
            RegisterBackstageViews();
        }

        private void RegisterSecurityItem()
        {
            //RULE: Terminar de completar la información del profile incluyendo un menú contextual para el acceso a manipular la cuenta de usuario
            var profileInfo = ViewModel.ProfilePicture;
            var loginInfo = ViewModel.LoginInformation;
            BarStaticItem bsiProfile = new BarHeaderItem();
            bsiProfile.Caption = $@"{loginInfo.Company?.CompanyName} {loginInfo.User.Name} {loginInfo.User.Surname} ({loginInfo.User.EmailAddress})";
            bsiProfile.AllowHtmlText = DefaultBoolean.True;

            BarButtonItem bbiLogout = new BarButtonItem();
            bbiLogout.Caption = L("Logout");
            bbiLogout.Name = "biLogout";
            bbiLogout.AllowGlyphSkinning = DefaultBoolean.True;
            bbiLogout.ImageOptions.ImageUri.Uri = "Close";
            bbiLogout.ImageOptions.ImageUri.ResourceType = typeof(MainForm);
            bbiLogout.BindCommand(() => ViewModel.Logout(), ViewModel);

            barSecurityItem.AddItem(bsiProfile);
            barSecurityItem.AddItem(bbiLogout);

            rpgApariencia.Visible = ViewModel.IsThemeFeatureEnabled;
        }

        /// <inheritdoc />
        protected override void LocalizeIsolatedItems()
        {
            rpInicio.Text = L(rpInicio.Text);
            rpVista.Text = L(rpVista.Text);
            rpgModulos.Text = L(rpgModulos.Text);
            rpgDiseño.Text = L(rpgDiseño.Text);
            rpgApariencia.Text = L(rpgApariencia.Text);
            rpgInformacion.Text = L(rpgInformacion.Text);

            barNavigationItem.Caption = L(barNavigationItem.Caption);
            barSecurityItem.Caption = L(barSecurityItem.Caption);
            biInformacion.Caption = L(biInformacion.Caption);
            biFolderPaneSubItem.Caption = L(biFolderPaneSubItem.Caption);
            bmiFolderNormal.Caption = L(bmiFolderNormal.Caption);
            bmiFolderMinimized.Caption = L(bmiFolderMinimized.Caption);
            bmiFolderOff.Caption = L(bmiFolderOff.Caption);

            bbiNormal.Caption = L(bbiNormal.Caption);
            bbiReading.Caption = L(bbiReading.Caption);
            navBar.Text = L(navBar.Text);
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

        /// <inheritdoc />
        protected override void OnClosed(EventArgs e)
        {
            ViewModel.ModuleControlAdded -= ViewModel_ViewAdded;
            ViewModel.ModuleControlRemoved -= ViewModel_ViewRemoved;
            ViewModel.SelectedModuleTypeChanged -= ViewModel_SelectedModuleTypeChanged;
            ViewModel.Print -= ViewModel_Print;
            ViewModel.ShowAllFolders -= ViewModel_ShowAllFolders;
            ViewModel.IsReadingModeChanged -= ViewModel_IsReadingModeChanged;
            ViewModel.Terminate -= ViewModel_OnTerminate;

            Thread.CurrentPrincipal = null;
            ViewModel.SelectedModuleType = null;
            base.OnClosed(e);
        }

        private void RegisterNavigationModules(BarLinkContainerItem menuItem, Module[] navigationItems)
        {
            var orderedMenuItems = navigationItems.OrderByCustom().ToArray();
            for (int i = 0; i < orderedMenuItems.Length; i++) RegisterNavigationModule(menuItem, orderedMenuItems[i]);
        }

        private void RegisterNavigationModule(BarLinkContainerItem menuItem, Module navigationItem)
        {
            if (navigationItem.SubModules.Count <= 0)
            {
                BarCheckItem biModule = new BarCheckItem();
                biModule.Caption = navigationItem.DisplayName;
                biModule.Name = "biModule" + navigationItem.Name;
                biModule.ImageOptions.ImageUri = navigationItem.SmallImageUri;
                biModule.AllowGlyphSkinning = DefaultBoolean.True;
                biModule.ImageUri.ResourceType = navigationItem.ResourceType ?? typeof(MainForm);
                biModule.GroupIndex = 1;
                biModule.Checked = ViewModel.SelectedModuleType.Name == navigationItem.Name;
                biModule.BindCommand((t) => ViewModel.SelectModule(t), ViewModel, () => navigationItem);
                menuItem.AddItem(biModule);
            }
            else
            {
                BarSubItem biModule = new BarSubItem();
                biModule.Caption = navigationItem.DisplayName;
                biModule.Name = "biModule" + navigationItem.Name;
                biModule.ImageOptions.ImageUri = navigationItem.SmallImageUri;
                biModule.AllowGlyphSkinning = DefaultBoolean.True;
                biModule.ImageUri.ResourceType = navigationItem.ResourceType ?? typeof(MainForm);
                menuItem.AddItem(biModule);

                var orderedMenuItems = navigationItem.SubModules.OrderByCustom().ToArray();
                for (int i = 0; i < orderedMenuItems.Length; i++)
                    RegisterNavigationModule(biModule, orderedMenuItems[i]);
            }
        }

        private void RegisterNavPanes(NavBarControl navBarControl, Module[] navigationItems)
        {
            var orderedMenuItems = navigationItems.OrderByCustom().ToArray();
            for (int i = 0; i < orderedMenuItems.Length; i++) 
                RegisterNavPane(navBarControl, ViewModel.GetNavPaneViewType(orderedMenuItems[i]));
            officeNavigationBar.RegisterItem += OfficeNavigationBar_RegisterItem;
            officeNavigationBar.NavigationClient = navBar;
        }

        private void RegisterNavPane(NavBarControl navBarControl, Module navigationItem)
        {
            NavBarGroup navGroup = new NavBarGroup();
            navGroup.Tag = navigationItem;
            navGroup.Name = "navGroup" + navigationItem.Name;
            navGroup.Caption = ViewModel.GetModuleCaption(navigationItem);
            navGroup.ImageUri = ViewModel.GetModuleImageUri(navigationItem, true);
            navGroup.ImageUri.ResourceType = ViewModel.GetModuleResourceType(navigationItem) ?? typeof(MainForm);
            navGroup.GroupStyle = NavBarGroupStyle.ControlContainer;
            navGroup.ControlContainer = new NavBarGroupControlContainer();
            navBarControl.Controls.Add(navGroup.ControlContainer);
            navBarControl.Groups.Add(navGroup);
        }

        private void ViewModel_ViewAdded(object sender, EventArgs e)
        {
            if (sender is Control moduleControl)
            {
                if (!DpiAwarenessHelper.Default.IsPerMonitor())
                    modulesContainer.SuspendLayout();

                moduleControl.Dock = DockStyle.Fill;
                moduleControl.Parent = modulesContainer;
                navBar.SendToBack();

                if (!DpiAwarenessHelper.Default.IsPerMonitor())
                    modulesContainer.ResumeLayout();

                Text = $@"KONTECG - {ViewModel.GetModuleCaption(ViewModel.SelectedModuleType)}";

                if (moduleControl is IRibbonOwner ribbonModuleControl)
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
        }

        private void ViewModel_ViewRemoved(object sender, EventArgs e)
        {
            if (sender is Control moduleControl)
            {
                GridHelper.HideCustomization(moduleControl);
                moduleControl.Parent = null;
            }
        }

        private void ViewModel_SelectedModuleTypeChanged(object sender, EventArgs e)
        {
            if (ViewModel.SelectedNavPaneModuleType != null)
                navBar.ActiveGroup = GetNavBarGroup(ViewModel.SelectedNavPaneModuleType);
            UpdateCompactLayout(!ribbonControl.Minimized);
        }

        private void ViewModel_Print(object sender, PrintEventArgs e)
        {
            backstageViewControl.SelectedTab = tabBackstageViewPrint;
            ribbonControl.ShowApplicationButtonContentControl();
        }

        private void ViewModel_ShowAllFolders(object sender, EventArgs e)
        {
            navBar.ShowNavPaneForm();
        }

        private void ViewModel_IsReadingModeChanged(object sender, EventArgs e)
        {
            _outlookBehavior.ReadingMode = ViewModel.IsReadingMode;
        }

        private void ViewModel_FiltersVisibilityChanged(object sender, EventArgs e)
        {
            switch (ViewModel.FiltersVisibility)
            {
                case CollectionViewFiltersVisibility.Visible:
                    navBar.OptionsNavPane.NavPaneState = NavPaneState.Expanded;
                    navBar.Visible = true;
                    break;
                case CollectionViewFiltersVisibility.Minimized:
                    navBar.OptionsNavPane.NavPaneState = NavPaneState.Collapsed;
                    navBar.Visible = true;
                    break;
                case CollectionViewFiltersVisibility.Hidden:
                    navBar.Visible = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ViewModel_OnTerminate(object sender, FormClosingEventArgs e)
        {
            if(e.Cancel) return;
            if (e.CloseReason == CloseReason.UserClosing) Close();
        }

        private void Ribbon_MinimizedChanged(object sender, EventArgs e)
        {
            UpdateCompactLayout(!ribbonControl.Minimized);
        }

        private void AdorneruiManager_OnQueryGuideFlyoutControl(object sender, QueryGuideFlyoutControlEventArgs e)
        {
            
        }

        private void ViewModel_OnIsShowingGuidesChanged(object sender, EventArgs e)
        {
            
        }

        private void RegisterBackstageViews()
        {
            ((ISupportInitialize)backstageViewControl).BeginInit();
            //backstageViewControl.Controls.Clear();
            //backstageViewControl.Items.Clear();
            
            //var backstageViews = ViewModel.Views.Where(c => c.Category == ViewCategory.BackstageView).ToArray();
            //for (int i = 0; i < backstageViews.Length; i++)
            //{
            //    if (_iocResolver.IsRegistered(backstageViews[i].Type)
            //        && backstageViews[i].Type.IsAssignableTo(typeof(RibbonApplicationUserControl))
            //        && !backstageViews[i].Type.IsAbstract
            //        && !backstageViews[i].Type.IsGenericType)
            //    {
            //        BackstageViewClientControl backstageViewClientControl = new();
            //        backstageViewClientControl.Name = "backstageViewClientControl" + backstageViews[i].Name;
            //        backstageViewClientControl.TabIndex = i + 3;

            //        BackstageViewTabItem bvti = new();
            //        bvti.Caption = backstageViews[i].DisplayName;
            //        bvti.Name = "bvti" + backstageViews[i].Name;
            //        bvti.ContentControl = backstageViewClientControl;
            //        if (i == 0) bvti.Selected = true;

            //        var backstageView = _iocResolver.Resolve(backstageViews[i].Type).As<RibbonApplicationUserControl>();
            //        if (backstageView != null)
            //        {
            //            backstageView.Name = "backstage" + backstageViews[i].Name;
            //            backstageView.Dock = DockStyle.Fill;
            //            backstageView.Parent = backstageViewClientControl;

            //            backstageViewClientControl.Controls.Add(backstageView);
            //            backstageViewControl.Controls.Add(backstageViewClientControl);
            //            backstageViewControl.Items.Add(bvti);
            //        }
            //    }
            //}

            //if (backstageViews.Length > 0)
            //{
            //    backstageViewControl.SelectedTab = (BackstageViewTabItem)backstageViewControl.Items[backstageViewControl.Items.FirstTabIndex];
            //    backstageViewControl.SelectedTabIndex = 0;
            //}

            ((ISupportInitialize)backstageViewControl).EndInit();
        }

        private void BackstageViewControl_SelectedTabChanged(object sender, BackstageViewItemEventArgs e)
        {
            if (e.Item == tabBackstageViewExport)
                AddBackStageViewModule(ViewModel.SelectedExportModuleType, tabBackstageViewExport);
            if (e.Item == tabBackstageViewPrint)
                AddBackStageViewModule(ViewModel.SelectedPrintModuleType, tabBackstageViewPrint);
            if (e.Item == tabBackstageViewSetting)
                AddBackStageViewModule(ViewModel.SelectedSettingsModuleType, tabBackstageViewSetting);
        }

        private void BackstageViewControl_Shown(object sender, EventArgs e)
        {
            tabBackstageViewExport.Enabled = ViewModel.SelectedExportModuleType != null;
            tabBackstageViewPrint.Enabled = ViewModel.SelectedPrintModuleType != null;
            tabBackstageViewSetting.Enabled = ViewModel.SelectedSettingsModuleType != null;
        }

        private void BackstageViewControl_Hidden(object sender, EventArgs e)
        {
            if (backstageViewControl.Items.Count == 0) return;
            if (backstageViewControl.SelectedTab != tabBackstageViewSetting)
                ViewModel.AfterReportHidden();

            backstageViewControl.SelectedTab = (BackstageViewTabItem)backstageViewControl.Items[backstageViewControl.Items.FirstTabIndex];
            backstageViewControl.SelectedTabIndex = backstageViewControl.Items.FirstTabIndex;

            ReleaseBackStageViewModule(tabBackstageViewExport);
            ReleaseBackStageViewModule(tabBackstageViewPrint);
            ReleaseBackStageViewModule(tabBackstageViewSetting);
        }

        private void AddBackStageViewModule(Module moduleType, BackstageViewTabItem tabItem)
        {
            ViewModel.BeforeReportShown(moduleType);
            tabItem.ContentControl.SuspendLayout();
            tabItem.ContentControl.Controls.Clear();
            ViewModel.AfterReportShown(moduleType);
            
            var moduleControl = GetReportView(moduleType);
            if (moduleControl != null)
            {
                moduleControl.Dock = DockStyle.Fill;
                moduleControl.Parent = tabItem.ContentControl;
            }
            
            tabItem.ContentControl.ResumeLayout();
        }

        private void ReleaseBackStageViewModule(BackstageViewTabItem tabItem)
        {
            tabItem.ContentControl.SuspendLayout();
            Control[] controls = new Control[tabItem.ContentControl.Controls.Count];
            tabItem.ContentControl.Controls.CopyTo(controls, 0);
            tabItem.ContentControl.Controls.Clear();
            for (int i = 0; i < controls.Length; i++)
                controls[i].Dispose();
            tabItem.ContentControl.ResumeLayout(false);
        }

        private void LookAndFeel_StyleChanged(object sender, EventArgs e)
        {
            backstageViewControl.Office2013StyleOptions.HeaderBackColor =
                DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
        }

        private void NavBar_ActiveGroupChanged(object sender, NavBarGroupEventArgs e)
        {
            var navPaneModuleType = (Module) e.Group.Tag;
            Control moduleControl = GetNavPaneView(navPaneModuleType);
            moduleControl.Dock = DockStyle.Fill;
            e.Group.ControlContainer.Controls.Add(moduleControl);

            var collapsedGroupModuleType = ViewModel.GetNavPaneViewType(navPaneModuleType, true);
            e.Group.CollapsedNavPaneContentControl = GetNavPaneView(collapsedGroupModuleType, true);
        }

        private void NavBar_NavPaneStateChanged(object sender, EventArgs e)
        {
            ViewModel.FiltersVisibility = navBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed
                ? CollectionViewFiltersVisibility.Minimized
                : CollectionViewFiltersVisibility.Visible;
        }

        private void OfficeNavigationBar_RegisterItem(object sender, NavigationBarNavigationClientItemEventArgs e)
        {
            NavBarGroup navGroup = (NavBarGroup)e.NavigationItem;
            var type = (Module) navGroup.Tag;
            e.Item.Tag = type;
            e.Item.Text = type.DisplayName;
            e.Item.Name = "navItem" + type.Name;
            if (type.Name == KontecgWinFormsConsts.ModuleNames.Dashboard || type.Name == KontecgWinFormsConsts.ModuleNames.Administration)
                e.Item.ShowPeekFormOnItemHover = DefaultBoolean.False;
            e.Item.BindCommand((t) => ViewModel.SelectModule(t), ViewModel, () => type);
        }

        private void OfficeNavigationBar_PopupMenuShowing(object sender, NavigationBarPopupMenuShowingEventArgs e)
        {
            if (e.MenuKind == NavigationBarMenuKind.Item)
            {
                if (e.Item.ShowPeekFormOnItemHover != DefaultBoolean.False)
                    CreateMenu(e.Menu, (Module) e.Item.Tag);
                else e.Cancel = true;
            }
        }

        private Module GetActivePeekModule()
        {
            return (Module) officeNavigationBar.PeekItem?.Tag;
        }

        private bool GetActiveShowGuideStatus()
        {
            return adorneruiManager.ShowGuides == DefaultBoolean.True;
        }

        private void OfficeNavigationBar_QueryPeekFormContent(object sender, QueryPeekFormContentEventArgs e)
        {
            var module = (Module)e.Item.Tag;
            if (!IsDockedModule(module))
                e.Control = GetPeekView(module);
        }

        private void OfficeNavigationBar_SynchronizeNavigationClientSelectedItem(object sender, NavigationBarNavigationClientSynchronizeItemEventArgs e)
        {
            var module = (Module) e.Item.Tag;
            if (!ViewModel.SelectedModuleType.Equals(module))
                ViewModel.SelectedModuleType = module;
        }

        private void CreateMenu(DXPopupMenu menu, Module moduleType)
        {
            if (IsDockedModule(moduleType))
            {
                var undockItem = new DevExpress.Utils.Menu.DXMenuItem();
                undockItem.Caption = L("HideThePeek");
                undockItem.BindCommand((t) => ViewModel.UndockPeekModule(t), ViewModel, () => moduleType);
                menu.Items.Add(undockItem);
            }
            else
            {
                var dockItem = new DevExpress.Utils.Menu.DXMenuItem();
                dockItem.Caption = L("DockThePeek");
                dockItem.BindCommand((t) => ViewModel.DockPeekModule(t), ViewModel, () => moduleType);
                var showItem = new DevExpress.Utils.Menu.DXMenuItem();
                showItem.Caption = L("ShowThePeek");
                showItem.BindCommand((t) => ViewModel.ShowPeekModule(t), ViewModel, () => moduleType);
                menu.Items.Add(dockItem);
                menu.Items.Add(showItem);
            }
        }

        public bool IsDockedModule(Module moduleType)
        {
            DockPanel panel = GetPanel(moduleType);
            return panel is {Visibility: DockVisibility.Visible};
        }

        public void DockModule(Module moduleType)
        {
            officeNavigationBar.HidePeekForm();
            DockPanel panel = GetPanel(moduleType);
            panel?.Restore();
        }

        public void UndockModule(Module moduleType)
        {
            DockPanel panel = GetPanel(moduleType);
            panel?.Close();
        }

        public void ShowPeek(Module moduleType)
        {
            officeNavigationBar.ShowPeekForm(GetNavigationBarItem(moduleType));
        }

        /// <inheritdoc />
        public void SaveLayoutToStream(MemoryStream ms)
        {
            dockManager.SaveLayoutToStream(ms);
        }

        /// <inheritdoc />
        public void RestoreLayoutFromStream(MemoryStream ms)
        {
            dockManager.RestoreLayoutFromStream(ms);
        }

        void ISupportTransitions.StartTransition(bool forward, object waitParameter)
        {
            var transition = transitionManager.Transitions[modulesContainer];
            var animator = transition.TransitionType as SlideFadeTransition;
            animator.Parameters.EffectOptions = forward ? PushEffectOptions.FromRight : PushEffectOptions.FromLeft;

            if (waitParameter == null)
                transition.ShowWaitingIndicator = DefaultBoolean.False;
            else
            {
                transition.ShowWaitingIndicator = DefaultBoolean.True;
                transition.WaitingIndicatorProperties.Caption = DevExpress.XtraEditors.EnumDisplayTextHelper.GetDisplayText(waitParameter);
                transition.WaitingIndicatorProperties.Description = L("Loading...");
                transition.WaitingIndicatorProperties.ContentMinSize = new Size(160, 0);
            }

            transitionManager.StartTransition(modulesContainer);
        }

        void ISupportTransitions.EndTransition()
        {
            transitionManager.EndTransition();
        }

        private void UpdateCompactLayout(bool compact)
        {
            if (ViewModel.SelectedNavPaneModuleType != null)
                UpdateCompactLayout(GetNavPaneView(ViewModel.SelectedNavPaneModuleType) as ISupportCompactLayout, compact);
            if (ViewModel.SelectedNavPaneHeaderModuleType != null)
                UpdateCompactLayout(GetNavPaneView(ViewModel.SelectedNavPaneHeaderModuleType, compact) as ISupportCompactLayout, compact);
        }

        private void UpdateCompactLayout(ISupportCompactLayout module, bool compact)
        {
            if (module != null)
                module.Compact = compact;
        }

        #region Resolve Controls

        private DockPanel GetPanel(Module moduleType)
        {
            var id = ViewModel.GetModuleId(moduleType);
            return dockManager.Panels.Concat(dockManager.HiddenPanels)
                              .FirstOrDefault(p => p.ID == id);
        }

        private NavigationBarItem GetNavigationBarItem(Module moduleType)
        {
            return officeNavigationBar.Items
                                      .FirstOrDefault(item => moduleType.Equals(item.Tag));
        }

        private NavBarGroup GetNavBarGroup(Module moduleType)
        {
            return navBar.Groups
                         .FirstOrDefault(g => moduleType.Equals(g.Tag));
        }

        private Control GetPeekView(Module moduleType)
        {
            Control moduleControl = ViewModel.GetModuleControl(moduleType, ViewModel.SelectedModuleViewModel, category: ViewCategory.PeekView) as Control;
            ViewModelHelper.EnsureModuleViewModel(moduleControl, ViewModel);
            return moduleControl;
        }

        private Control GetNavPaneView(Module moduleType, bool collapsed = false)
        {
            Control moduleControl = ViewModel.GetModuleControl(moduleType, ViewModel.SelectedModuleViewModel, category: !collapsed ? ViewCategory.FilterPaneView : ViewCategory.FilterPaneCollapsedView) as Control;
            ViewModelHelper.EnsureModuleViewModel(moduleControl, ViewModel);
            return moduleControl;
        }

        private Control GetReportView(Module moduleType)
        {
            Control moduleControl = ViewModel.GetModuleControl(moduleType, null, category: ViewCategory.PrintView) as Control;
            ViewModelHelper.EnsureModuleViewModel(moduleControl, ViewModel.SelectedModuleViewModel, ViewModel.ReportParameter);
            return moduleControl;
        }

        private Control GetSettingsView(Module moduleType)
        {
            Control moduleControl = ViewModel.GetModuleControl(moduleType, null, category: ViewCategory.SettingsView) as Control;
            ViewModelHelper.EnsureModuleViewModel(moduleControl, ViewModel.SelectedModuleViewModel);
            return moduleControl;
        }

        #endregion

        #region Notifications

        private int _notificationsCount;
        System.Windows.Forms.Timer _notificationsTimer;
        AlertControl _alertControl;

        private void InitNotifications()
        {
            if (CanUseToastNotifications())
            {
                notificationManager.ApplicationId = KontecgWinFormsConsts.ProcessName;
                notificationManager.TryCreateApplicationShortcut();
                notificationManager.Activated += NotificationsManager_Activated;
            }
            else
            {
                _alertControl = new AlertControl(components);
                _alertControl.AllowHtmlText = true;
                _alertControl.FormLocation = AlertFormLocation.TopRight;
                _alertControl.ShowPinButton = false;
                _alertControl.AlertClick += AlertControl_AlertClick;
            }

            EnsureNotificationsTimer();
        }

        private void EnsureNotificationsTimer()
        {
            if (_notificationsTimer == null)
            {
                _notificationsTimer = new System.Windows.Forms.Timer(components);
                _notificationsTimer.Interval = 60000;
                _notificationsTimer.Tick += NotificationsTimer_Tick;
            }
            _notificationsTimer.Start();
        }

        private void DestroyNotificationsTimer()
        {
            if (_notificationsTimer != null)
            {
                _notificationsTimer.Stop();
                _notificationsTimer.Tick -= NotificationsTimer_Tick;
                _notificationsTimer.Dispose();
            }
            _notificationsTimer = null;
        }

        private void NotificationsTimer_Tick(object sender, EventArgs e)
        {
            if (notificationManager.IsDisposing)
            {
                DestroyNotificationsTimer();
                return;
            }
            if (_notificationsCount < notificationManager.Notifications.Count)
            {
                _notificationsTimer.Interval = 120000;
                ShowNotification(_notificationsCount++);
            }
            else DestroyNotificationsTimer();
        }

        private void AlertControl_AlertClick(object sender, AlertClickEventArgs e)
        {
            object notificationId = e.Info.Tag;
            e.AlertForm.Close();

            OnNotificationClick(notificationId);
        }

        private void NotificationsManager_Activated(object sender, ToastNotificationEventArgs e)
        {
            OnNotificationClick(e.NotificationID);
        }

        private bool CanUseToastNotifications()
        {
            return ToastNotificationsManager.AreToastNotificationsSupported;
        }

        private void ShowNotification(int index)
        {
            var notification = notificationManager.Notifications[index];
            if (CanUseToastNotifications())
                notificationManager.ShowNotification(notification);
            else
            {
                var alertInfo = new AlertInfo(
                    caption: "<b>" + notification.Header + "</b>",
                    text: notification.Body + " " + notification.Body2,
                    image: AppHelper.AppImage)
                {
                    Tag = notification.ID
                };
                _alertControl.Show(this, alertInfo);
            }
        }

        private void OnNotificationClick(object notificationId)
        {
            var backstageViewForm = backstageViewControl.FindForm();
            if (backstageViewForm != null && backstageViewForm != this)
            {
                backstageViewForm.Hide();
                ribbonControl.HideApplicationButtonContentControl();
            }

            //if (notificationId == notificationManager.Notifications[0].ID)
            //{
            //    ViewModel.SelectedModuleType = ModuleType.Orders;
            //}

            //if (notificationId == notificationManager.Notifications[1].ID)
            //{
            //    ISupportMap supportMap = ViewModel.SelectedModuleViewModel as ISupportMap;
            //    if (supportMap != null && supportMap.CanShowMap())
            //        supportMap.ShowMap();
            //}

            //if (notificationId == notificationManager.Notifications[2].ID)
            //{
            //    ViewModel.SelectedModuleType = ModuleType.Products;
            //}

            //if (notificationId == notificationManager.Notifications[3].ID)
            //{
            //    if (!(ViewModel.SelectedModuleViewModel is ISupportAnalysis))
            //        ViewModel.SelectedModuleType = ModuleType.Customers;
            //    ISupportAnalysis supportAnalysis = ViewModel.SelectedModuleViewModel as ISupportAnalysis;
            //    if (supportAnalysis != null)
            //        supportAnalysis.ShowAnalysis();
            //}
        }

        #endregion
    }
}

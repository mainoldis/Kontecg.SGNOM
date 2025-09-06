using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Kontecg.Views;
using DevExpress.Mvvm.POCO;
using Kontecg.Domain;
using Kontecg.Configuration;
using Kontecg.Features;
using Kontecg.Services;
using Kontecg.Dependency;
using DevExpress.Mvvm;
using Kontecg.Runtime.Session;
using Kontecg.Authorization.Users.Profile.Dto;
using Kontecg.Sessions.Dto;
using Kontecg.Authorization.Users.Profile;
using Kontecg.Sessions;
using Kontecg.Threading;

namespace Kontecg.ViewModels
{
    public class MainViewModel : KontecgViewModelBase, IZoomViewModel
    {
        private readonly IModuleLocator _moduleLocator;
        private readonly ISessionAppService _sessionAppService;
        private readonly IProfileAppService _profileAppService;
        private readonly ITransitionService _transitionService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IIocResolver _iocResolver;


        public MainViewModel(IModuleLocator moduleLocator, 
            ISessionAppService sessionAppService,
            IProfileAppService profileAppService,
            ITransitionService transitionService, 
            IWorkspaceService workspaceService,
            IIocResolver iocResolver)
        {
            _moduleLocator = moduleLocator;
            _sessionAppService = sessionAppService;
            _profileAppService = profileAppService;
            _transitionService = transitionService;
            _workspaceService = workspaceService;
            _iocResolver = iocResolver;
            RegisterLocalServices();
        }

        private void RegisterLocalServices()
        {
            ISupportServices localServices = (ISupportServices)this;
        }

        public Form Owner { get; set; }

        [Command]
        public virtual void ShowInfo()
        {
            DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin,
                new DevExpress.Utils.About.ProductStringInfo("KONTECG", "SGNOM"));
        }

        #region Navigation & User

        public virtual bool IsLogin { get; set; }

        public virtual IReadOnlyList<Module> Modules => _moduleLocator.Modules.ToList();

        public virtual GetCurrentLoginInformationOutput LoginInformation => _sessionAppService.GetCurrentLoginInformation();

        public virtual GetProfilePictureOutput ProfilePicture => AsyncHelper.RunSync(() => _profileAppService.GetProfilePicture());

        [Command]
        public virtual void Logout()
        {
            RaiseTerminate(new FormClosingEventArgs(CloseReason.UserClosing, false));
        }

        public bool CanLogout()
        {
            return KontecgSession.UserId != null;
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

        #endregion

        #region Views

        public virtual Module SelectedModuleType { get; set; }

        public Module SelectedNavPaneModuleType => GetNavPaneViewType(SelectedModuleType);

        public Module SelectedPeekModuleType => GetPeekViewType(SelectedModuleType);

        public Module SelectedNavPaneHeaderModuleType => GetNavPaneViewType(SelectedModuleType, true);

        public Module SelectedExportModuleType => GetExportViewType(SelectedModuleType);

        public Module SelectedPrintModuleType => GetPrintViewType(SelectedModuleType);

        public Module SelectedSettingsModuleType => GetSettingsViewType(SelectedModuleType);

        public virtual object SelectedModuleControl { get; set; }

        public object SelectedModuleViewModel => ((ISupportViewModel)SelectedModuleControl).ViewModel;

        [Command]
        public virtual void SelectModule(Module moduleType)
        {
            SelectedModuleType = moduleType;
        }

        public bool CanSelectedModule(Module moduleType)
        {
            return SelectedModuleType != moduleType && moduleType != null;
        }

        public bool CanDockPeekModule(Module moduleType)
        {
            return !(Owner as IPeekViewHost).IsDockedModule(moduleType);
        }

        [Command]
        public void DockPeekModule(Module moduleType)
        {
            (Owner as IPeekViewHost).DockModule(moduleType);
        }

        [Command]
        public void UndockPeekModule(Module moduleType)
        {
            (Owner as IPeekViewHost).UndockModule(moduleType);
        }

        public bool CanUndockPeekModule(Module moduleType)
        {
            return (Owner as IPeekViewHost).IsDockedModule(moduleType);
        }

        public bool CanShowPeekModule(Module moduleType)
        {
            return !(Owner as IPeekViewHost).IsDockedModule(moduleType);
        }

        [Command]
        public void ShowPeekModule(Module moduleType)
        {
            (Owner as IPeekViewHost).ShowPeek(moduleType);
        }

        public Module[] PeekModules => _moduleLocator.Modules.Where(m => m.CanShowPeekView).Select(m => m).ToArray();

        public Module[] TopNavigation => _moduleLocator.Modules.Select(m => m).ToArray();

        public event EventHandler ModuleControlAdded;

        public event EventHandler ModuleControlRemoved;

        public event EventHandler SelectedModuleControlChanged;

        public event EventHandler SelectedModuleTypeChanged;

        protected virtual void OnSelectedModuleTypeChanged(Module oldType)
        {
            var transitionService = _iocResolver.ResolveAsDisposable<ITransitionService>();
            object waitParameter = !IsModuleLoaded(SelectedModuleType) ? (object)SelectedModuleType : null;
            bool effective = (SelectedModuleType != null) && (oldType != null);
            using (transitionService.Object.StartTransition(SelectedModuleType?.Order > oldType?.Order, waitParameter))
            {
                if (oldType != null)
                    _workspaceService.SaveWorkspace(oldType.Name);
                else
                    _workspaceService.SetupDefaultWorkspace();

                try
                {
                    SelectedModuleControl = GetModuleControl(SelectedModuleType, null);
                    RaiseSelectedModuleTypeChanged();
                }
                catch (Exception e)
                {
                }

                if (SelectedModuleType != null)
                    _workspaceService.RestoreWorkspace(SelectedModuleType.Name);
            }
        }

        protected virtual void OnSelectedModuleControlChanged(object oldModule)
        {
            if (oldModule != null) ModuleControlRemoved?.Invoke(oldModule, EventArgs.Empty);

            SelectedModuleControlChanged?.Invoke(this, EventArgs.Empty);

            if (SelectedModuleControl == null) return;
            ViewModelHelper.EnsureModuleViewModel(SelectedModuleControl, this);
            ModuleControlAdded?.Invoke(SelectedModuleControl, EventArgs.Empty);
        }

        protected virtual void RaiseSelectedModuleTypeChanged()
        {
            this.RaiseCanExecuteChanged(x => x.SelectModule(null));
            this.RaisePropertyChanged(x => SelectedNavPaneModuleType);
            this.RaisePropertyChanged(x => SelectedNavPaneHeaderModuleType);
            SelectedModuleTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool IsModuleLoaded(Module moduleType)
        {
            return _moduleLocator.IsModuleLoaded(moduleType);
        }

        public Guid GetModuleId(Module moduleType)
        {
            return _moduleLocator.GetModule(moduleType)?.Id ?? Guid.Empty;
        }

        public string GetModuleImageUri(Module moduleType, bool smallImage = false, bool viewImage = false, ViewCategory category = ViewCategory.MainView)
        {
            var module = _moduleLocator.GetModule(moduleType);
            if(module == null) return null;

            if (!viewImage)
                return smallImage ? module.SmallImageUri : module.LargeImageUri;

            switch (category)
            {
                case ViewCategory.EditView:
                    return module.EditView?.Icon;
                case ViewCategory.PeekView:
                    return module.PeekView?.Icon;
                case ViewCategory.ExportView:
                    return module.ExportView?.Icon;
                case ViewCategory.PrintView:
                    return module.PrintView?.Icon;
                case ViewCategory.AnalysisView:
                    return module.AnalysisView?.Icon;
                case ViewCategory.SettingsView:
                    return module.SettingsView?.Icon;
                default:
                    return module.MainView?.Icon;
            }
        }

        public Type GetModuleResourceType(Module moduleType)
        {
            var module = _moduleLocator.GetModule(moduleType);
            return module?.ResourceType;
        }

        public string GetModuleCaption(Module moduleType)
        {
            return moduleType.MainView?.DisplayName ??
                   moduleType.DisplayName;
        }

        public Module GetNavPaneViewType(Module moduleType, bool collapsed = false)
        {
            var module = collapsed
                ? _moduleLocator.GetModuleType(moduleType, ViewCategory.FilterPaneCollapsedView)
                : _moduleLocator.GetModuleType(moduleType, ViewCategory.FilterPaneView);

            return module;
        }

        public Module GetPeekViewType(Module moduleType)
        {
            return _moduleLocator.GetModuleType(moduleType, ViewCategory.PeekView);
        }

        public Module GetExportViewType(Module moduleType)
        {
            return _moduleLocator.GetModuleType(moduleType, ViewCategory.ExportView);
        }

        public Module GetPrintViewType(Module moduleType)
        {
            return _moduleLocator.GetModuleType(moduleType, ViewCategory.PrintView);
        }

        public Module GetSettingsViewType(Module moduleType)
        {
            return _moduleLocator.GetModuleType(moduleType, ViewCategory.SettingsView);
        }

        public object GetModuleControl(Module moduleType, object viewModel, object parameter = null, ViewCategory category = ViewCategory.MainView)
        {
            return _moduleLocator.GetModuleControl(moduleType, viewModel, parameter, category);
        }

        #endregion

        #region ReadingMode

        public virtual bool IsReadingMode { get; set; }

        [Command]
        public void TurnOnReadingMode()
        {
            IsReadingMode = true;
        }

        public bool CanTurnOnReadingMode()
        {
            return !IsReadingMode;
        }

        [Command]
        public void TurnOffReadingMode()
        {
            IsReadingMode = false;
        }
        public bool CanTurnOffReadingMode()
        {
            return IsReadingMode;
        }

        public event EventHandler IsReadingModeChanged;

        protected virtual void OnIsReadingModeChanged()
        {
            this.RaiseCanExecuteChanged(x => x.TurnOnReadingMode());
            this.RaiseCanExecuteChanged(x => x.TurnOffReadingMode());
            RaiseIsReadingModeChanged();
        }

        private void RaiseIsReadingModeChanged()
        {
            IsReadingModeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region FiltersVisibility

        public virtual CollectionViewFiltersVisibility FiltersVisibility { get; set; }

        [Command]
        public void ShowFilters()
        {
            FiltersVisibility = CollectionViewFiltersVisibility.Visible;
        }

        public bool CanShowFilters()
        {
            return FiltersVisibility != CollectionViewFiltersVisibility.Visible;
        }

        [Command]
        public void MinimizeFilters()
        {
            FiltersVisibility = CollectionViewFiltersVisibility.Minimized;
        }

        public bool CanMinimizeFilters()
        {
            return FiltersVisibility != CollectionViewFiltersVisibility.Minimized;
        }

        [Command]
        public void HideFilters()
        {
            FiltersVisibility = CollectionViewFiltersVisibility.Hidden;
        }

        public bool CanHideFilters()
        {
            return FiltersVisibility != CollectionViewFiltersVisibility.Hidden;
        }

        public event EventHandler FiltersVisibilityChanged;

        protected virtual void OnFiltersVisibilityChanged()
        {
            this.RaiseCanExecuteChanged(x => x.ShowFilters());
            this.RaiseCanExecuteChanged(x => x.MinimizeFilters());
            this.RaiseCanExecuteChanged(x => x.HideFilters());
            RaiseFiltersVisibilityChanged();
        }

        private void RaiseFiltersVisibilityChanged()
        {
            FiltersVisibilityChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Print & Reports

        public object ReportParameter { get; set; }

        public Module CurrentReport { get; set; }

        public event EventHandler<PrintEventArgs> Print;

        public void BeforeReportShown(Module reportType)
        {
            if (ReportParameter != null)
                return;
        }

        public void AfterReportShown(Module reportType)
        {
            if (CurrentReport != reportType)
            {
                bool reportChanged = CurrentReport != null;
                CurrentReport = reportType;
                if (reportChanged && reportType != null)
                {
                    //TODO: Completar lo que resta con el ViewModel
                }
            }
        }

        public void AfterReportHidden()
        {
            CurrentReport = null;
            ReportParameter = null;
        }

        public void RaisePrint(object parameter)
        {
            ReportParameter = parameter;
            Print?.Invoke(this, new PrintEventArgs(ReportParameter));
        }

        #endregion

        public event EventHandler ShowAllFolders;

        public void RaiseShowAllFolders()
        {
            ShowAllFolders?.Invoke(this, EventArgs.Empty);
        }

        #region Zoom

        /// <inheritdoc />
        public ISupportZoom ZoomComponent => SelectedModuleControl as ISupportZoom;

        /// <inheritdoc />
        public event EventHandler ZoomComponentChanged
        {
            add => SelectedModuleControlChanged += value;
            remove => SelectedModuleControlChanged -= value;
        }

        #endregion

        #region Theme

        public virtual bool IsThemeFeatureEnabled => IsEnabled(WinFormsFeatureNames.ChangeThemeFeature) &&
                                                     SettingManager.GetSettingValue<bool>(WinFormsSettings.Theme.AllowChangeTheme);

        #endregion

        #region Information

        public virtual string UserInformation
        {
            get
            {
                var loginInformation = LoginInformation;
                return $@"{loginInformation.Company?.CompanyName} {loginInformation.User?.Name} {loginInformation.User?.Surname}";
            }
        } 

        public virtual string Information { get; set; }

        #endregion

        #region Guides

        public virtual bool IsShowingGuides { get; set; }

        [Command]
        public void ToggleGuides()
        {
            IsShowingGuides = !IsShowingGuides;
        }
        
        public event EventHandler IsShowingGuidesChanged;

        protected virtual void OnIsShowingGuidesChanged()
        {
            RaiseIsShowingGuidesChanged();
        }

        private void RaiseIsShowingGuidesChanged()
        {
            IsShowingGuidesChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}

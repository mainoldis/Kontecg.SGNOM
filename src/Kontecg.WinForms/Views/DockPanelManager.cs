using DevExpress.XtraBars.Docking;
using Kontecg.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Kontecg.Views
{
    public class DockPanelManager : KontecgCoreDomainServiceBase
    {
        private readonly IDictionary<Guid, object> _panels;
        private MainViewModel _mainViewModel;
        private DockManager _dockManager;

        public DockPanelManager()
        {
            _panels = new Dictionary<Guid, object>();
            LocalizationSourceName = KontecgWinFormsConsts.LocalizationSourceName;
        }

        public void Initialize(DockManager dockManager, MainViewModel mainViewModel)
        {
            _dockManager = dockManager;
            _mainViewModel = mainViewModel;
        }

        public void RegisterPeekPanels()
        {
            if (_panels.Count > 0)
            {
                _panels.Clear();
                _dockManager.ClosedPanel -= DockManager_OnClosedPanel;
                _dockManager.VisibilityChanged -= DockManager_OnVisibilityChanged;
                _dockManager.StartDocking -= DockManager_OnStartDocking;
            }

            _dockManager.ClosedPanel += DockManager_OnClosedPanel;
            _dockManager.VisibilityChanged += DockManager_OnVisibilityChanged;
            _dockManager.StartDocking += DockManager_OnStartDocking;
            _dockManager.BeginInit();

            var moduleTypes = _mainViewModel.PeekModules;
            for (int i = 0; i < moduleTypes.Length; i++) RegisterPeekPanel(moduleTypes[i]);

            _dockManager.EndInit();
        }

        private void RegisterPeekPanel(Module module)
        {
            if(module == null) return;

            var panel = new DockPanel();
            panel.ID = module.Id;
            _panels.Add(panel.ID, _mainViewModel.GetModuleControl(module, null, null, ViewCategory.PeekView));
            panel.Name = $@"peekPanel{module.Name}";
            panel.Options.AllowDockBottom = false;
            panel.Options.AllowDockLeft = false;
            panel.Options.AllowDockTop = false;
            panel.Options.AllowFloating = false;
            panel.Text = module.PeekView.DisplayName;
            panel.Visibility = DockVisibility.Hidden;
            panel.SavedDock = DockingStyle.Right;
            panel.OriginalSize = new System.Drawing.Size(200, 200);

            if (_dockManager.HiddenPanels.Count > 0)
            {
                panel.SavedParent = _dockManager.HiddenPanels[0];
                panel.Dock = DockingStyle.Fill;
                panel.SavedDock = DockingStyle.Fill;
                panel.SavedIndex = _dockManager.HiddenPanels.Count - 1;
            }

            var container = new ControlContainer();
            container.Name = $@"{panel.Name}ControlContainer";
            panel.Controls.Add(container);
            panel.Register(_dockManager);
            _dockManager.HiddenPanels.AddRange([panel]);
        }

        private void DockManager_OnStartDocking(object sender, DockPanelCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void DockManager_OnVisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (e.Visibility == DockVisibility.Visible && _panels.ContainsKey(e.Panel.ID))
            {
                Control viewControl = GetPeekViewControl(e.Panel);
                ViewModelHelper.EnsureModuleViewModel(viewControl, _mainViewModel);
                viewControl.Dock = DockStyle.Fill;
                e.Panel.ControlContainer.Controls.Add(viewControl);
            }
        }

        private void DockManager_OnClosedPanel(object sender, DockPanelEventArgs e)
        {
            Control viewControl = GetPeekViewControl(e.Panel);
            e.Panel.ControlContainer.Controls.Remove(viewControl);
        }

        private Control GetPeekViewControl(DockPanel panel)
        {
            return _panels[panel.ID] as Control;
        }
    }
}

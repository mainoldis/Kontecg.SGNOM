using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils;
using Kontecg.Dependency;
using Kontecg.Domain;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;
using Kontecg.ViewModels;
using Kontecg.Views;

namespace Kontecg.Services
{
    internal class ModuleLocator : ISingletonDependency, IModuleLocator
    {
        private readonly IIocResolver _iocResolver;
        private const string ModuleCacheName = "ModuleCacheName";
        private readonly ITypedCache<string, Module> _cache;
        private readonly IList<Module> _modules;

        public ModuleLocator(
            IIocResolver iocResolver,
            IUserModuleManager moduleManager,
            IKontecgSession kontecgSession,
            ICacheManager cacheManager)
        {
            _iocResolver = iocResolver;
            _cache = cacheManager.GetCache<string, Module>(ModuleCacheName);
            _modules = moduleManager.GetModules(kontecgSession.ToUserIdentifier());
        }

        public IList<Module> Modules => _modules;

        private void PutOnCache(Module module)
        {
            _cache.Set($"{module.Name}.{module.Id}", module);
        }

        private Module FindModule(IList<Module> modules, string moduleType)
        {
            if(modules.Count == 0) return null;

            Module found = null;

            foreach (var module in modules)
            {
                if (module.Name == moduleType)
                {
                    found = module;
                    break;
                }
                    
                found = FindModule(module.SubModules, moduleType);
            }

            return found;
        }

        private object ActivateView(UserView userView, object viewModel, object parameter = null)
        {
            if(userView == null) return null;

            try
            {
                var scope  = _iocResolver.CreateScope();
                var control = scope.Resolve(userView.Type, viewModel);
                if(control == null) return null;

                if (control is BaseRibbonForm ribbonForm)
                {
                    ribbonForm.IconOptions.ImageUri = new DxImageUri(userView.Icon);
                    var ribbonOwner = (IRibbonOwner) ribbonForm;
                    ribbonOwner.Ribbon.Manager.HideBarsWhenMerging = false;
                    ribbonOwner.Ribbon.StatusBar.HideWhenMerging = DefaultBoolean.False;
                    ribbonOwner.Ribbon.StatusBar.Visible = false;
                    ribbonOwner.Ribbon.Visible = false;
                }

                if (control is BaseForm form)
                {
                    form.IconOptions.ImageUri = new DxImageUri(userView.Icon);
                }

                ViewModelHelper.EnsureViewModel(viewModel, null, parameter);

                userView.Control = control;
                return userView.Control;
            }
            catch
            {
            }

            return null;
        }

        public Module GetModule(Module moduleType)
        {
            if(moduleType == null) return null;
            var module = FindModule(_modules, moduleType.Name);

            if (module == null)
                return null;

            var cached = _cache.GetOrDefault($"{module.Name}.{module.Id}");
            if (cached != null)
                return cached;

            PutOnCache(module);
            return module;
        }

        public Module GetModuleType(Module moduleType, ViewCategory? category = null)
        {
            var module = GetModule(moduleType);

            if (module == null) return null;

            if (category == null)
                return module;

            switch (category)
            {
                case ViewCategory.DetailView:
                    return module.DetailView?.Owner;
                case ViewCategory.FilterPaneView:
                    return module.FilterPaneView?.Owner;
                case ViewCategory.FilterPaneCollapsedView:
                    return module.FilterPaneCollapsedView?.Owner;
                case ViewCategory.CustomFilterView:
                    return module.CustomFilterView?.Owner;
                case ViewCategory.GroupFilterView:
                    return module.GroupFilterView?.Owner;
                case ViewCategory.EditView:
                    return module.EditView?.Owner;
                case ViewCategory.PeekView:
                    return module.PeekView?.Owner;
                case ViewCategory.ExportView:
                    return module.ExportView?.Owner;
                case ViewCategory.PrintView:
                    return module.PrintView?.Owner;
                case ViewCategory.AnalysisView:
                    return module.AnalysisView?.Owner;
                case ViewCategory.SettingsView:
                    return module.SettingsView?.Owner;
                default:
                    return module.MainView?.Owner;
            }
        }

        public object GetModuleControl(Module moduleType, object viewModel, object parameter = null, ViewCategory category = ViewCategory.MainView)
        {
            var module = GetModule(moduleType);
            if(module == null) return null;
            
            switch (category)
            {
                case ViewCategory.DetailView:
                    return ActivateView(module.DetailView, viewModel, parameter);
                case ViewCategory.FilterPaneView:
                    return ActivateView(module.FilterPaneView, viewModel, parameter);
                case ViewCategory.FilterPaneCollapsedView:
                    return ActivateView(module.FilterPaneCollapsedView, viewModel, parameter);
                case ViewCategory.CustomFilterView:
                    return ActivateView(module.CustomFilterView, viewModel, parameter);
                case ViewCategory.GroupFilterView:
                    return ActivateView(module.GroupFilterView, viewModel, parameter);
                case ViewCategory.EditView:
                    return ActivateView(module.EditView, viewModel, parameter);
                case ViewCategory.PeekView:
                    return ActivateView(module.PeekView, viewModel, parameter);
                case ViewCategory.ExportView:
                    return ActivateView(module.ExportView, viewModel, parameter);
                case ViewCategory.PrintView:
                    return ActivateView(module.PrintView, viewModel, parameter);
                case ViewCategory.AnalysisView:
                    return ActivateView(module.AnalysisView, viewModel, parameter);
                case ViewCategory.SettingsView:
                    return ActivateView(module.SettingsView, viewModel, parameter);
                default:
                    return ActivateView(module.MainView, viewModel, parameter);
            }
        }

        /// <inheritdoc />
        public void ReleaseModuleControl(object module)
        {
        }

        /// <inheritdoc />
        public bool IsModuleLoaded(Module moduleType)
        {
            return _cache.GetOrDefault(_modules.Select(m => $"{m.Name}.{m.Id}").ToArray())
                         .All(m => m != null);
        }

    }
}

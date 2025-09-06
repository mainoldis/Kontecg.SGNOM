using System.Diagnostics;
using DevExpress.Mvvm.POCO;
using Kontecg.Dependency;
using Kontecg.ViewModels;

namespace Kontecg.Presenters
{
    public abstract class BasePresenter<TViewModel> where TViewModel : KontecgViewModelBase
    {
        private bool _isDisposing;

        protected BasePresenter(TViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public TViewModel ViewModel { [DebuggerStepThrough] get; private set; }

        protected TParentViewModel GetParentViewModel<TParentViewModel>()
        {
            return (TParentViewModel)((ISupportParentViewModel)ViewModel).ParentViewModel;
        }

        protected virtual void OnDisposing() { }

        public void Dispose()
        {
            if (_isDisposing) return;
            _isDisposing = true;
            OnDisposing();
            ViewModel = null;
        }

        protected TService GetService<TService>() where TService : class
        {
            var serviceContainer = GetServiceContainer();
            if (serviceContainer != null && serviceContainer.IsRegistered<TService>())
                return serviceContainer.Resolve<TService>();

            var secondServiceContainer = GetServiceContainerFromAnother();
            return secondServiceContainer != null
                ? serviceContainer.GetService<TService>()
                : null;
        }

        private IIocManager GetServiceContainer()
        {
            if (!(ViewModel is IIocManagerAccessor)) return null;
            return ((IIocManagerAccessor)ViewModel).IocManager;
        }

        private DevExpress.Mvvm.IServiceContainer GetServiceContainerFromAnother()
        {
            if (!(ViewModel is DevExpress.Mvvm.ISupportServices)) return null;
            return ((DevExpress.Mvvm.ISupportServices)ViewModel).ServiceContainer;
        }
    }
}
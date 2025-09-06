using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Kontecg.Domain;

namespace Kontecg.ViewModels.Shared
{
    public class ViewSettingsViewModel : KontecgViewModelBase
    {
        private readonly CollectionUiViewModel _collectionUiViewModelCore;

        public static ViewSettingsViewModel Create(CollectionUiViewModel collectionUiViewModel)
        {
            return ViewModelSource.Create(() => new ViewSettingsViewModel(collectionUiViewModel));
        }

        protected ViewSettingsViewModel(CollectionUiViewModel collectionUiViewModel)
        {
            _collectionUiViewModelCore = collectionUiViewModel;
        }

        [Command]
        public void ResetCustomFilters()
        {
            var vm = ViewModelHelper.GetParentViewModel<ISupportCustomFilters>(this);
            vm?.ResetCustomFilters();
        }
        public CollectionUiViewModel CollectionUiViewModel => _collectionUiViewModelCore;

        [Command]
        public void ResetView()
        {
            CollectionUiViewModel.ResetView();
        }

        public IDocument Document { get; set; }

        [Command]
        public void Ok()
        {
            Document.Close();
        }

        [Command]
        public void Cancel()
        {
            Document.Close();
        }
    }
}
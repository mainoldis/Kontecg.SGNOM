using Kontecg.ViewModels;
using Kontecg.ViewModels.Shared;

namespace Kontecg.Views.Shared
{
    public partial class ViewSettingsControl : BaseUserControl
    {
        /// <inheritdoc />
        public ViewSettingsControl(CollectionUiViewModel collectionViewModel)
            : base(typeof(ViewSettingsViewModel), [collectionViewModel])
        {
            InitializeComponent();

            BindCommands();
        }

        public ViewSettingsViewModel ViewModel => GetViewModel<ViewSettingsViewModel>();

        protected void BindCommands()
        {
            resetFiltersBtn.BindCommand(() => ViewModel.ResetCustomFilters(), ViewModel);
            resetViewBtn.BindCommand(() => ViewModel.ResetView(), ViewModel);

            okBtn.BindCommand(() => ViewModel.Ok(), ViewModel);
            cancelBtn.BindCommand(() => ViewModel.Cancel(), ViewModel);
        }
    }
}
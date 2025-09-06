using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Kontecg.Common.Dto;
using Kontecg.Domain;
using Kontecg.HumanResources;
using Kontecg.HumanResources.Dto;
using System.Linq;
using DevExpress.Mvvm.POCO;
using Kontecg.Threading;
using Kontecg.ViewModels.Shared;

namespace Kontecg.ViewModels.Persons
{
    public class PersonsCollectionViewModel : EntitiesViewModel<PersonDto, long, FindPersonsInput, IHumanResourcesAppService>, ISupportCustomFilters
    {
        /// <inheritdoc />
        public PersonsCollectionViewModel(IHumanResourcesAppService humanResourcesAppService, ICancellationTokenProvider cancellationTokenProvider)
            : base(humanResourcesAppService, null)
        {
            DefaultEntitySelector = SelectedEntityCallback;
        }

        private PersonDto SelectedEntityCallback()
        {
            if(HasEntities && !HasMultipleSelection)
                return FilteredEntities[0];
            return null;
        }

        [Command]
        public void ShowViewSettings()
        {
            var dms = this.GetService<IDocumentManagerService>("View Settings");
            if (dms != null)
            {
                var document = dms.Documents.FirstOrDefault(d => d.Content is ViewSettingsViewModel) ??
                               dms.CreateDocument("View Settings", null, null, this);
                document.Show();
            }
        }

        #region ISupportCustomFilters

        public event EventHandler CustomFilter;

        public event EventHandler CustomFiltersReset;

        [Command]
        public void ResetCustomFilters()
        {
            RaiseCustomFiltersReset();
        }

        [Command]
        public void NewCustomFilter()
        {
            RaiseCustomFilter();
        }

        private void RaiseCustomFilter()
        {
            CustomFilter?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseCustomFiltersReset()
        {
            CustomFiltersReset?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ShowAllFolders

        [Command]
        public void ShowAllFolders()
        {
            RaiseShowAllFolders();
        }

        private void RaiseShowAllFolders()
        {
            MainViewModel mainViewModel = ViewModelHelper.GetParentViewModel<MainViewModel>(this);
            if (mainViewModel != null)
                mainViewModel.RaiseShowAllFolders();
        }

        #endregion

        #region Print

        public bool CanPrint()
        {
            return AllowPrintEntities && HasSelection;
        }

        private void RaisePrint(string reportType)
        {
            MainViewModel mainViewModel = ViewModelHelper.GetParentViewModel<MainViewModel>(this);
            if (mainViewModel != null)
                mainViewModel.RaisePrint(reportType);
        }

        #endregion
    }
}
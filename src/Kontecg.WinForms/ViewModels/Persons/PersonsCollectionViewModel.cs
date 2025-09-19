using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Kontecg.Common.Dto;
using Kontecg.Domain;
using Kontecg.HumanResources;
using Kontecg.HumanResources.Dto;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm.POCO;
using Kontecg.Application.Services.Dto;
using Kontecg.Primitives;
using Kontecg.Threading;
using Kontecg.ViewModels.Shared;

namespace Kontecg.ViewModels.Persons
{
    public class PersonsCollectionViewModel : EntitiesViewModel<PersonDto, long, FindPersonsInput, IHumanResourcesAppService>, ISupportCustomFilters, ISupportAnalysis
    {
        /// <inheritdoc />
        public PersonsCollectionViewModel(IHumanResourcesAppService humanResourcesAppService, ICancellationTokenProvider cancellationTokenProvider)
            : base(humanResourcesAppService, null)
        {
            DefaultEntitySelector = SelectedEntityCallback;
        }

        private PersonDto SelectedEntityCallback()
        {
            if(IsLoaded && !HasMultipleSelection)
                return Entities[0];
            return null;
        }

        /// <inheritdoc />
        protected override async ValueTask<PagedResultDto<PersonDto>> LoadDataSourceAsync()
        {
            return !HasService ? new PagedResultDto<PersonDto>() : Service.GetAll(Filter);
        }

        [Command]
        public void QuickReport(string reportType)
        {
            RaisePrint(reportType);
        }

        public bool CanQuickReport(string reportType)
        {
            return AllowPrintEntities && HasSelection;
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

        /// <inheritdoc />
        [Command]
        public void ShowAnalysis()
        {
            ShowDocument<HumanResourcesAnalysisViewModel>(KontecgWinFormsConsts.ModuleNames.HumanResources, null);
        }

        #region ISupportCustomFilters

        public event EventHandler CustomFilter;

        public event EventHandler CustomFiltersReset;

        [Command]
        public void ResetCustomFilters()
        {
            Filter = new FindPersonsInput { MaxResultCount = int.MaxValue };
            RaiseCustomFiltersReset();
        }

        [Command]
        public void NewCustomFilter()
        {
            Filter = new FindPersonsInput {Gender = Gender.F, MaxResultCount = int.MaxValue };
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

        private void ShowDocument<TViewModel>(string documentType, object parameter)
        {
            var document = FindDocument<TViewModel>();
            if (parameter is int)
                document = FindDocument<TViewModel>((int)parameter);
            if (parameter is long)
                document = FindDocument<TViewModel>((long)parameter);
            if (document == null)
                document = DocumentManagerService.CreateDocument(documentType, null, parameter, this);
            else
                ViewModelHelper.EnsureViewModel(document.Content, this, parameter);
            document.Show();
        }
    }
}
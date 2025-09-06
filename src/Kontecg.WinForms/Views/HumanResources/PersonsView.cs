using System;
using System.Windows.Forms;
using DevExpress.Utils.MVVM.Services;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraLayout.Utils;
using Kontecg.Domain;
using Kontecg.Localization;
using Kontecg.HumanResources.Dto;
using Kontecg.Presenters;
using Kontecg.Services;
using Kontecg.ViewModels;
using Kontecg.ViewModels.Persons;

namespace Kontecg.Views.HumanResources
{
    public partial class PersonsView : BaseUserControl, IRibbonOwner
    {
        private PersonDetailView _personDetailView;

        public PersonsView()
            :base(typeof(PersonsCollectionViewModel))
        {
            InitializeComponent();

            layoutView.Appearance.FieldCaption.ForeColor = ColorHelper.DisabledTextColor;
            layoutView.Appearance.FieldCaption.Options.UseForeColor = true;

            CollectionUiViewModel = DevExpress.Mvvm.POCO.ViewModelSource.Create<CollectionUiViewModel>();

            //ViewModel.SelectedEntityChanged += ViewModelOnSelectedEntityChanged;

            BindCommands();

            InitViewKind();
            InitViewLayout();
        }

        public RibbonControl Ribbon => ribbonControl;

        /// <inheritdoc />
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //ViewModel.Reload();
            ViewModel.SelectedEntity = ViewModel.DefaultEntitySelector();

            _personDetailView = ViewModel.IocManager.Resolve<PersonDetailView>();
            ViewModelHelper.EnsureModuleViewModel(_personDetailView, ViewModel, ViewModel.SelectedEntity);
            _personDetailView.Dock = DockStyle.Fill;
            _personDetailView.Parent = pnlView;

            gridView.ExpandMasterRow(0);
            UpdateEntitiesCountRelatedUi();
        }

        protected override void OnDisposing()
        {
            //CollectionPresenter.Dispose();
            base.OnDisposing();
        }

        /// <inheritdoc />
        protected override void LocalizeIsolatedItems()
        {
            base.LocalizeIsolatedItems();
            Ribbon.LocalizeWithSource(LocalizationSource);
            gridControl.LocalizeWithSource(LocalizationSource);
        }

        public PersonsCollectionViewModel ViewModel => GetViewModel<PersonsCollectionViewModel>();

        /// <inheritdoc />
        protected override void OnParentViewModelAttached()
        {
        }

        /// <inheritdoc />
        protected override void OnInitServices()
        {
            Context.RegisterService(this);
            Context.RegisterService(MessageBoxService.CreateXtraMessageBoxService(GuessOwner() ?? this.ParentForm));
            Context.RegisterService("View Settings", new ViewSettingsDialogDocumentManagerService(() => CollectionUiViewModel));
        }

        protected void BindCommands()
        {
            var bindings = MvvmContext.OfType<PersonsCollectionViewModel>();

            bindings.BindCommand(biNew, m => m.NotifyAsync);
            //bindings.BindCommand(biEdit, m => m.Edit);
            //bindings.BindCommand(biDelete, m => m.Delete);
            //bindings.BindCommand(biNewCustomFilter, m => m.NewCustomFilter);
            //bindings.BindCommand(biViewSettings, m => m.ShowViewSettings);
            bindings.SetObjectDataSourceBinding(bindingSource, m => m.FilteredEntities);

            gridView.FocusedRowObjectChanged += (s, e) =>
            {
                ViewModel.SelectedEntity = e.Row as PersonDto;
            };
        }

        private void ViewModelOnSelectedEntityChanged(object sender, EventArgs e)
        {
            if (ViewModel.SelectedEntity != null)
            {
                int rowHandle = gridView.FindRow(ViewModel.SelectedEntity);
                if (rowHandle >= 0)
                    gridView.FocusedRowHandle = rowHandle;
            }
        }

        #region ViewKind

        protected CollectionUiViewModel CollectionUiViewModel { get; private set; }

        private void InitViewKind()
        {
            CollectionUiViewModel.ViewKindChanged += ViewModel_ViewKindChanged;
            bciShowCard.BindCommand(() => CollectionUiViewModel.ShowCard(), CollectionUiViewModel);
            bciShowList.BindCommand(() => CollectionUiViewModel.ShowList(), CollectionUiViewModel);
            bmiShowCard.BindCommand(() => CollectionUiViewModel.ShowCard(), CollectionUiViewModel);
            bmiShowList.BindCommand(() => CollectionUiViewModel.ShowList(), CollectionUiViewModel);
            biResetView.BindCommand(() => CollectionUiViewModel.ResetView(), CollectionUiViewModel);
        }

        private void ViewModel_ViewKindChanged(object sender, System.EventArgs e)
        {
            if (CollectionUiViewModel.ViewKind == CollectionViewKind.CardView)
                gridControl.MainView = layoutView;
            else
            {
                gridControl.MainView = gridView;
                gridView.ExpandMasterRow(0);
            }

            UpdateEntitiesCountRelatedUi();
            GridHelper.SetFindControlImages(gridControl);
        }

        #endregion

        #region ViewLayout

        private void InitViewLayout()
        {
            CollectionUiViewModel.ViewLayoutChanged += ViewModel_ViewLayoutChanged;
            bmiHorizontalLayout.BindCommand(() => CollectionUiViewModel.ShowHorizontalLayout(), CollectionUiViewModel);
            bmiVerticalLayout.BindCommand(() => CollectionUiViewModel.ShowVerticalLayout(), CollectionUiViewModel);
            bmiHideDetail.BindCommand(() => CollectionUiViewModel.HideDetail(), CollectionUiViewModel);
        }

        private void ViewModel_ViewLayoutChanged(object sender, System.EventArgs e)
        {
            bool detailHidden = CollectionUiViewModel.IsDetailHidden;
            splitterItem.Visibility = detailHidden ? LayoutVisibility.Never : LayoutVisibility.Always;
            detailItem.Visibility = detailHidden ? LayoutVisibility.Never : LayoutVisibility.Always;
            if (!detailHidden)
            {
                if (splitterItem.IsVertical != CollectionUiViewModel.IsHorizontalLayout)
                    Root.RotateLayout();
                _personDetailView.IsHorizontalLayout = CollectionUiViewModel.IsHorizontalLayout;
            }
        }

        #endregion

        private void UpdateEntitiesCountRelatedUi()
        {
            var count = ViewModel.TotalCount;
            hiItemsCount.Caption = ViewModel.StatusMessage;
            UpdateAdditionalButtons(count > 0);
        }

        private void UpdateAdditionalButtons(bool hasRecords)
        {
            biReverseSort.Enabled = hasRecords;
            biAddColumns.Enabled = biExpandCollapse.Enabled = hasRecords && (CollectionUiViewModel.ViewKind == CollectionViewKind.ListView);
        }
    }
}

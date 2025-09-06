#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.MVVM;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;
using Kontecg.Application.Services.Dto;
using Kontecg.ViewModels;

namespace Kontecg.Presenters
{
    public abstract class CollectionPresenter<TEntityDto, TPrimaryKey, TService>
        : BasePresenter<CollectionViewModel<TEntityDto, TPrimaryKey, TService>>
        where TEntityDto : EntityDto<TPrimaryKey>
        where TService : class
    {
        private readonly Action<int> _updateUIAction;
        private readonly KontecgViewModelBase _viewModel;
        private GridControl _gridControlCore;
        private GridView _gridViewCore;
        private LayoutView _layoutViewCore;
        private MVVMContext _context;
        private IPropertyBinding _entitiesBinding;
        private IDisposable _loadingTrigger;
        private BarCheckItem _biAddColumns;
        private int _lockFocusedRowChanged = 0;

        protected CollectionPresenter(GridControl gridControl, CollectionViewModel<TEntityDto, TPrimaryKey, TService> viewModel, Action<int> updateUIAction)
            : base(viewModel)
        {
            _gridControlCore = gridControl;
            _updateUIAction = updateUIAction;
            _gridViewCore = gridControl.ViewCollection.FirstOrDefault(view => (view is GridView) && string.IsNullOrEmpty(view.LevelName)) as GridView;
            _layoutViewCore = gridControl.ViewCollection.FirstOrDefault(view => (view is LayoutView) && string.IsNullOrEmpty(view.LevelName)) as LayoutView;

            SubscribeViewModelEvents();
            InitMouseClickBehavior();
            InitFocusedRowSynchronization();

            GridControl.Load += GridControl_Load;
        }

        protected override void OnDisposing()
        {
            GridControl.Load -= GridControl_Load;

            UnsubscribeViewModelEvents();
            ReleaseMouseClickBehavior();
            ReleaseFocusedRowSynchronization();
            _loadingTrigger.Dispose();
            _entitiesBinding.Dispose();

            _gridControlCore = null;
            _layoutViewCore = null;
            _gridViewCore = null;
            base.OnDisposing();
        }

        protected virtual void SubscribeViewModelEvents()
        {
            ViewModel.EntityChanged += ViewModel_EntityChanged;
            ViewModel.EntitiesCountChanged += ViewModel_EntitiesCountChanged;
        }

        protected virtual void UnsubscribeViewModelEvents()
        {
            ViewModel.EntityChanged -= ViewModel_EntityChanged;
            ViewModel.EntitiesCountChanged -= ViewModel_EntitiesCountChanged;
        }

        private void GridControl_Load(object sender, EventArgs e)
        {
            GridHelper.SetFindControlImages(GridControl);
        }

        protected GridControl GridControl => _gridControlCore;

        protected GridView GridView => _gridViewCore;

        protected LayoutView LayoutView => _layoutViewCore;

        protected virtual void SetTopRow()
        {
            if (GridView == null) return;
            GridView.ClearSelection();
            GridView.SelectRow(0);
            GridView.FocusedRowHandle = 0;
            GridView.ExpandMasterRow(0);
        }

        protected void ViewModel_Reload(object sender, System.EventArgs e)
        {
            OnReloadEntities(_context);
            SetTopRow();
        }

        private void ViewModel_EntityChanged(object sender, EntityEventArgs<TPrimaryKey> e)
        {
            if (LayoutView != null)
                LayoutView.InvalidateCardCaption(LayoutView.LocateByValue(GetKeyColumn(), e.Key));
        }

        private void ViewModel_EntitiesCountChanged(object sender, EntitiesCountEventArgs e)
        {
            UpdateEntitiesCountRelatedUI(e.Count);
        }

        public void ReloadEntities(MVVMContext context)
        {
            _context ??= context;
            OnReloadEntities(context);
        }

        private void OnReloadEntities(MVVMContext context)
        {
            if (_entitiesBinding == null)
            {
                _entitiesBinding = context.SetBinding(GridControl, g => g.DataSource, "Entities");
                _loadingTrigger = context.SetTrigger<CollectionViewModel<TEntityDto, TPrimaryKey, TService>, bool>(
                    x => x.IsLoading, (loading) =>
                    {
                        if (loading)
                            GridView.ShowLoadingPanel();
                        else
                        {
                            GridView.HideLoadingPanel();
                            SetTopRow();
                        }
                    });
            }
            else _entitiesBinding.UpdateTarget();
            ((ColumnView)GridControl.MainView).FindFilterText = null;
            ((ColumnView)GridControl.MainView).ActiveFilterString = null;
            UpdateEntitiesCountRelatedUI(ViewModel.Entities.Count);
        }

        private void UpdateEntitiesCountRelatedUI(int count)
        {
            if (_updateUIAction != null) _updateUIAction(count);
        }

        public void ReverseSort(GridColumn gridColumn, LayoutViewColumn layoutViewColumn)
        {
            if (GridControl.MainView == GridView)
                ReverseSort(GridView, gridColumn);
            else
                ReverseSort(LayoutView, layoutViewColumn);
        }

        public void ReverseSort(ColumnView view, GridColumn column)
        {
            if (view.SortInfo[column] == null) return;

            view.SortInfo[column].SortOrder = view.SortInfo[column].SortOrder == ColumnSortOrder.Ascending
                ? ColumnSortOrder.Descending
                : ColumnSortOrder.Ascending;
        }

        public void ExpandCollapseGroups()
        {
            if (GridControl.MainView == GridView)
            {
                if (GridView.GetRowExpanded(-1))
                    GridView.CollapseAllGroups();
                else
                    GridView.ExpandAllGroups();
            }
        }

        public void ExpandCollapseMasterRows()
        {
            if (GridControl.MainView == GridView)
            {
                if (GridView.GetMasterRowExpanded(0))
                    GridView.CollapseMasterRow(0);
                else
                    GridView.ExpandMasterRow(0);
            }
        }

        public void AddColumns(BarCheckItem biAddColumns)
        {
            _biAddColumns = biAddColumns;
            if (GridControl.MainView == GridView)
            {
                if (!biAddColumns.Checked)
                    GridView.HideCustomization();
                else
                {
                    GridView.HideCustomizationForm += GridView_HideCustomizationForm;
                    GridView.ShowCustomization();
                }
            }
        }

        private void GridView_HideCustomizationForm(object sender, System.EventArgs e)
        {
            GridView.HideCustomizationForm -= GridView_HideCustomizationForm;
            if (_biAddColumns != null)
                _biAddColumns.Checked = false;
            _biAddColumns = null;
        }

        #region Focused Row Synchronization

        private void InitFocusedRowSynchronization()
        {
            ViewModel.SelectedEntityChanged += ViewModel_SelectedEntityChanged;
            if (GridView != null)
            {
                GridView.DataController.AllowCurrentRowObjectForGroupRow = false;
                GridView.FocusedRowObjectChanged += ColumnView_FocusedRowObjectChanged;
                GridView.SelectionChanged += View_SelectionChanged;
            }
            if (LayoutView != null)
            {
                LayoutView.FocusedRowObjectChanged += ColumnView_FocusedRowObjectChanged;
                LayoutView.SelectionChanged += View_SelectionChanged;
            }
        }

        private void ReleaseFocusedRowSynchronization()
        {
            ViewModel.SelectedEntityChanged -= ViewModel_SelectedEntityChanged;
            if (GridView != null)
            {
                GridView.FocusedRowObjectChanged -= ColumnView_FocusedRowObjectChanged;
                GridView.SelectionChanged -= View_SelectionChanged;
            }
            if (LayoutView != null)
            {
                LayoutView.FocusedRowObjectChanged -= ColumnView_FocusedRowObjectChanged;
                LayoutView.SelectionChanged -= View_SelectionChanged;
            }
        }

        private void View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var helper = new ColumnViewHelper<TEntityDto, TPrimaryKey, TService>((ColumnView)sender, ViewModel);
            SetSelection(helper.GetSelection());
        }

        private void ColumnView_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
        {
            _lockFocusedRowChanged++;
            try
            {
                ColumnView view = (ColumnView)sender;
                if (view.IsValidRowHandle(e.FocusedRowHandle))
                {
                    if (view.IsDataRow(e.FocusedRowHandle))
                        ViewModel.SelectedEntity = e.Row as TEntityDto;
                    else
                        ViewModel.SelectedEntity = null;
                }
            }
            finally { _lockFocusedRowChanged--; }
        }

        private void ViewModel_SelectedEntityChanged(object sender, System.EventArgs e)
        {
            if (_lockFocusedRowChanged > 0) return;
            if (ViewModel.SelectedEntity == null)
            {
                if (GridView != null)
                    GridView.FocusedRowHandle = GridControl.InvalidRowHandle;
                if (LayoutView != null)
                    LayoutView.FocusedRowHandle = GridControl.InvalidRowHandle;
            }
            else
            {
                if (GridView != null)
                    GridView.FocusedRowHandle = GridView.LocateByValue(GetKeyColumn(), GetKey(ViewModel.SelectedEntity));
                if (LayoutView != null)
                    LayoutView.FocusedRowHandle = LayoutView.LocateByValue(GetKeyColumn(), GetKey(ViewModel.SelectedEntity));
            }
        }

        protected abstract void SetSelection(IEnumerable<TEntityDto> selection);

        protected abstract TPrimaryKey GetKey(TEntityDto entityDto);

        protected virtual string GetKeyColumn()
        {
            return "Id";
        }

        #endregion

        #region Mouse Click Behavior

        protected virtual void InitMouseClickBehavior()
        {
            if (GridView != null)
            {
                GridView.RowClick += GridView_RowClick;
                GridView.PopupMenuShowing += GridView_PopupMenuShowing;
            }

            if (LayoutView != null)
            {
                LayoutView.MouseDown += LayoutView_MouseDown;
                LayoutView.DoubleClick += LayoutView_DoubleClick;
            }
        }

        protected virtual void ReleaseMouseClickBehavior()
        {
            if (GridView != null)
            {
                GridView.RowClick -= GridView_RowClick;
                GridView.PopupMenuShowing -= GridView_PopupMenuShowing;
            }

            if (LayoutView != null)
            {
                LayoutView.MouseDown -= LayoutView_MouseDown;
                LayoutView.DoubleClick -= LayoutView_DoubleClick;
            }
        }

        private void LayoutView_DoubleClick(object sender, System.EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            if (ea.Button != MouseButtons.Left) return;
            var hitInfo = ((LayoutView)sender).CalcHitInfo(ea.Location);
            if (hitInfo.InCard)
            {
                var helper = new ColumnViewHelper<TEntityDto, TPrimaryKey, TService>((ColumnView)sender, ViewModel);
                ea.Handled = helper.EditEntity(hitInfo.RowHandle);
            }
        }

        private void LayoutView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.Clicks != 1) return;
            DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
            var hitInfo = ((LayoutView)sender).CalcHitInfo(ea.Location);
            if (hitInfo.InCard)
            {
                var helper = new ColumnViewHelper<TEntityDto, TPrimaryKey, TService>((ColumnView)sender, ViewModel);
                ea.Handled = helper.ShowEntityMenu(ea.Location, hitInfo.RowHandle);
            }
        }

        private void GridView_RowClick(object sender, RowClickEventArgs e)
        {
            if (e.Clicks == 2 && e.Button == MouseButtons.Left)
            {
                var helper = new ColumnViewHelper<TEntityDto, TPrimaryKey, TService>((ColumnView)sender, ViewModel);
                e.Handled = helper.EditEntity(e.RowHandle);
            }
        }

        private void GridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == GridMenuType.Row && e.HitInfo.InRowCell)
            {
                var helper = new ColumnViewHelper<TEntityDto, TPrimaryKey, TService>((ColumnView)sender, ViewModel);
                helper.PopulateEntityMenu(e.Menu, e.HitInfo.RowHandle);
            }
        }

        #endregion
    }
} 
#endif
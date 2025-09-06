#if false
using DevExpress.XtraGrid.Views.Base;
using Kontecg.ViewModels;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils.Menu;
using Kontecg.Application.Services.Dto;
using Kontecg.Localization;

namespace Kontecg.Presenters
{
    public class ColumnViewHelper<TEntityDto, TPrimaryKey, TService>
        where TEntityDto : EntityDto<TPrimaryKey>
        where TService : class
    {
        private readonly CollectionViewModel<TEntityDto, TPrimaryKey, TService> _viewModel;
        private readonly ColumnView _view;

        public ColumnViewHelper(ColumnView view, CollectionViewModel<TEntityDto, TPrimaryKey, TService> viewModel)
        {
            _view = view;
            _viewModel = viewModel;
        }

        public IEnumerable<TEntityDto> GetSelection()
        {
            int[] rowHandles = _view.GetSelectedRows();
            TEntityDto[] entities = new TEntityDto[rowHandles.Length];
            for (int i = 0; i < entities.Length; i++)
                entities[i] = _view.GetRow(rowHandles[i]) as TEntityDto;
            return entities;
        }

        public void PopulateEntityMenu(DXPopupMenu menu, int rowHandle)
        {
            if (!_view.IsDataRow(rowHandle)) return;
            TEntityDto entity = _view.GetRow(rowHandle) as TEntityDto;
            if (entity != null)
                CreateEntityMenu(menu, entity);
        }

        public bool ShowEntityMenu(Point pt, int rowHandle)
        {
            TEntityDto entity = _view.GetRow(rowHandle) as TEntityDto;
            if (entity != null)
            {
                var rowMenu = new DXPopupMenu();
                CreateEntityMenu(rowMenu, entity);
                MenuManagerHelper.ShowMenu(rowMenu, _view.GridControl.LookAndFeel,
                    _view.GridControl.MenuManager, _view.GridControl, pt);
                return true;
            }
            return false;
        }

        public bool EditEntity(int rowHandle)
        {
            if (!_view.IsDataRow(rowHandle)) return false;
            TEntityDto entity = _view.GetRow(rowHandle) as TEntityDto;
            if (entity != null && _viewModel.CanEdit(entity))
            {
                _viewModel.Edit(entity);
                return true;
            }
            return false;
        }

        public bool IsEntity(int rowHandle)
        {
            if (!_view.IsValidRowHandle(rowHandle)) return false;
            return _view.IsDataRow(rowHandle);
        }

        public bool SelectEntity(int rowHandle)
        {
            if (!_view.IsValidRowHandle(rowHandle)) return false;
            if (_view.IsDataRow(rowHandle))
                _viewModel.SelectedEntity = _view.GetRow(rowHandle) as TEntityDto;
            else
                _viewModel.SelectedEntity = null;
            return true;
        }

        protected void CreateEntityMenu(DXPopupMenu rowMenu, TEntityDto entity)
        {
            var newItem = new DXMenuItem();
            newItem.Caption = LocalizationHelper.GetString(KontecgWinFormsConsts.LocalizationSourceName, "New");
            newItem.BindCommand(() => _viewModel.New(), _viewModel);

            var editItem = new DXMenuItem();
            editItem.Caption = LocalizationHelper.GetString(KontecgWinFormsConsts.LocalizationSourceName, "Edit");
            editItem.BindCommand((ee) => _viewModel.Edit(ee), _viewModel, () => entity);

            var deleteItem = new DXMenuItem();
            deleteItem.Caption = LocalizationHelper.GetString(KontecgWinFormsConsts.LocalizationSourceName, "Delete");
            deleteItem.BindCommand((ee) => _viewModel.Delete(ee), _viewModel, () => entity);

            rowMenu.Items.Add(newItem);
            rowMenu.Items.Add(editItem);
            rowMenu.Items.Add(deleteItem);
        }
    }
} 
#endif
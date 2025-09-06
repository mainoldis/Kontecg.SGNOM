using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Kontecg.Domain;
using Kontecg.ViewModels;
using System;

namespace Kontecg.Views
{
    public class ZoomLevelManager : KontecgCoreDomainServiceBase
    {
        private ZoomTrackBarControl _zoomTrackBarControl;

        private BarEditItem _editItem;
        private BarButtonItem _captionItem;
        private IZoomViewModel _viewModel;
        private ISupportZoom _supportZoomComponent;
        private int _zoomLevel;

        private static readonly int[] ZoomValues = [100, 110, 125, 150, 175, 200, 250, 300, 350, 400, 500];

        public ZoomLevelManager()
        {
            LocalizationSourceName = KontecgWinFormsConsts.LocalizationSourceName;
        }

        public void Initialize(BarEditItem editItem, BarButtonItem captionItem, IZoomViewModel viewModel)
        {
            _editItem = editItem;
            _captionItem = captionItem;
            _viewModel = viewModel;

            if (_viewModel != null)
                _viewModel.ZoomComponentChanged += ViewModel_OnZoomComponentChanged;

            if (_editItem != null)
            {
                _editItem.HiddenEditor += EditItem_OnHiddenEditor;
                _editItem.ShownEditor += EditItem_OnShownEditor;
            }
        }

        private void EditItem_OnShownEditor(object sender, ItemClickEventArgs e)
        {
            _zoomTrackBarControl = _editItem.Manager.ActiveEditor as ZoomTrackBarControl;
            if (ZoomControl != null)
            {
                ZoomControl.ValueChanged += ZoomControl_OnValueChanged;
                ZoomControl_OnValueChanged(ZoomControl, EventArgs.Empty);
            }
        }

        private void EditItem_OnHiddenEditor(object sender, ItemClickEventArgs e)
        {
            ZoomControl.ValueChanged -= ZoomControl_OnValueChanged;
            _zoomTrackBarControl = null;
        }

        private void ZoomControl_OnValueChanged(object sender, EventArgs e)
        {
            int val = ZoomControl.Value * 10;
            if (ZoomControl.Value > 10) val = ZoomValues[ZoomControl.Value - 10];
            ZoomLevel = val;
        }

        private void ViewModel_OnZoomComponentChanged(object sender, EventArgs e)
        {
            if (_supportZoomComponent != null)
                _supportZoomComponent.ZoomChanged -= SupportZoomComponent_OnZoomChanged;

            UpdateZoomLevelFromComponent();

            _supportZoomComponent = _viewModel.ZoomComponent;

            if (_supportZoomComponent != null)
                _supportZoomComponent.ZoomChanged += SupportZoomComponent_OnZoomChanged;
        }

        private void SupportZoomComponent_OnZoomChanged(object sender, EventArgs e)
        {
            UpdateZoomLevelFromComponent();
        }

        public ZoomTrackBarControl ZoomControl => _zoomTrackBarControl;

        public int ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                if (ZoomLevel == value) return;
                _zoomLevel = value;
                OnZoomLevelChanged(value);
            }
        }

        private void OnZoomLevelChanged(int value)
        {
            int index = Array.IndexOf(ZoomValues, value);
            if (index == -1)
                value = (value / 10);
            else
                value = 10 + index;
            _editItem.EditValue = value;
            _captionItem.Caption = $@" {ZoomLevel}%";
            UpdateComponentZoomLevel();
        }

        private void UpdateComponentZoomLevel()
        {
            if (_viewModel.ZoomComponent is { } supportZoom)
                supportZoom.ZoomLevel = ZoomLevel;
        }

        private void UpdateZoomLevelFromComponent()
        {
            ISupportZoom supportZoom = _viewModel.ZoomComponent;
            if (supportZoom != null)
                ZoomLevel = supportZoom.ZoomLevel;
            _editItem.Visibility = _captionItem.Visibility =
                (supportZoom != null) ? BarItemVisibility.Always : BarItemVisibility.Never;
        }
    }
}

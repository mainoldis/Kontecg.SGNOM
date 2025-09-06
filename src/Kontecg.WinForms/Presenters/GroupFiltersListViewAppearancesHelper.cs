using DevExpress.XtraGrid.Views.WinExplorer;

namespace Kontecg.Presenters
{
    public static class GroupFiltersListViewAppearancesHelper
    {
        public static void Apply(WinExplorerView winExplorerView)
        {
            winExplorerView.Appearance.ItemDescriptionNormal.ForeColor = ColorHelper.DisabledTextColor;
            winExplorerView.Appearance.ItemDescriptionNormal.Options.UseForeColor = true;
            winExplorerView.Appearance.ItemDescriptionHovered.ForeColor = ColorHelper.DisabledTextColor;
            winExplorerView.Appearance.ItemDescriptionHovered.Options.UseForeColor = true;
            winExplorerView.Appearance.ItemDescriptionPressed.ForeColor = ColorHelper.DisabledTextColor;
            winExplorerView.Appearance.ItemDescriptionPressed.Options.UseForeColor = true;
            winExplorerView.Appearance.ItemDescriptionSelected.ForeColor = ColorHelper.DisabledTextColor;
            winExplorerView.Appearance.ItemDescriptionSelected.Options.UseForeColor = true;
        }
    }
}

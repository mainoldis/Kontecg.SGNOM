using System;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraTreeList;

namespace Kontecg.Presenters
{
    public static class FiltersTreeListAppearancesHelper
    {
        public static void Apply(TreeList treeList)
        {
            treeList.BackColor = System.Drawing.Color.Transparent;
            treeList.Appearance.Empty.BackColor = System.Drawing.Color.Transparent;
            treeList.Appearance.Empty.Options.UseBackColor = true;
            treeList.Appearance.Row.BackColor = System.Drawing.Color.Transparent;
            treeList.Appearance.Row.Options.UseBackColor = true;
            treeList.LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;

            var font = FontHelper.GetFont(treeList.Font.FontFamily.Name, treeList.Font.Size, FontStyle.Bold);
            treeList.Appearance.FocusedRow.Font = font;
            treeList.Appearance.FocusedRow.Options.UseFont = true;
            treeList.Appearance.HideSelectionRow.Font = font;
            treeList.Appearance.HideSelectionRow.Options.UseFont = true;
            treeList.Appearance.SelectedRow.Font = font;
            treeList.Appearance.SelectedRow.Options.UseFont = true;
        }

        static void LookAndFeel_StyleChanged(object sender, EventArgs e)
        {
            var lf = (UserLookAndFeel)sender;
            var treeList = lf?.OwnerControl as TreeList;

            if (treeList != null)
                treeList.Appearance.Row.ForeColor = GridHelper.GetTransparentRowForeColor(lf);
        }
    }
}

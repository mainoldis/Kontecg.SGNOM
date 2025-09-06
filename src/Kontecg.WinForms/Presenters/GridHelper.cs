using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Kontecg.Presenters
{
    public static class GridHelper
    {
        public static void GridViewFocusObject(ColumnView cView, object obj)
        {
            if (obj == null) return;
            int oldFocusedRowHandle = cView.FocusedRowHandle;
            for (int i = 0; i < cView.DataRowCount; ++i)
            {
                object rowObj = cView.GetRow(i) as object;
                if (rowObj == null) continue;
                if (ReferenceEquals(obj, rowObj))
                {
                    if (i == oldFocusedRowHandle)
                        cView.FocusedRowHandle = GridControl.InvalidRowHandle;
                    cView.FocusedRowHandle = i;
                    break;
                }
            }
        }

        public static void SetFindControlImages(GridControl grid)
        {
            FindControl findControl = null;
            foreach (Control ctrl in grid.Controls)
            {
                findControl = ctrl as FindControl;
                if (findControl != null) break;
            }

            if (findControl != null)
            {
                findControl.SuspendLayout();
                findControl.FindEdit.Properties.BeginUpdate();

                //findControl.FindButton.ImageOptions.SvgImage = global::DevExpress.ProductsDemo.Win.Properties.Resources.Search1;
                //findControl.FindButton.ImageOptions.SvgImageSize = new Size(16, 16);
                //findControl.ClearButton.ImageOptions.SvgImage = global::DevExpress.ProductsDemo.Win.Properties.Resources.Delete;
                //findControl.ClearButton.ImageOptions.SvgImageSize = new Size(16, 16);
                //findControl.CalcButtonsBestFit();

                EditorButton btn = findControl.FindEdit.Properties.Buttons[0];
                btn.Kind = ButtonPredefines.Search;
                btn = new ClearButton();
                btn.Visible = false;
                findControl.FindEdit.Properties.Buttons.Add(btn);
                findControl.FindEdit.ButtonClick += (s, e) =>
                {
                    if (e.Button is ClearButton)
                    {
                        ButtonEdit edit = s as ButtonEdit;
                        edit.Text = string.Empty;
                    }
                };
                findControl.FindEdit.Properties.EndUpdate();
                findControl.FindEdit.EditValueChanged += (s, e) =>
                {
                    findControl.SuspendLayout();
                    MRUEdit edit = s as MRUEdit;
                    edit.Properties.BeginUpdate();
                    try
                    {
                        edit.Properties.Buttons[0].Visible = string.IsNullOrEmpty(edit.Text);
                        edit.Properties.Buttons[1].Visible = !string.IsNullOrEmpty(edit.Text);
                    }
                    finally
                    {
                        edit.Properties.EndUpdate();
                    }

                    findControl.ResumeLayout(false);
                };
                findControl.ResumeLayout(false);
            }
        }

        public static void HideCustomization(Control control)
        {
            if (control == null) return;
            foreach (Control child in control.Controls)
            {
                GridControl grid = child as GridControl;
                if (grid != null)
                {
                    GridView gridView = grid.MainView as GridView;
                    gridView?.HideCustomization();
                    continue;
                }

                HideCustomization(child);
            }
        }

        internal static Color GetTransparentRowForeColor(UserLookAndFeel lf)
        {
            return lf.ActiveSkinName == "VS2010" ? ColorHelper.GetHeaderForeColor(lf) : SystemColors.ControlText;
        }
    }
}

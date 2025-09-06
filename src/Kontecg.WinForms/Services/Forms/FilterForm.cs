using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using Kontecg.Runtime;
using Kontecg.Views;

namespace Kontecg.Services.Forms
{
    public partial class FilterForm : BaseForm, IDXMenuManagerProvider
    {
        public FilterForm()
        {
            InitializeComponent();
            IconOptions.SvgImage = AppHelper.AppIcon;
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ScreenManager.SetFormToCurrentScreen(this);
            Bounds = PlacementHelper.Arrange(Size, Owner?.Bounds ?? ScreenManager.CurrentScreen.Bounds, System.Drawing.ContentAlignment.MiddleCenter);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is UserControl)
            {
                AcceptButton = Find(e.Control, (btn) => L(btn.Text) == L("OK"));
                CancelButton = Find(e.Control, (btn) => L(btn.Text) == L("Cancel"));
            }
        }

        private IButtonControl Find(Control control, Predicate<Control> predicate)
        {
            foreach (Control child in control.Controls)
            {
                if (child is IButtonControl buttonControl && predicate(child))
                    return buttonControl;
                IButtonControl nested = Find(child, predicate);
                if (nested != null)
                    return nested;
            }

            return null;
        }

        IDXMenuManager IDXMenuManagerProvider.MenuManager => AppHelper.MainForm.Ribbon.Manager;
    }
}

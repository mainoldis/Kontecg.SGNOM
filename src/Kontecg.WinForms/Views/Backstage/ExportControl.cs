using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraPrinting;
using Kontecg.Runtime;

namespace Kontecg.Views.Backstage
{
    public partial class ExportControl : BaseUserControl
    {
        private readonly DXPopupMenu _menuExport;

        public ExportControl()
        {
            InitializeComponent();
            SelectedExport = ExportTarget.Pdf;
            _menuExport = new DXPopupMenu();

            AddExportTarget(ExportTarget.Pdf);
            AddExportTarget(ExportTarget.Html);
            AddExportTarget(ExportTarget.Mht);
            AddExportTarget(ExportTarget.Rtf);
            AddExportTarget(ExportTarget.Xls);
            AddExportTarget(ExportTarget.Xlsx);
            AddExportTarget(ExportTarget.Csv);
            AddExportTarget(ExportTarget.Text);
            AddExportTarget(ExportTarget.Image);

            btnExport.DropDownControl = _menuExport;
            _menuExport.BeforePopup += MenuExport_BeforePopup;
            moduleLayout.BackColor =
                DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
            LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
        }

        void LookAndFeel_StyleChanged(object sender, EventArgs e)
        {
            moduleLayout.BackColor =
                DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            btnExport.MenuManager = MenuManagerHelper.FindMenuManager(AppHelper.MainForm);
        }

        private void MenuExport_BeforePopup(object sender, EventArgs e)
        {
            foreach (DXMenuCheckItem item in _menuExport.Items)
                item.Checked = object.Equals(item.Tag, SelectedExport);
        }

        private void AddExportTarget(ExportTarget target)
        {
            var exportItem = new DXMenuCheckItem()
            {
                Caption = target.ToString(),
                Tag = target
            };
            _menuExport.Items.Add(exportItem);
            exportItem.Click += ExportItem_Click;
        }

        private void ExportItem_Click(object sender, EventArgs e)
        {
            SelectedExport = (ExportTarget) ((DXMenuItem) sender).Tag;
        }

        public void SetSettings(Control control)
        {
            for (int i = settingsPanel.Controls.Count - 1; i >= 0; i--)
                settingsPanel.Controls[i].Dispose();
            if (control != null)
            {
                control.Dock = DockStyle.Fill;
                control.Parent = settingsPanel;
            }
        }

        public bool ExportEnabled
        {
            set => btnExport.Enabled = value;
        }

        public ExportTarget SelectedExport { get; set; }

        public event EventHandler ExportClick;

        private void RaiseExportClick()
        {
            ExportClick?.Invoke(this, EventArgs.Empty);
        }

        private void Export_Click(object sender, EventArgs e)
        {
            RaiseExportClick();
        }
    }
}
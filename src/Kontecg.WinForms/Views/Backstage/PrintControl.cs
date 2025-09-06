using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Printing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Preview;
using DevExpress.XtraLayout.Utils;

namespace Kontecg.Views.Backstage
{
    public partial class PrintControl : BaseUserControl
    {
        private PrinterImagesContainer _imagesContainer;
        private PrinterItemContainer _printerItemContainer;

        public PrintControl()
        {
            InitializeComponent();
            _imagesContainer = new PrinterImagesContainer();

            CreatePrinterItemContainer();

            moduleLayout.BackColor =
                DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);

            LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
        }

        private void CreatePrinterItemContainer()
        {
            try
            {
                _printerItemContainer = new PrinterItemContainer();
            }
            catch
            {
            }
        }

        private void LookAndFeel_StyleChanged(object sender, EventArgs e)
        {
            moduleLayout.BackColor =
                DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbPrinters.Properties.LargeImages = _imagesContainer.LargeImages;
            cbPrinters.Properties.SmallImages = _imagesContainer.SmallImages;
            if (_printerItemContainer != null)
            {
                foreach (PrinterItem item in _printerItemContainer.Items)
                    cbPrinters.Properties.Items.Add(new ImageComboBoxItem(item.DisplayName, item,
                        _imagesContainer.GetImageIndex(item)));
            }
        }

        public bool PrintEnabled
        {
            set => btnOptions.Enabled = btnPrint.Enabled = value;
        }

        public bool SettingsVisible
        {
            set => ItemForSettings.Visibility = value ? LayoutVisibility.Always : LayoutVisibility.Never;
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

        public string SelectedPrinterName
        {
            get
            {
                if (cbPrinters.EditValue is PrinterItem item)
                    return item.FullName;
                return string.Empty;
            }
            set => cbPrinters.EditValue = FindPrinterItem(value);
        }

        PrinterItem FindPrinterItem(string value)
        {
            if (_printerItemContainer != null)
            {
                for (int i = 0; i < _printerItemContainer.Items.Count; i++)
                {
                    if (_printerItemContainer.Items[i].FullName != value) continue;
                    return _printerItemContainer.Items[i];
                }
            }

            return null;
        }

        public event EventHandler PrintClick;
        public event EventHandler PrintOptionsClick;

        private void RaisePrintOptionsClick()
        {
            PrintOptionsClick?.Invoke(this, EventArgs.Empty);
        }

        private void RaisePrintClick()
        {
            PrintClick?.Invoke(this, EventArgs.Empty);
        }

        private void Print_Click(object sender, EventArgs e)
        {
            RaisePrintClick();
        }

        private void Options_Click(object sender, EventArgs e)
        {
            RaisePrintOptionsClick();
        }
    }
}
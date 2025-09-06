using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.Preview;

namespace Kontecg.Views.Backstage
{
    public partial class PreviewControl : BaseUserControl
    {
        public PreviewControl()
        {
            InitializeComponent();
        }

        public DocumentViewer DocumentViewer => documentViewerCore;

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            DocumentViewer.BackColor =
                DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
            LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
        }

        private void LookAndFeel_StyleChanged(object sender, EventArgs e)
        {
            DocumentViewer.BackColor =
                DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public object DocumentSource
        {
            get => documentViewerCore.DocumentSource;
            set
            {
                if (!ReferenceEquals(documentViewerCore.DocumentSource, value))
                {
                    documentViewerCore.DocumentSource = value;
                }
            }
        }
    }
}
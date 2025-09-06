using System;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;
using DevExpress.XtraBars.Ribbon;
using Kontecg.Domain;

namespace Kontecg.Views.Dashboard
{
    public partial class DashboardDesignerView : BaseUserControl, IRibbonOwner
    {
        public DashboardDesignerView()
        {
            InitializeComponent();
            dashboardDesigner.CreateRibbon();
            dashboardDesigner.AsyncDataLoading += DashboardDesignerOnAsyncDataLoading;
            Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if(DesignMode) return;

            CreateRibbonMenu();
        }

        private void DashboardDesignerOnAsyncDataLoading(object sender, DataLoadingEventArgs e)
        {
            //TODO: Asignar el datasource
        }

        public void AttachDashboardForEdit(DevExpress.DashboardCommon.Dashboard dashboard)
        {
            dashboardDesigner.Dashboard = dashboard;
        }

        public void DeAttachDashboardDesigner()
        {
            dashboardDesigner.Dashboard = null;
        }

        protected void OnClosing(CancelEventArgs e)
        {
            if (dashboardDesigner.IsDashboardModified)
            {
                DialogResult result = XtraMessageBox.Show(LookAndFeel, this, L("ConfirmSaveQuestion"), L("DashboardDesigner"),
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                    e.Cancel = true;
                else
                    SaveDashboard = result == DialogResult.Yes;
            }
        }

        public DevExpress.DashboardCommon.Dashboard Dashboard => dashboardDesigner.Dashboard;

        public bool SaveDashboard { get; private set; }

        /// <inheritdoc />
        public RibbonControl Ribbon => dashboardDesigner.Ribbon;

        protected virtual void CreateRibbonMenu()
        {
            var homepage = Ribbon.GetDashboardRibbonPage(DashboardBarItemCategory.None, DashboardRibbonPage.Home);
            RibbonPageGroup fileRibbonPageGroup = homepage.Groups[0];
            fileRibbonPageGroup.Enabled = false;
            Ribbon.Toolbar.ItemLinks.RemoveAt(0);
            Control backstageViewControl = Ribbon.ApplicationButtonDropDownControl as Control;
            if (backstageViewControl != null)
                backstageViewControl.Enabled = false;
        }
    }
}

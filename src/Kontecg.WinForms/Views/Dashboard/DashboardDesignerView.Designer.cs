namespace Kontecg.Views.Dashboard
{
    partial class DashboardDesignerView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dashboardDesigner = new DevExpress.DashboardWin.DashboardDesigner();
            ((System.ComponentModel.ISupportInitialize)dashboardDesigner).BeginInit();
            SuspendLayout();
            // 
            // dashboardDesigner
            // 
            dashboardDesigner.ActionOnClose = DevExpress.DashboardWin.DashboardActionOnClose.Discard;
            dashboardDesigner.AllowInspectAggregatedData = true;
            dashboardDesigner.AllowInspectRawData = true;
            dashboardDesigner.AsyncMode = true;
            dashboardDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            dashboardDesigner.Location = new System.Drawing.Point(0, 0);
            dashboardDesigner.Name = "dashboardDesigner";
            dashboardDesigner.Size = new System.Drawing.Size(740, 519);
            dashboardDesigner.TabIndex = 0;
            dashboardDesigner.UseNeutralFilterMode = true;
            // 
            // DashboardDesignerView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(740, 519);
            Controls.Add(dashboardDesigner);
            Name = "DashboardDesignerView";
            ((System.ComponentModel.ISupportInitialize)dashboardDesigner).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.DashboardWin.DashboardDesigner dashboardDesigner;
    }
}

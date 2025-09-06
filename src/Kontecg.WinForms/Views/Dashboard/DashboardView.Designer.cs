namespace Kontecg.Views.Dashboard
{
    partial class DashboardView
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
            components = new System.ComponentModel.Container();
            dashboardViewer = new DevExpress.DashboardWin.DashboardViewer(components);
            ((System.ComponentModel.ISupportInitialize)dashboardViewer).BeginInit();
            SuspendLayout();
            // 
            // dashboardViewer
            // 
            dashboardViewer.AllowInspectAggregatedData = true;
            dashboardViewer.AllowInspectRawData = true;
            dashboardViewer.Appearance.BackColor = System.Drawing.Color.FromArgb(210, 210, 210);
            dashboardViewer.Appearance.Options.UseBackColor = true;
            dashboardViewer.AsyncMode = true;
            dashboardViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            dashboardViewer.Location = new System.Drawing.Point(0, 0);
            dashboardViewer.Name = "dashboardViewer";
            dashboardViewer.Size = new System.Drawing.Size(640, 480);
            dashboardViewer.TabIndex = 0;
            // 
            // DashboardView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(dashboardViewer);
            Name = "DashboardView";
            Size = new System.Drawing.Size(640, 480);
            ((System.ComponentModel.ISupportInitialize)dashboardViewer).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.DashboardWin.DashboardViewer dashboardViewer;
    }
}

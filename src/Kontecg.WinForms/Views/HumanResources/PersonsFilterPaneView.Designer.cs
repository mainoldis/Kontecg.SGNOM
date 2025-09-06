namespace Kontecg.Views.HumanResources
{
    partial class PersonsFilterPaneView
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
            moduleLayout = new DevExpress.XtraLayout.LayoutControl();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)MvvmContext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            SuspendLayout();
            // 
            // moduleLayout
            // 
            moduleLayout.AllowCustomization = false;
            moduleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            moduleLayout.Location = new System.Drawing.Point(0, 0);
            moduleLayout.Name = "moduleLayout";
            moduleLayout.Root = Root;
            moduleLayout.Size = new System.Drawing.Size(196, 613);
            moduleLayout.TabIndex = 0;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Name = "Root";
            Root.OptionsItemText.TextToControlDistance = 6;
            Root.Size = new System.Drawing.Size(196, 613);
            Root.TextVisible = false;
            // 
            // PersonsFilterPaneView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(moduleLayout);
            Name = "PersonsFilterPaneView";
            Size = new System.Drawing.Size(196, 613);
            ((System.ComponentModel.ISupportInitialize)MvvmContext).EndInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl moduleLayout;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
    }
}

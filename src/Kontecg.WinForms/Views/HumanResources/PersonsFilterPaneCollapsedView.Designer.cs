namespace Kontecg.Views.HumanResources
{
    partial class PersonsFilterPaneCollapsedView
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
            btnNew = new DevExpress.XtraEditors.SimpleButton();
            btnNewLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)MvvmContext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).BeginInit();
            moduleLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnNewLayoutControlItem).BeginInit();
            SuspendLayout();
            // 
            // moduleLayout
            // 
            moduleLayout.AllowCustomization = false;
            moduleLayout.Controls.Add(btnNew);
            moduleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            moduleLayout.Location = new System.Drawing.Point(0, 0);
            moduleLayout.Name = "moduleLayout";
            moduleLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2024, 360, 650, 400);
            moduleLayout.Root = Root;
            moduleLayout.Size = new System.Drawing.Size(60, 600);
            moduleLayout.TabIndex = 1;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { btnNewLayoutControlItem });
            Root.Name = "Root";
            Root.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 10, 10);
            Root.Size = new System.Drawing.Size(60, 600);
            Root.TextVisible = false;
            // 
            // btnNew
            // 
            btnNew.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.False;
            btnNew.ImageOptions.ImageUri.Uri = "outlook%20inspired/newcustomer";
            btnNew.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            btnNew.Location = new System.Drawing.Point(12, 12);
            btnNew.Name = "btnNew";
            btnNew.Size = new System.Drawing.Size(46, 36);
            btnNew.StyleController = moduleLayout;
            btnNew.TabIndex = 2;
            // 
            // btnNewLayoutControlItem
            // 
            btnNewLayoutControlItem.Control = btnNew;
            btnNewLayoutControlItem.CustomizationFormText = "New";
            btnNewLayoutControlItem.Location = new System.Drawing.Point(0, 0);
            btnNewLayoutControlItem.Name = "btnNewLayoutControlItem";
            btnNewLayoutControlItem.Size = new System.Drawing.Size(50, 580);
            btnNewLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            btnNewLayoutControlItem.TextVisible = false;
            btnNewLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // PersonsFilterPaneCollapsedView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(moduleLayout);
            Name = "PersonsFilterPaneCollapsedView";
            Size = new System.Drawing.Size(60, 600);
            ((System.ComponentModel.ISupportInitialize)MvvmContext).EndInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).EndInit();
            moduleLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnNewLayoutControlItem).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl moduleLayout;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraLayout.LayoutControlItem btnNewLayoutControlItem;
    }
}

namespace Kontecg.Views.Account
{
    partial class ProfileControl
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
            pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)MvvmContext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).BeginInit();
            moduleLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            SuspendLayout();
            // 
            // moduleLayout
            // 
            moduleLayout.Controls.Add(pictureEdit1);
            moduleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            moduleLayout.Location = new System.Drawing.Point(0, 0);
            moduleLayout.Name = "moduleLayout";
            moduleLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2205, 0, 650, 400);
            moduleLayout.OptionsView.AllowGlyphSkinning = true;
            moduleLayout.Root = Root;
            moduleLayout.Size = new System.Drawing.Size(238, 232);
            moduleLayout.TabIndex = 0;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(238, 232);
            Root.TextVisible = false;
            // 
            // pictureEdit1
            // 
            pictureEdit1.Location = new System.Drawing.Point(117, 12);
            pictureEdit1.Name = "pictureEdit1";
            pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            pictureEdit1.Size = new System.Drawing.Size(109, 208);
            pictureEdit1.StyleController = moduleLayout;
            pictureEdit1.TabIndex = 4;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = pictureEdit1;
            layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new System.Drawing.Size(218, 212);
            layoutControlItem1.TextSize = new System.Drawing.Size(93, 13);
            // 
            // ProfileControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(moduleLayout);
            Name = "ProfileControl";
            Size = new System.Drawing.Size(238, 232);
            ((System.ComponentModel.ISupportInitialize)MvvmContext).EndInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).EndInit();
            moduleLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl moduleLayout;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}

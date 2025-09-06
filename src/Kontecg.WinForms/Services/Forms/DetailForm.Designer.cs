namespace Kontecg.Services.Forms {
    partial class DetailForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
            biAbout = new DevExpress.XtraBars.BarButtonItem();
            rpInicio = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgInformacion = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            ((System.ComponentModel.ISupportInitialize)ribbonControl).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl
            // 
            ribbonControl.ExpandCollapseItem.Id = 0;
            ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl.ExpandCollapseItem, biAbout });
            ribbonControl.Location = new System.Drawing.Point(0, 0);
            ribbonControl.MaxItemId = 5;
            ribbonControl.Name = "ribbonControl";
            ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { rpInicio });
            ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
            ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl.Size = new System.Drawing.Size(1004, 158);
            ribbonControl.StatusBar = ribbonStatusBar;
            ribbonControl.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.True;
            // 
            // biAbout
            // 
            biAbout.Caption = "About";
            biAbout.Id = 4;
            biAbout.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.False;
            biAbout.ImageOptions.ImageUri.Uri = "outlook%20inspired/about";
            biAbout.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F1);
            biAbout.MergeOrder = 99;
            biAbout.Name = "biAbout";
            // 
            // rpInicio
            // 
            rpInicio.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgInformacion });
            rpInicio.Name = "rpInicio";
            rpInicio.Text = "RibbonGroupHome";
            // 
            // rpgInformacion
            // 
            rpgInformacion.AllowTextClipping = false;
            rpgInformacion.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgInformacion.ItemLinks.Add(biAbout);
            rpgInformacion.MergeOrder = 1;
            rpgInformacion.Name = "rpgInformacion";
            rpgInformacion.Text = "Info";
            // 
            // ribbonStatusBar
            // 
            ribbonStatusBar.Location = new System.Drawing.Point(0, 774);
            ribbonStatusBar.Name = "ribbonStatusBar";
            ribbonStatusBar.Ribbon = ribbonControl;
            ribbonStatusBar.Size = new System.Drawing.Size(1004, 24);
            // 
            // DetailForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1004, 798);
            Controls.Add(ribbonStatusBar);
            Controls.Add(ribbonControl);
            FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            Name = "DetailForm";
            Ribbon = ribbonControl;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            StatusBar = ribbonStatusBar;
            Text = "";
            ((System.ComponentModel.ISupportInitialize)ribbonControl).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpInicio;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgInformacion;
        private DevExpress.XtraBars.BarButtonItem biAbout;
    }
}

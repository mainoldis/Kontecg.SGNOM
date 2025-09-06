using System;
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraSplashScreen;
using Kontecg.Dependency;
using Kontecg.Resources.Embedded;

namespace Kontecg.Views
{
    public class KontecgSplashScreen : SplashScreen
    {
        public enum UpdateSplashCommand
        {
            Caption,
            Description,
            AdditionalParams,
        }

        public KontecgSplashScreen()
        {
            InitializeComponent();
            CopyrightDataUpdate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var embeddedResourceManager = IocManager.Instance.Resolve<IEmbeddedResourceManager>();

            this.peLogoBrand.EditValue = embeddedResourceManager.GetResource(KontecgWinFormsConsts.ResourcesNames.BrandCompany)?.Content;
            this.peLogo.EditValue = embeddedResourceManager.GetResource(KontecgWinFormsConsts.ResourcesNames.BrandLogo)?.Content;
        }

        protected override void DrawContent(GraphicsCache graphicsCache, Skin skin)
        {
            Rectangle bounds = ClientRectangle;
            bounds.Width--; bounds.Height--;
            graphicsCache.Graphics.DrawRectangle(graphicsCache.GetPen(Color.FromArgb(255, 87, 87, 87), 1), bounds);
        }

        protected virtual void CopyrightDataUpdate()
        {
            lblCopyright.Text = $"Copyright © 2013 - { DateTime.Now.Year} ECG.\r\nTodos los derechos reservados";
        }

        /// <inheritdoc />
        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
            if ((UpdateSplashCommand)cmd == UpdateSplashCommand.Description)
            {
                lblStatus.Text = (string)arg;
            }
        }

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblCopyright = new DevExpress.XtraEditors.LabelControl();
            lblStatus = new DevExpress.XtraEditors.LabelControl();
            marqueeProgressBarControl = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            peLogo = new DevExpress.XtraEditors.PictureEdit();
            peLogoBrand = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)marqueeProgressBarControl.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)peLogo.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)peLogoBrand.Properties).BeginInit();
            SuspendLayout();
            // 
            // lblCopyright
            // 
            lblCopyright.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            lblCopyright.Location = new Point(24, 287);
            lblCopyright.Name = "lblCopyright";
            lblCopyright.Size = new Size(47, 13);
            lblCopyright.TabIndex = 6;
            lblCopyright.Text = "Copyright";
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(24, 207);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(108, 13);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Cargando aplicación...";
            // 
            // marqueeProgressBarControl
            // 
            marqueeProgressBarControl.EditValue = 0;
            marqueeProgressBarControl.Location = new Point(23, 231);
            marqueeProgressBarControl.Name = "marqueeProgressBarControl";
            marqueeProgressBarControl.Properties.EndColor = Color.FromArgb(191, 8, 17);
            marqueeProgressBarControl.Properties.StartColor = Color.FromArgb(191, 8, 17);
            marqueeProgressBarControl.Size = new Size(404, 12);
            marqueeProgressBarControl.TabIndex = 5;
            // 
            // peLogo
            // 
            peLogo.Location = new Point(12, 12);
            peLogo.Name = "peLogo";
            peLogo.Properties.AllowFocused = false;
            peLogo.Properties.Appearance.BackColor = Color.Transparent;
            peLogo.Properties.Appearance.Options.UseBackColor = true;
            peLogo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            peLogo.Properties.ShowMenu = false;
            peLogo.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            peLogo.Size = new Size(426, 180);
            peLogo.TabIndex = 9;
            // 
            // peLogoBrand
            // 
            peLogoBrand.Location = new Point(278, 266);
            peLogoBrand.Name = "peLogoBrand";
            peLogoBrand.Properties.AllowFocused = false;
            peLogoBrand.Properties.Appearance.BackColor = Color.Transparent;
            peLogoBrand.Properties.Appearance.Options.UseBackColor = true;
            peLogoBrand.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            peLogoBrand.Properties.ShowMenu = false;
            peLogoBrand.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            peLogoBrand.Size = new Size(160, 48);
            peLogoBrand.TabIndex = 8;
            // 
            // KontecgSplashScreen
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new Size(450, 320);
            Controls.Add(peLogo);
            Controls.Add(peLogoBrand);
            Controls.Add(lblStatus);
            Controls.Add(lblCopyright);
            Controls.Add(marqueeProgressBarControl);
            DoubleBuffered = true;
            Name = "KontecgSplashScreen";
            Padding = new System.Windows.Forms.Padding(1);
            ((System.ComponentModel.ISupportInitialize)marqueeProgressBarControl.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)peLogo.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)peLogoBrand.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraEditors.LabelControl lblCopyright;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.PictureEdit peLogoBrand;
        private DevExpress.XtraEditors.PictureEdit peLogo;
        private DevExpress.XtraEditors.MarqueeProgressBarControl marqueeProgressBarControl;
    }
}

namespace Kontecg.Views.Account
{
    partial class LoginView
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
            txtUsernameOrEmail = new DevExpress.XtraEditors.TextEdit();
            txtPassword = new DevExpress.XtraEditors.TextEdit();
            btnSignIn = new DevExpress.XtraEditors.SimpleButton();
            cboDomainName = new DevExpress.XtraEditors.ComboBoxEdit();
            chkRememberMe = new DevExpress.XtraEditors.CheckEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            lciUsernameOrEmail = new DevExpress.XtraLayout.LayoutControlItem();
            lciPassword = new DevExpress.XtraLayout.LayoutControlItem();
            lciDomainName = new DevExpress.XtraLayout.LayoutControlItem();
            lciCommand = new DevExpress.XtraLayout.LayoutControlItem();
            lciRememberMe = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)MvvmContext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).BeginInit();
            moduleLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtUsernameOrEmail.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtPassword.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboDomainName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkRememberMe.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciUsernameOrEmail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciPassword).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciDomainName).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciCommand).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciRememberMe).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            SuspendLayout();
            // 
            // moduleLayout
            // 
            moduleLayout.Controls.Add(txtUsernameOrEmail);
            moduleLayout.Controls.Add(txtPassword);
            moduleLayout.Controls.Add(btnSignIn);
            moduleLayout.Controls.Add(cboDomainName);
            moduleLayout.Controls.Add(chkRememberMe);
            moduleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            moduleLayout.Location = new System.Drawing.Point(0, 0);
            moduleLayout.Name = "moduleLayout";
            moduleLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2307, 0, 650, 400);
            moduleLayout.Root = Root;
            moduleLayout.Size = new System.Drawing.Size(346, 163);
            moduleLayout.TabIndex = 0;
            // 
            // txtUsernameOrEmail
            // 
            txtUsernameOrEmail.Location = new System.Drawing.Point(109, 12);
            txtUsernameOrEmail.Name = "txtUsernameOrEmail";
            txtUsernameOrEmail.Size = new System.Drawing.Size(225, 20);
            txtUsernameOrEmail.StyleController = moduleLayout;
            txtUsernameOrEmail.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.Location = new System.Drawing.Point(109, 36);
            txtPassword.Name = "txtPassword";
            txtPassword.Properties.UseSystemPasswordChar = true;
            txtPassword.Size = new System.Drawing.Size(225, 20);
            txtPassword.StyleController = moduleLayout;
            txtPassword.TabIndex = 2;
            // 
            // btnSignIn
            // 
            btnSignIn.Location = new System.Drawing.Point(230, 108);
            btnSignIn.Name = "btnSignIn";
            btnSignIn.Size = new System.Drawing.Size(104, 22);
            btnSignIn.StyleController = moduleLayout;
            btnSignIn.TabIndex = 5;
            btnSignIn.Text = "SignIn";
            // 
            // cboDomainName
            // 
            cboDomainName.Location = new System.Drawing.Point(109, 60);
            cboDomainName.Name = "cboDomainName";
            cboDomainName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboDomainName.Size = new System.Drawing.Size(225, 20);
            cboDomainName.StyleController = moduleLayout;
            cboDomainName.TabIndex = 3;
            // 
            // chkRememberMe
            // 
            chkRememberMe.Location = new System.Drawing.Point(12, 84);
            chkRememberMe.Name = "chkRememberMe";
            chkRememberMe.Properties.Caption = "RememberMe";
            chkRememberMe.Size = new System.Drawing.Size(322, 20);
            chkRememberMe.StyleController = moduleLayout;
            chkRememberMe.TabIndex = 4;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciUsernameOrEmail, lciPassword, lciDomainName, lciCommand, lciRememberMe, emptySpaceItem1 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(346, 163);
            Root.TextVisible = false;
            // 
            // lciUsernameOrEmail
            // 
            lciUsernameOrEmail.Control = txtUsernameOrEmail;
            lciUsernameOrEmail.Location = new System.Drawing.Point(0, 0);
            lciUsernameOrEmail.Name = "lciUsernameOrEmail";
            lciUsernameOrEmail.Size = new System.Drawing.Size(326, 24);
            lciUsernameOrEmail.Text = "UserNameOrEmail";
            lciUsernameOrEmail.TextSize = new System.Drawing.Size(85, 13);
            // 
            // lciPassword
            // 
            lciPassword.Control = txtPassword;
            lciPassword.Location = new System.Drawing.Point(0, 24);
            lciPassword.Name = "lciPassword";
            lciPassword.Size = new System.Drawing.Size(326, 24);
            lciPassword.Text = "Password";
            lciPassword.TextSize = new System.Drawing.Size(85, 13);
            // 
            // lciDomainName
            // 
            lciDomainName.Control = cboDomainName;
            lciDomainName.Location = new System.Drawing.Point(0, 48);
            lciDomainName.Name = "lciDomainName";
            lciDomainName.Size = new System.Drawing.Size(326, 24);
            lciDomainName.Text = "DomainName";
            lciDomainName.TextSize = new System.Drawing.Size(85, 13);
            // 
            // lciCommand
            // 
            lciCommand.Control = btnSignIn;
            lciCommand.Location = new System.Drawing.Point(218, 96);
            lciCommand.Name = "lciCommand";
            lciCommand.Size = new System.Drawing.Size(108, 47);
            lciCommand.TextSize = new System.Drawing.Size(0, 0);
            lciCommand.TextVisible = false;
            // 
            // lciRememberMe
            // 
            lciRememberMe.Control = chkRememberMe;
            lciRememberMe.Location = new System.Drawing.Point(0, 72);
            lciRememberMe.Name = "lciRememberMe";
            lciRememberMe.Size = new System.Drawing.Size(326, 24);
            lciRememberMe.TextSize = new System.Drawing.Size(0, 0);
            lciRememberMe.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new System.Drawing.Point(0, 96);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new System.Drawing.Size(218, 47);
            emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // LoginView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(moduleLayout);
            Name = "LoginView";
            Size = new System.Drawing.Size(346, 163);
            ((System.ComponentModel.ISupportInitialize)MvvmContext).EndInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).EndInit();
            moduleLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtUsernameOrEmail.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtPassword.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboDomainName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkRememberMe.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciUsernameOrEmail).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciPassword).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciDomainName).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciCommand).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciRememberMe).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl moduleLayout;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txtUsernameOrEmail;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraLayout.LayoutControlItem lciUsernameOrEmail;
        private DevExpress.XtraLayout.LayoutControlItem lciPassword;
        private DevExpress.XtraLayout.LayoutControlItem lciDomainName;
        private DevExpress.XtraEditors.SimpleButton btnSignIn;
        private DevExpress.XtraLayout.LayoutControlItem lciCommand;
        private DevExpress.XtraEditors.ComboBoxEdit cboDomainName;
        private DevExpress.XtraEditors.CheckEdit chkRememberMe;
        private DevExpress.XtraLayout.LayoutControlItem lciRememberMe;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}

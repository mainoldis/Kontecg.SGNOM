namespace Kontecg.Views.OrganizationUnits
{
    partial class WorkPlaceEditView
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
            txtDisplayName = new DevExpress.XtraEditors.TextEdit();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            cboClassification = new DevExpress.XtraEditors.ComboBoxEdit();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            cboWorkPlacePayment = new DevExpress.XtraEditors.ComboBoxEdit();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            seMaxApprovedMembers = new DevExpress.XtraEditors.SpinEdit();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            btnOk = new DevExpress.XtraEditors.SimpleButton();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)MvvmContext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).BeginInit();
            moduleLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtDisplayName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboClassification.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboWorkPlacePayment.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)seMaxApprovedMembers.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            SuspendLayout();
            // 
            // moduleLayout
            // 
            moduleLayout.Controls.Add(txtDisplayName);
            moduleLayout.Controls.Add(cboClassification);
            moduleLayout.Controls.Add(cboWorkPlacePayment);
            moduleLayout.Controls.Add(seMaxApprovedMembers);
            moduleLayout.Controls.Add(btnOk);
            moduleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            moduleLayout.Location = new System.Drawing.Point(0, 0);
            moduleLayout.Name = "moduleLayout";
            moduleLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(463, 289, 869, 546);
            moduleLayout.Root = Root;
            moduleLayout.Size = new System.Drawing.Size(405, 192);
            moduleLayout.TabIndex = 0;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem2, layoutControlItem3, layoutControlItem4, layoutControlItem5, emptySpaceItem1 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(405, 192);
            Root.TextVisible = false;
            // 
            // txtDisplayName
            // 
            txtDisplayName.Location = new System.Drawing.Point(134, 12);
            txtDisplayName.Name = "txtDisplayName";
            txtDisplayName.Size = new System.Drawing.Size(259, 20);
            txtDisplayName.StyleController = moduleLayout;
            txtDisplayName.TabIndex = 0;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = txtDisplayName;
            layoutControlItem1.CustomizationFormText = "lciDisplayName";
            layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new System.Drawing.Size(385, 24);
            layoutControlItem1.Text = "DisplayName";
            layoutControlItem1.TextSize = new System.Drawing.Size(110, 13);
            // 
            // cboClassification
            // 
            cboClassification.Location = new System.Drawing.Point(134, 36);
            cboClassification.Name = "cboClassification";
            cboClassification.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboClassification.Size = new System.Drawing.Size(259, 20);
            cboClassification.StyleController = moduleLayout;
            cboClassification.TabIndex = 2;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = cboClassification;
            layoutControlItem2.CustomizationFormText = "lciClassification";
            layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new System.Drawing.Size(385, 24);
            layoutControlItem2.Text = "Classification";
            layoutControlItem2.TextSize = new System.Drawing.Size(110, 13);
            // 
            // cboWorkPlacePayment
            // 
            cboWorkPlacePayment.Location = new System.Drawing.Point(134, 60);
            cboWorkPlacePayment.Name = "cboWorkPlacePayment";
            cboWorkPlacePayment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboWorkPlacePayment.Properties.DropDownRows = 30;
            cboWorkPlacePayment.Size = new System.Drawing.Size(259, 20);
            cboWorkPlacePayment.StyleController = moduleLayout;
            cboWorkPlacePayment.TabIndex = 3;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = cboWorkPlacePayment;
            layoutControlItem3.CustomizationFormText = "lciWorkPlacePayment";
            layoutControlItem3.Location = new System.Drawing.Point(0, 48);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new System.Drawing.Size(385, 24);
            layoutControlItem3.Text = "WorkPlacePayment";
            layoutControlItem3.TextSize = new System.Drawing.Size(110, 13);
            // 
            // seMaxApprovedMembers
            // 
            seMaxApprovedMembers.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
            seMaxApprovedMembers.Location = new System.Drawing.Point(134, 84);
            seMaxApprovedMembers.Name = "seMaxApprovedMembers";
            seMaxApprovedMembers.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            seMaxApprovedMembers.Properties.MaxValue = new decimal(new int[] { 1000, 0, 0, 0 });
            seMaxApprovedMembers.Size = new System.Drawing.Size(259, 20);
            seMaxApprovedMembers.StyleController = moduleLayout;
            seMaxApprovedMembers.TabIndex = 4;
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = seMaxApprovedMembers;
            layoutControlItem4.CustomizationFormText = "lciMaxApprovedMembers";
            layoutControlItem4.Location = new System.Drawing.Point(0, 72);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new System.Drawing.Size(385, 24);
            layoutControlItem4.Text = "MaxApprovedMembers";
            layoutControlItem4.TextSize = new System.Drawing.Size(110, 13);
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(281, 108);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(112, 22);
            btnOk.StyleController = moduleLayout;
            btnOk.TabIndex = 5;
            btnOk.Text = "Confirm";
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = btnOk;
            layoutControlItem5.Location = new System.Drawing.Point(269, 96);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new System.Drawing.Size(116, 76);
            layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new System.Drawing.Point(0, 96);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new System.Drawing.Size(269, 76);
            emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // WorkPlaceCreateView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(moduleLayout);
            Name = "WorkPlaceCreateView";
            Size = new System.Drawing.Size(405, 192);
            ((System.ComponentModel.ISupportInitialize)MvvmContext).EndInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).EndInit();
            moduleLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtDisplayName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboClassification.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboWorkPlacePayment.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)seMaxApprovedMembers.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl moduleLayout;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txtDisplayName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.ComboBoxEdit cboClassification;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.ComboBoxEdit cboWorkPlacePayment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SpinEdit seMaxApprovedMembers;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}

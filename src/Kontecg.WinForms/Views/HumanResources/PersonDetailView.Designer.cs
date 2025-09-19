namespace Kontecg.Views.HumanResources
{
    partial class PersonDetailView
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
            moduleLayout = new DevExpress.XtraDataLayout.DataLayoutControl();
            pePhoto = new DevExpress.XtraEditors.PictureEdit();
            bindingSource = new System.Windows.Forms.BindingSource(components);
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            ItemForPhoto = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            FullNameTextEdit = new DevExpress.XtraEditors.TextEdit();
            ItemForFullName = new DevExpress.XtraLayout.LayoutControlItem();
            IdentityCardTextEdit = new DevExpress.XtraEditors.TextEdit();
            ItemForIdentityCard = new DevExpress.XtraLayout.LayoutControlItem();
            GenderTextEdit = new DevExpress.XtraEditors.TextEdit();
            ItemForGender = new DevExpress.XtraLayout.LayoutControlItem();
            BirthDateDateEdit = new DevExpress.XtraEditors.DateEdit();
            ItemForBirthDate = new DevExpress.XtraLayout.LayoutControlItem();
            AgeTextEdit = new DevExpress.XtraEditors.TextEdit();
            ItemForAge = new DevExpress.XtraLayout.LayoutControlItem();
            ScholarshipLevelTextEdit = new DevExpress.XtraEditors.TextEdit();
            ItemForScholarshipLevel = new DevExpress.XtraLayout.LayoutControlItem();
            ScholarshipTextEdit = new DevExpress.XtraEditors.TextEdit();
            ItemForScholarship = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)MvvmContext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).BeginInit();
            moduleLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pePhoto.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemForPhoto).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FullNameTextEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemForFullName).BeginInit();
            ((System.ComponentModel.ISupportInitialize)IdentityCardTextEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemForIdentityCard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)GenderTextEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemForGender).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BirthDateDateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BirthDateDateEdit.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemForBirthDate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AgeTextEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemForAge).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScholarshipLevelTextEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemForScholarshipLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScholarshipTextEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemForScholarship).BeginInit();
            SuspendLayout();
            // 
            // moduleLayout
            // 
            moduleLayout.AllowCustomization = false;
            moduleLayout.Controls.Add(pePhoto);
            moduleLayout.Controls.Add(FullNameTextEdit);
            moduleLayout.Controls.Add(IdentityCardTextEdit);
            moduleLayout.Controls.Add(GenderTextEdit);
            moduleLayout.Controls.Add(BirthDateDateEdit);
            moduleLayout.Controls.Add(AgeTextEdit);
            moduleLayout.Controls.Add(ScholarshipLevelTextEdit);
            moduleLayout.Controls.Add(ScholarshipTextEdit);
            moduleLayout.DataSource = bindingSource;
            moduleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            moduleLayout.Location = new System.Drawing.Point(0, 0);
            moduleLayout.Name = "moduleLayout";
            moduleLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(528, 276, 767, 641);
            moduleLayout.Root = Root;
            moduleLayout.Size = new System.Drawing.Size(278, 382);
            moduleLayout.TabIndex = 0;
            // 
            // pePhoto
            // 
            pePhoto.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bindingSource, "Photo", true));
            pePhoto.Location = new System.Drawing.Point(10, 10);
            pePhoto.Name = "pePhoto";
            pePhoto.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            pePhoto.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            pePhoto.Size = new System.Drawing.Size(100, 168);
            pePhoto.StyleController = moduleLayout;
            pePhoto.TabIndex = 4;
            // 
            // bindingSource
            // 
            bindingSource.DataSource = typeof(Kontecg.HumanResources.Dto.PersonDto);
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { ItemForPhoto, layoutControlGroup1, emptySpaceItem1 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(278, 382);
            Root.TextVisible = false;
            // 
            // ItemForPhoto
            // 
            ItemForPhoto.Control = pePhoto;
            ItemForPhoto.Location = new System.Drawing.Point(0, 0);
            ItemForPhoto.MaxSize = new System.Drawing.Size(350, 420);
            ItemForPhoto.MinSize = new System.Drawing.Size(100, 120);
            ItemForPhoto.Name = "ItemForPhoto";
            ItemForPhoto.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            ItemForPhoto.Size = new System.Drawing.Size(100, 168);
            ItemForPhoto.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            ItemForPhoto.TextSize = new System.Drawing.Size(0, 0);
            ItemForPhoto.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { ItemForFullName, ItemForIdentityCard, ItemForGender, ItemForBirthDate, ItemForAge, ItemForScholarshipLevel, ItemForScholarship });
            layoutControlGroup1.Location = new System.Drawing.Point(100, 0);
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.Size = new System.Drawing.Size(158, 168);
            layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new System.Drawing.Point(0, 168);
            emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 10);
            emptySpaceItem1.MinSize = new System.Drawing.Size(10, 10);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new System.Drawing.Size(258, 194);
            emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // FullNameTextEdit
            // 
            FullNameTextEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bindingSource, "FullName", true));
            FullNameTextEdit.Location = new System.Drawing.Point(206, 12);
            FullNameTextEdit.Name = "FullNameTextEdit";
            FullNameTextEdit.Size = new System.Drawing.Size(60, 20);
            FullNameTextEdit.StyleController = moduleLayout;
            FullNameTextEdit.TabIndex = 5;
            // 
            // ItemForFullName
            // 
            ItemForFullName.Control = FullNameTextEdit;
            ItemForFullName.Location = new System.Drawing.Point(0, 0);
            ItemForFullName.Name = "ItemForFullName";
            ItemForFullName.Size = new System.Drawing.Size(158, 24);
            ItemForFullName.Text = "Full Name";
            ItemForFullName.TextSize = new System.Drawing.Size(82, 13);
            // 
            // IdentityCardTextEdit
            // 
            IdentityCardTextEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bindingSource, "IdentityCard", true));
            IdentityCardTextEdit.Location = new System.Drawing.Point(206, 36);
            IdentityCardTextEdit.Name = "IdentityCardTextEdit";
            IdentityCardTextEdit.Size = new System.Drawing.Size(60, 20);
            IdentityCardTextEdit.StyleController = moduleLayout;
            IdentityCardTextEdit.TabIndex = 6;
            // 
            // ItemForIdentityCard
            // 
            ItemForIdentityCard.Control = IdentityCardTextEdit;
            ItemForIdentityCard.Location = new System.Drawing.Point(0, 24);
            ItemForIdentityCard.Name = "ItemForIdentityCard";
            ItemForIdentityCard.OptionsTableLayoutItem.ColumnIndex = 1;
            ItemForIdentityCard.Size = new System.Drawing.Size(158, 24);
            ItemForIdentityCard.Text = "Identity Card";
            ItemForIdentityCard.TextSize = new System.Drawing.Size(82, 13);
            // 
            // GenderTextEdit
            // 
            GenderTextEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bindingSource, "Gender", true));
            GenderTextEdit.Location = new System.Drawing.Point(206, 60);
            GenderTextEdit.Name = "GenderTextEdit";
            GenderTextEdit.Size = new System.Drawing.Size(60, 20);
            GenderTextEdit.StyleController = moduleLayout;
            GenderTextEdit.TabIndex = 7;
            // 
            // ItemForGender
            // 
            ItemForGender.Control = GenderTextEdit;
            ItemForGender.Location = new System.Drawing.Point(0, 48);
            ItemForGender.Name = "ItemForGender";
            ItemForGender.OptionsTableLayoutItem.RowIndex = 1;
            ItemForGender.Size = new System.Drawing.Size(158, 24);
            ItemForGender.Text = "Gender";
            ItemForGender.TextSize = new System.Drawing.Size(82, 13);
            // 
            // BirthDateDateEdit
            // 
            BirthDateDateEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bindingSource, "BirthDate", true));
            BirthDateDateEdit.EditValue = null;
            BirthDateDateEdit.Location = new System.Drawing.Point(206, 84);
            BirthDateDateEdit.Name = "BirthDateDateEdit";
            BirthDateDateEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            BirthDateDateEdit.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            BirthDateDateEdit.Size = new System.Drawing.Size(60, 20);
            BirthDateDateEdit.StyleController = moduleLayout;
            BirthDateDateEdit.TabIndex = 8;
            // 
            // ItemForBirthDate
            // 
            ItemForBirthDate.Control = BirthDateDateEdit;
            ItemForBirthDate.Location = new System.Drawing.Point(0, 72);
            ItemForBirthDate.Name = "ItemForBirthDate";
            ItemForBirthDate.OptionsTableLayoutItem.ColumnIndex = 1;
            ItemForBirthDate.OptionsTableLayoutItem.RowIndex = 1;
            ItemForBirthDate.Size = new System.Drawing.Size(158, 24);
            ItemForBirthDate.Text = "Birth Date";
            ItemForBirthDate.TextSize = new System.Drawing.Size(82, 13);
            // 
            // AgeTextEdit
            // 
            AgeTextEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bindingSource, "Age", true));
            AgeTextEdit.Location = new System.Drawing.Point(206, 108);
            AgeTextEdit.Name = "AgeTextEdit";
            AgeTextEdit.Properties.Appearance.Options.UseTextOptions = true;
            AgeTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            AgeTextEdit.Properties.Mask.EditMask = "N0";
            AgeTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            AgeTextEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            AgeTextEdit.Size = new System.Drawing.Size(60, 20);
            AgeTextEdit.StyleController = moduleLayout;
            AgeTextEdit.TabIndex = 9;
            // 
            // ItemForAge
            // 
            ItemForAge.Control = AgeTextEdit;
            ItemForAge.Location = new System.Drawing.Point(0, 96);
            ItemForAge.Name = "ItemForAge";
            ItemForAge.OptionsTableLayoutItem.RowIndex = 2;
            ItemForAge.Size = new System.Drawing.Size(158, 24);
            ItemForAge.Text = "Age";
            ItemForAge.TextSize = new System.Drawing.Size(82, 13);
            // 
            // ScholarshipLevelTextEdit
            // 
            ScholarshipLevelTextEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bindingSource, "ScholarshipLevel", true));
            ScholarshipLevelTextEdit.Location = new System.Drawing.Point(206, 132);
            ScholarshipLevelTextEdit.Name = "ScholarshipLevelTextEdit";
            ScholarshipLevelTextEdit.Size = new System.Drawing.Size(60, 20);
            ScholarshipLevelTextEdit.StyleController = moduleLayout;
            ScholarshipLevelTextEdit.TabIndex = 10;
            // 
            // ItemForScholarshipLevel
            // 
            ItemForScholarshipLevel.Control = ScholarshipLevelTextEdit;
            ItemForScholarshipLevel.Location = new System.Drawing.Point(0, 120);
            ItemForScholarshipLevel.Name = "ItemForScholarshipLevel";
            ItemForScholarshipLevel.OptionsTableLayoutItem.ColumnIndex = 1;
            ItemForScholarshipLevel.OptionsTableLayoutItem.RowIndex = 2;
            ItemForScholarshipLevel.Size = new System.Drawing.Size(158, 24);
            ItemForScholarshipLevel.Text = "Scholarship Level";
            ItemForScholarshipLevel.TextSize = new System.Drawing.Size(82, 13);
            // 
            // ScholarshipTextEdit
            // 
            ScholarshipTextEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bindingSource, "Scholarship", true));
            ScholarshipTextEdit.Location = new System.Drawing.Point(206, 156);
            ScholarshipTextEdit.Name = "ScholarshipTextEdit";
            ScholarshipTextEdit.Size = new System.Drawing.Size(60, 20);
            ScholarshipTextEdit.StyleController = moduleLayout;
            ScholarshipTextEdit.TabIndex = 11;
            // 
            // ItemForScholarship
            // 
            ItemForScholarship.Control = ScholarshipTextEdit;
            ItemForScholarship.Location = new System.Drawing.Point(0, 144);
            ItemForScholarship.Name = "ItemForScholarship";
            ItemForScholarship.OptionsTableLayoutItem.RowIndex = 3;
            ItemForScholarship.Size = new System.Drawing.Size(158, 24);
            ItemForScholarship.Text = "Scholarship";
            ItemForScholarship.TextSize = new System.Drawing.Size(82, 13);
            // 
            // PersonDetailView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(moduleLayout);
            Name = "PersonDetailView";
            Size = new System.Drawing.Size(278, 382);
            ((System.ComponentModel.ISupportInitialize)MvvmContext).EndInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).EndInit();
            moduleLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pePhoto.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemForPhoto).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)FullNameTextEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemForFullName).EndInit();
            ((System.ComponentModel.ISupportInitialize)IdentityCardTextEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemForIdentityCard).EndInit();
            ((System.ComponentModel.ISupportInitialize)GenderTextEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemForGender).EndInit();
            ((System.ComponentModel.ISupportInitialize)BirthDateDateEdit.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)BirthDateDateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemForBirthDate).EndInit();
            ((System.ComponentModel.ISupportInitialize)AgeTextEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemForAge).EndInit();
            ((System.ComponentModel.ISupportInitialize)ScholarshipLevelTextEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemForScholarshipLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)ScholarshipTextEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemForScholarship).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraDataLayout.DataLayoutControl moduleLayout;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private System.Windows.Forms.BindingSource bindingSource;
        private DevExpress.XtraEditors.PictureEdit pePhoto;
        private DevExpress.XtraLayout.LayoutControlItem ItemForPhoto;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.TextEdit FullNameTextEdit;
        private DevExpress.XtraEditors.TextEdit IdentityCardTextEdit;
        private DevExpress.XtraEditors.TextEdit GenderTextEdit;
        private DevExpress.XtraEditors.DateEdit BirthDateDateEdit;
        private DevExpress.XtraEditors.TextEdit AgeTextEdit;
        private DevExpress.XtraEditors.TextEdit ScholarshipLevelTextEdit;
        private DevExpress.XtraEditors.TextEdit ScholarshipTextEdit;
        private DevExpress.XtraLayout.LayoutControlItem ItemForFullName;
        private DevExpress.XtraLayout.LayoutControlItem ItemForIdentityCard;
        private DevExpress.XtraLayout.LayoutControlItem ItemForGender;
        private DevExpress.XtraLayout.LayoutControlItem ItemForBirthDate;
        private DevExpress.XtraLayout.LayoutControlItem ItemForAge;
        private DevExpress.XtraLayout.LayoutControlItem ItemForScholarshipLevel;
        private DevExpress.XtraLayout.LayoutControlItem ItemForScholarship;
    }
}

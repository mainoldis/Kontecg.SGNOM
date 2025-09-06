namespace Kontecg.Views
{
    partial class MigratingOrganizationUnits
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule treeListFormatRule1 = new DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule();
            DevExpress.XtraEditors.FormatConditionRuleValue formatConditionRuleValue1 = new DevExpress.XtraEditors.FormatConditionRuleValue();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MigratingOrganizationUnits));
            colLevel = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            treeOU = new DevExpress.XtraTreeList.TreeList();
            colCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            colDisplayName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            colClassification = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            colWorkPlacePaymentCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            colDescription = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            colMaxMembersApproved = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            imageCollection = new DevExpress.Utils.DPIAwareImageCollection(components);
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            gridMaster = new DevExpress.XtraGrid.GridControl();
            gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumnOrganizationUnitId = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumnFirstLevelDisplayName = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumnThirdLevelDisplayName = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumnCenterCost = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumnExp = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumnFullName = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumnCode = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumnEffectiveSince = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumnWorkshit = new DevExpress.XtraGrid.Columns.GridColumn();
            btnRefreshDocuments = new DevExpress.XtraEditors.SimpleButton();
            btnNewNode = new DevExpress.XtraEditors.SimpleButton();
            btnRefreshTree = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)treeOU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)imageCollection).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridMaster).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView).BeginInit();
            SuspendLayout();
            // 
            // colLevel
            // 
            colLevel.Caption = "Level";
            colLevel.FieldName = "Level";
            colLevel.Name = "colLevel";
            colLevel.OptionsColumn.AllowEdit = false;
            colLevel.OptionsColumn.AllowSort = true;
            colLevel.OptionsFilter.AutoFilterCondition = DevExpress.XtraTreeList.Columns.AutoFilterCondition.Equals;
            colLevel.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.List;
            colLevel.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
            colLevel.Visible = true;
            colLevel.VisibleIndex = 3;
            // 
            // treeOU
            // 
            treeOU.AllowDrop = true;
            treeOU.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { colCode, colDisplayName, colClassification, colLevel, colWorkPlacePaymentCode, colDescription, colMaxMembersApproved });
            treeOU.Dock = System.Windows.Forms.DockStyle.Fill;
            treeListFormatRule1.ApplyToRow = true;
            treeListFormatRule1.Column = colLevel;
            treeListFormatRule1.Description = null;
            treeListFormatRule1.Name = "FormatLevel3";
            treeListFormatRule1.Rule = formatConditionRuleValue1;
            treeOU.FormatRules.Add(treeListFormatRule1);
            treeOU.HierarchyColumn = colCode;
            treeOU.KeyFieldName = "Id";
            treeOU.Location = new System.Drawing.Point(0, 0);
            treeOU.Name = "treeOU";
            treeOU.OptionsBehavior.EditingMode = DevExpress.XtraTreeList.TreeListEditingMode.EditForm;
            treeOU.OptionsBehavior.EditorShowMode = DevExpress.XtraTreeList.TreeListEditorShowMode.DoubleClick;
            treeOU.OptionsDragAndDrop.AcceptOuterNodes = true;
            treeOU.OptionsDragAndDrop.DropNodesMode = DevExpress.XtraTreeList.DropNodesMode.Advanced;
            treeOU.OptionsEditForm.ShowOnDoubleClick = DevExpress.Utils.DefaultBoolean.True;
            treeOU.OptionsView.BestFitNodes = DevExpress.XtraTreeList.TreeListBestFitNodes.Display;
            treeOU.OptionsView.ShowIndentAsRowStyle = true;
            treeOU.ParentFieldName = "ParentId";
            treeOU.PreviewFieldName = "Description";
            treeOU.RootValue = 0L;
            treeOU.Size = new System.Drawing.Size(693, 586);
            treeOU.StateImageList = imageCollection;
            treeOU.TabIndex = 0;
            // 
            // colCode
            // 
            colCode.Caption = "Code";
            colCode.FieldName = "Code";
            colCode.Name = "colCode";
            colCode.OptionsColumn.AllowEdit = false;
            colCode.OptionsColumn.AllowSort = true;
            colCode.Visible = true;
            colCode.VisibleIndex = 0;
            // 
            // colDisplayName
            // 
            colDisplayName.Caption = "DisplayName";
            colDisplayName.FieldName = "DisplayName";
            colDisplayName.Name = "colDisplayName";
            colDisplayName.OptionsColumn.AllowEdit = false;
            colDisplayName.OptionsColumn.AllowSort = true;
            colDisplayName.Visible = true;
            colDisplayName.VisibleIndex = 1;
            // 
            // colClassification
            // 
            colClassification.Caption = "Classification";
            colClassification.FieldName = "Classification";
            colClassification.Name = "colClassification";
            colClassification.OptionsColumn.AllowEdit = false;
            colClassification.OptionsColumn.AllowSort = true;
            colClassification.Visible = true;
            colClassification.VisibleIndex = 2;
            // 
            // colWorkPlacePaymentCode
            // 
            colWorkPlacePaymentCode.Caption = "PaymentCode";
            colWorkPlacePaymentCode.FieldName = "WorkPlacePaymentCode";
            colWorkPlacePaymentCode.Name = "colWorkPlacePaymentCode";
            colWorkPlacePaymentCode.OptionsColumn.AllowEdit = false;
            colWorkPlacePaymentCode.OptionsColumn.AllowSort = true;
            colWorkPlacePaymentCode.Visible = true;
            colWorkPlacePaymentCode.VisibleIndex = 4;
            // 
            // colDescription
            // 
            colDescription.Caption = "Acronym";
            colDescription.FieldName = "Description";
            colDescription.Name = "colDescription";
            colDescription.OptionsColumn.AllowEdit = false;
            colDescription.OptionsColumn.AllowSort = true;
            colDescription.Visible = true;
            colDescription.VisibleIndex = 5;
            // 
            // colMaxMembersApproved
            // 
            colMaxMembersApproved.AllNodesSummary = true;
            colMaxMembersApproved.Caption = "Approved";
            colMaxMembersApproved.FieldName = "WorkPlace.MaxMembersApproved";
            colMaxMembersApproved.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Right;
            colMaxMembersApproved.Name = "colMaxMembersApproved";
            colMaxMembersApproved.OptionsFilter.AllowAutoFilter = false;
            colMaxMembersApproved.OptionsFilter.AllowFilter = false;
            colMaxMembersApproved.SummaryFooter = DevExpress.XtraTreeList.SummaryItemType.Sum;
            colMaxMembersApproved.Visible = true;
            colMaxMembersApproved.VisibleIndex = 6;
            // 
            // imageCollection
            // 
            imageCollection.Images.AddRange(new DevExpress.Utils.DefaultImage[] { new DevExpress.Utils.DefaultImage(new DevExpress.Utils.LocalImageLocator("documentmap_16x16.png")), new DevExpress.Utils.DefaultImage(new DevExpress.Utils.LocalImageLocator("documentmap_16x16.png")), new DevExpress.Utils.DefaultImage(new DevExpress.Utils.LocalImageLocator("treeview_16x16.png")), new DevExpress.Utils.DefaultImage(new DevExpress.Utils.LocalImageLocator("new_16x16.png")), new DevExpress.Utils.DefaultImage(new DevExpress.Utils.LocalImageLocator("snapemptytablerowseparator_16x16.png")) });
            imageCollection.Owner = this;
            imageCollection.Stream = (DevExpress.Utils.DPIAwareImageCollectionStreamer)resources.GetObject("imageCollection.Stream");
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.AllowDrop = true;
            splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
            splitContainerControl1.Location = new System.Drawing.Point(12, 12);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(treeOU);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(gridMaster);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new System.Drawing.Size(1295, 586);
            splitContainerControl1.SplitterPosition = 693;
            splitContainerControl1.TabIndex = 0;
            // 
            // gridMaster
            // 
            gridMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            gridMaster.Location = new System.Drawing.Point(0, 0);
            gridMaster.MainView = gridView;
            gridMaster.Name = "gridMaster";
            gridMaster.ShowOnlyPredefinedDetails = true;
            gridMaster.Size = new System.Drawing.Size(592, 586);
            gridMaster.TabIndex = 0;
            gridMaster.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });
            // 
            // gridView
            // 
            gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumnOrganizationUnitId, gridColumnFirstLevelDisplayName, gridColumnThirdLevelDisplayName, gridColumnCenterCost, gridColumnExp, gridColumnFullName, gridColumnCode, gridColumnEffectiveSince, gridColumnWorkshit });
            gridView.GridControl = gridMaster;
            gridView.GroupCount = 1;
            gridView.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] { new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, "Documento.Exp", gridColumnExp, "{0}") });
            gridView.Name = "gridView";
            gridView.OptionsBehavior.AutoExpandAllGroups = true;
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsCustomization.AllowMergedGrouping = DevExpress.Utils.DefaultBoolean.True;
            gridView.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gridView.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Embedded;
            gridView.OptionsDetail.SmartDetailExpandButtonMode = DevExpress.XtraGrid.Views.Grid.DetailExpandButtonMode.CheckDefaultDetail;
            gridView.OptionsFind.AlwaysVisible = true;
            gridView.OptionsSelection.MultiSelect = true;
            gridView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] { new DevExpress.XtraGrid.Columns.GridColumnSortInfo(gridColumnFirstLevelDisplayName, DevExpress.Data.ColumnSortOrder.Ascending) });
            gridView.ViewCaption = "Documentos";
            // 
            // gridColumnOrganizationUnitId
            // 
            gridColumnOrganizationUnitId.Caption = "OUID";
            gridColumnOrganizationUnitId.FieldName = "Document.OrganizationUnitId";
            gridColumnOrganizationUnitId.Name = "gridColumnOrganizationUnitId";
            gridColumnOrganizationUnitId.Visible = true;
            gridColumnOrganizationUnitId.VisibleIndex = 0;
            gridColumnOrganizationUnitId.Width = 60;
            // 
            // gridColumnFirstLevelDisplayName
            // 
            gridColumnFirstLevelDisplayName.Caption = "Nivel 1";
            gridColumnFirstLevelDisplayName.FieldName = "Document.FirstLevelDisplayName";
            gridColumnFirstLevelDisplayName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            gridColumnFirstLevelDisplayName.Name = "gridColumnFirstLevelDisplayName";
            gridColumnFirstLevelDisplayName.Visible = true;
            gridColumnFirstLevelDisplayName.VisibleIndex = 1;
            gridColumnFirstLevelDisplayName.Width = 69;
            // 
            // gridColumnThirdLevelDisplayName
            // 
            gridColumnThirdLevelDisplayName.Caption = "Nivel 3";
            gridColumnThirdLevelDisplayName.FieldName = "Document.ThirdLevelDisplayName";
            gridColumnThirdLevelDisplayName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            gridColumnThirdLevelDisplayName.Name = "gridColumnThirdLevelDisplayName";
            gridColumnThirdLevelDisplayName.Visible = true;
            gridColumnThirdLevelDisplayName.VisibleIndex = 1;
            gridColumnThirdLevelDisplayName.Width = 60;
            // 
            // gridColumnCenterCost
            // 
            gridColumnCenterCost.Caption = "CC";
            gridColumnCenterCost.FieldName = "Document.CenterCost";
            gridColumnCenterCost.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            gridColumnCenterCost.Name = "gridColumnCenterCost";
            gridColumnCenterCost.Visible = true;
            gridColumnCenterCost.VisibleIndex = 2;
            gridColumnCenterCost.Width = 60;
            // 
            // gridColumnExp
            // 
            gridColumnExp.Caption = "Chapa";
            gridColumnExp.FieldName = "Document.Exp";
            gridColumnExp.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            gridColumnExp.Name = "gridColumnExp";
            gridColumnExp.Visible = true;
            gridColumnExp.VisibleIndex = 3;
            gridColumnExp.Width = 60;
            // 
            // gridColumnFullName
            // 
            gridColumnFullName.Caption = "Trabajador";
            gridColumnFullName.FieldName = "Document.Person.FullName";
            gridColumnFullName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            gridColumnFullName.Name = "gridColumnFullName";
            gridColumnFullName.Visible = true;
            gridColumnFullName.VisibleIndex = 4;
            gridColumnFullName.Width = 60;
            // 
            // gridColumnCode
            // 
            gridColumnCode.Caption = "Movimiento";
            gridColumnCode.FieldName = "Document.Code";
            gridColumnCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            gridColumnCode.Name = "gridColumnCode";
            gridColumnCode.Visible = true;
            gridColumnCode.VisibleIndex = 5;
            gridColumnCode.Width = 60;
            // 
            // gridColumnEffectiveSince
            // 
            gridColumnEffectiveSince.Caption = "Efectivo";
            gridColumnEffectiveSince.FieldName = "Document.EffectiveSince";
            gridColumnEffectiveSince.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            gridColumnEffectiveSince.Name = "gridColumnEffectiveSince";
            gridColumnEffectiveSince.Visible = true;
            gridColumnEffectiveSince.VisibleIndex = 6;
            gridColumnEffectiveSince.Width = 58;
            // 
            // gridColumnWorkshit
            // 
            gridColumnWorkshit.Caption = "Turno";
            gridColumnWorkshit.FieldName = "Document.WorkShift.DisplayName";
            gridColumnWorkshit.MaxWidth = 30;
            gridColumnWorkshit.Name = "gridColumnWorkshit";
            gridColumnWorkshit.Visible = true;
            gridColumnWorkshit.VisibleIndex = 7;
            gridColumnWorkshit.Width = 30;
            // 
            // btnRefreshDocuments
            // 
            btnRefreshDocuments.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnRefreshDocuments.Location = new System.Drawing.Point(1232, 604);
            btnRefreshDocuments.Name = "btnRefreshDocuments";
            btnRefreshDocuments.Size = new System.Drawing.Size(75, 23);
            btnRefreshDocuments.TabIndex = 3;
            btnRefreshDocuments.Text = "Refresh";
            // 
            // btnNewNode
            // 
            btnNewNode.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnNewNode.Location = new System.Drawing.Point(12, 604);
            btnNewNode.Name = "btnNewNode";
            btnNewNode.Size = new System.Drawing.Size(75, 23);
            btnNewNode.TabIndex = 1;
            btnNewNode.Text = "New";
            // 
            // btnRefreshTree
            // 
            btnRefreshTree.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnRefreshTree.Location = new System.Drawing.Point(93, 604);
            btnRefreshTree.Name = "btnRefreshTree";
            btnRefreshTree.Size = new System.Drawing.Size(75, 23);
            btnRefreshTree.TabIndex = 2;
            btnRefreshTree.Text = "Refresh";
            // 
            // MigratingOrganizationUnits
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1319, 649);
            Controls.Add(btnRefreshTree);
            Controls.Add(btnNewNode);
            Controls.Add(btnRefreshDocuments);
            Controls.Add(splitContainerControl1);
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "MigratingOrganizationUnits";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = " ";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)treeOU).EndInit();
            ((System.ComponentModel.ISupportInitialize)imageCollection).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridMaster).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraTreeList.TreeList treeOU;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDisplayName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colClassification;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colLevel;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colWorkPlacePaymentCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDescription;
        private DevExpress.XtraEditors.SimpleButton btnRefreshDocuments;
        private DevExpress.XtraEditors.SimpleButton btnNewNode;
        private DevExpress.XtraGrid.GridControl gridMaster;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnOrganizationUnitId;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFirstLevelDisplayName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnThirdLevelDisplayName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnCenterCost;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnExp;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFullName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnEffectiveSince;
        private DevExpress.Utils.DPIAwareImageCollection imageCollection;
        private DevExpress.XtraEditors.SimpleButton btnRefreshTree;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnWorkshit;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colMaxMembersApproved;
    }
}
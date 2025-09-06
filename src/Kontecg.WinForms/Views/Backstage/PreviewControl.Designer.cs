namespace Kontecg.Views.Backstage {
    partial class PreviewControl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewControl));
            documentViewerCore = new DevExpress.XtraPrinting.Preview.DocumentViewer();
            documentViewerBarManager1 = new DevExpress.XtraPrinting.Preview.DocumentViewerBarManager(components);
            previewBar1 = new DevExpress.XtraPrinting.Preview.PreviewBar();
            printPreviewBarItem18 = new DevExpress.XtraPrinting.Preview.PrintPreviewBarItem();
            printPreviewStaticItem1 = new DevExpress.XtraPrinting.Preview.PrintPreviewStaticItem();
            printPreviewBarItem19 = new DevExpress.XtraPrinting.Preview.PrintPreviewBarItem();
            progressBarEditItem1 = new DevExpress.XtraPrinting.Preview.ProgressBarEditItem();
            repositoryItemProgressBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            printPreviewBarItem1 = new DevExpress.XtraPrinting.Preview.PrintPreviewBarItem();
            printPreviewStaticItem2 = new DevExpress.XtraPrinting.Preview.PrintPreviewStaticItem();
            zoomTrackBarEditItem1 = new DevExpress.XtraPrinting.Preview.ZoomTrackBarEditItem();
            repositoryItemZoomTrackBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();
            printPreviewBarItemWholePage = new DevExpress.XtraPrinting.Preview.PrintPreviewBarItem();
            printPreviewBarItemMultiplePages = new DevExpress.XtraPrinting.Preview.PrintPreviewBarItem();
            printPreviewBarItemScale = new DevExpress.XtraPrinting.Preview.PrintPreviewBarItem();
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            printPreviewRepositoryItemComboBox1 = new DevExpress.XtraPrinting.Preview.PrintPreviewRepositoryItemComboBox();
            ((System.ComponentModel.ISupportInitialize)MvvmContext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)documentViewerBarManager1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemProgressBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemZoomTrackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)printPreviewRepositoryItemComboBox1).BeginInit();
            SuspendLayout();
            // 
            // documentViewerCore
            // 
            documentViewerCore.Dock = System.Windows.Forms.DockStyle.Fill;
            documentViewerCore.Location = new System.Drawing.Point(0, 0);
            documentViewerCore.Name = "documentViewerCore";
            documentViewerCore.Size = new System.Drawing.Size(1045, 560);
            documentViewerCore.TabIndex = 0;
            // 
            // documentViewerBarManager1
            // 
            documentViewerBarManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] { previewBar1 });
            documentViewerBarManager1.DockControls.Add(barDockControlTop);
            documentViewerBarManager1.DockControls.Add(barDockControlBottom);
            documentViewerBarManager1.DockControls.Add(barDockControlLeft);
            documentViewerBarManager1.DockControls.Add(barDockControlRight);
            documentViewerBarManager1.DocumentViewer = documentViewerCore;
            documentViewerBarManager1.Form = this;
            documentViewerBarManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { printPreviewStaticItem1, progressBarEditItem1, printPreviewBarItem1, printPreviewStaticItem2, zoomTrackBarEditItem1, printPreviewBarItemScale, printPreviewBarItem18, printPreviewBarItem19, printPreviewBarItemMultiplePages, printPreviewBarItemWholePage });
            documentViewerBarManager1.MaxItemId = 57;
            documentViewerBarManager1.PreviewBar = previewBar1;
            documentViewerBarManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemProgressBar1, repositoryItemZoomTrackBar1, printPreviewRepositoryItemComboBox1 });
            documentViewerBarManager1.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.True;
            // 
            // previewBar1
            // 
            previewBar1.BarName = "Toolbar";
            previewBar1.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            previewBar1.DockCol = 0;
            previewBar1.DockRow = 0;
            previewBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            previewBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(printPreviewBarItem18), new DevExpress.XtraBars.LinkPersistInfo(printPreviewStaticItem1), new DevExpress.XtraBars.LinkPersistInfo(printPreviewBarItem19), new DevExpress.XtraBars.LinkPersistInfo(progressBarEditItem1), new DevExpress.XtraBars.LinkPersistInfo(printPreviewBarItem1), new DevExpress.XtraBars.LinkPersistInfo(printPreviewStaticItem2, true), new DevExpress.XtraBars.LinkPersistInfo(zoomTrackBarEditItem1), new DevExpress.XtraBars.LinkPersistInfo(printPreviewBarItemWholePage, true), new DevExpress.XtraBars.LinkPersistInfo(printPreviewBarItemMultiplePages), new DevExpress.XtraBars.LinkPersistInfo(printPreviewBarItemScale) });
            previewBar1.OptionsBar.AllowQuickCustomization = false;
            previewBar1.OptionsBar.DrawBorder = false;
            previewBar1.OptionsBar.DrawDragBorder = false;
            previewBar1.OptionsBar.UseWholeRow = true;
            previewBar1.Text = "Toolbar";
            // 
            // printPreviewBarItem18
            // 
            printPreviewBarItem18.Caption = "Previous Page";
            printPreviewBarItem18.Command = DevExpress.XtraPrinting.PrintingSystemCommand.ShowPrevPage;
            printPreviewBarItem18.Enabled = false;
            printPreviewBarItem18.Hint = "Previous Page";
            printPreviewBarItem18.Id = 24;
            printPreviewBarItem18.Name = "printPreviewBarItem18";
            // 
            // printPreviewStaticItem1
            // 
            printPreviewStaticItem1.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            printPreviewStaticItem1.Caption = "Nothing";
            printPreviewStaticItem1.Id = 0;
            printPreviewStaticItem1.LeftIndent = 1;
            printPreviewStaticItem1.Name = "printPreviewStaticItem1";
            printPreviewStaticItem1.RightIndent = 1;
            printPreviewStaticItem1.Type = "PageOfPages";
            // 
            // printPreviewBarItem19
            // 
            printPreviewBarItem19.Caption = "Next Page";
            printPreviewBarItem19.Command = DevExpress.XtraPrinting.PrintingSystemCommand.ShowNextPage;
            printPreviewBarItem19.Enabled = false;
            printPreviewBarItem19.Hint = "Next Page";
            printPreviewBarItem19.Id = 25;
            printPreviewBarItem19.Name = "printPreviewBarItem19";
            // 
            // progressBarEditItem1
            // 
            progressBarEditItem1.Edit = repositoryItemProgressBar1;
            progressBarEditItem1.EditHeight = 12;
            progressBarEditItem1.EditWidth = 150;
            progressBarEditItem1.Id = 2;
            progressBarEditItem1.Name = "progressBarEditItem1";
            progressBarEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // repositoryItemProgressBar1
            // 
            repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
            // 
            // printPreviewBarItem1
            // 
            printPreviewBarItem1.Caption = "Stop";
            printPreviewBarItem1.Command = DevExpress.XtraPrinting.PrintingSystemCommand.StopPageBuilding;
            printPreviewBarItem1.Enabled = false;
            printPreviewBarItem1.Hint = "Stop";
            printPreviewBarItem1.Id = 3;
            printPreviewBarItem1.Name = "printPreviewBarItem1";
            printPreviewBarItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // printPreviewStaticItem2
            // 
            printPreviewStaticItem2.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            printPreviewStaticItem2.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            printPreviewStaticItem2.Caption = "100%";
            printPreviewStaticItem2.Id = 5;
            printPreviewStaticItem2.Name = "printPreviewStaticItem2";
            printPreviewStaticItem2.TextAlignment = System.Drawing.StringAlignment.Far;
            printPreviewStaticItem2.Type = "ZoomFactor";
            // 
            // zoomTrackBarEditItem1
            // 
            zoomTrackBarEditItem1.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            zoomTrackBarEditItem1.Edit = repositoryItemZoomTrackBar1;
            zoomTrackBarEditItem1.EditValue = 90;
            zoomTrackBarEditItem1.EditWidth = 140;
            zoomTrackBarEditItem1.Enabled = false;
            zoomTrackBarEditItem1.Id = 6;
            zoomTrackBarEditItem1.Name = "zoomTrackBarEditItem1";
            zoomTrackBarEditItem1.Range = new int[]
    {
    10,
    500
    };
            // 
            // repositoryItemZoomTrackBar1
            // 
            repositoryItemZoomTrackBar1.Alignment = DevExpress.Utils.VertAlignment.Center;
            repositoryItemZoomTrackBar1.AllowFocused = false;
            repositoryItemZoomTrackBar1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            repositoryItemZoomTrackBar1.Maximum = 180;
            repositoryItemZoomTrackBar1.Middle = 90;
            repositoryItemZoomTrackBar1.Name = "repositoryItemZoomTrackBar1";
            // 
            // printPreviewBarItemWholePage
            // 
            printPreviewBarItemWholePage.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            printPreviewBarItemWholePage.Caption = "Fit To Page";
            printPreviewBarItemWholePage.Command = DevExpress.XtraPrinting.PrintingSystemCommand.ZoomToWholePage;
            printPreviewBarItemWholePage.Enabled = false;
            printPreviewBarItemWholePage.Hint = "Fit to Page";
            printPreviewBarItemWholePage.Id = 27;
            printPreviewBarItemWholePage.Name = "printPreviewBarItemWholePage";
            // 
            // printPreviewBarItemMultiplePages
            // 
            printPreviewBarItemMultiplePages.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            printPreviewBarItemMultiplePages.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            printPreviewBarItemMultiplePages.Caption = "Multiple Pages";
            printPreviewBarItemMultiplePages.Command = DevExpress.XtraPrinting.PrintingSystemCommand.MultiplePages;
            printPreviewBarItemMultiplePages.Enabled = false;
            printPreviewBarItemMultiplePages.Hint = "Multiple Pages";
            printPreviewBarItemMultiplePages.Id = 27;
            printPreviewBarItemMultiplePages.Name = "printPreviewBarItemMultiplePages";
            // 
            // printPreviewBarItemScale
            // 
            printPreviewBarItemScale.ActAsDropDown = true;
            printPreviewBarItemScale.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            printPreviewBarItemScale.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            printPreviewBarItemScale.Caption = "Scale";
            printPreviewBarItemScale.Command = DevExpress.XtraPrinting.PrintingSystemCommand.Scale;
            printPreviewBarItemScale.Enabled = false;
            printPreviewBarItemScale.Hint = "Scale";
            printPreviewBarItemScale.Id = 17;
            printPreviewBarItemScale.ImageOptions.ImageIndex = 25;
            printPreviewBarItemScale.ImageOptions.ImageUri.Uri = "outlook%20inspired/icon_scale";
            printPreviewBarItemScale.Name = "printPreviewBarItemScale";
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            barDockControlTop.Location = new System.Drawing.Point(0, 0);
            barDockControlTop.Manager = documentViewerBarManager1;
            barDockControlTop.Size = new System.Drawing.Size(1045, 0);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            barDockControlBottom.Location = new System.Drawing.Point(0, 560);
            barDockControlBottom.Manager = documentViewerBarManager1;
            barDockControlBottom.Size = new System.Drawing.Size(1045, 24);
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            barDockControlLeft.Manager = documentViewerBarManager1;
            barDockControlLeft.Size = new System.Drawing.Size(0, 560);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            barDockControlRight.Location = new System.Drawing.Point(1045, 0);
            barDockControlRight.Manager = documentViewerBarManager1;
            barDockControlRight.Size = new System.Drawing.Size(0, 560);
            // 
            // printPreviewRepositoryItemComboBox1
            // 
            printPreviewRepositoryItemComboBox1.AutoComplete = false;
            printPreviewRepositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            printPreviewRepositoryItemComboBox1.DropDownRows = 11;
            printPreviewRepositoryItemComboBox1.Name = "printPreviewRepositoryItemComboBox1";
            // 
            // PreviewView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(documentViewerCore);
            Controls.Add(barDockControlLeft);
            Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom);
            Controls.Add(barDockControlTop);
            Name = "PreviewView";
            Size = new System.Drawing.Size(1045, 584);
            ((System.ComponentModel.ISupportInitialize)MvvmContext).EndInit();
            ((System.ComponentModel.ISupportInitialize)documentViewerBarManager1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemProgressBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemZoomTrackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)printPreviewRepositoryItemComboBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraPrinting.Preview.DocumentViewer documentViewerCore;
        private DevExpress.XtraPrinting.Preview.DocumentViewerBarManager documentViewerBarManager1;
        private DevExpress.XtraPrinting.Preview.PreviewBar previewBar1;
        private DevExpress.XtraPrinting.Preview.PrintPreviewBarItem printPreviewBarItem18;
        private DevExpress.XtraPrinting.Preview.PrintPreviewStaticItem printPreviewStaticItem1;
        private DevExpress.XtraPrinting.Preview.PrintPreviewBarItem printPreviewBarItem19;
        private DevExpress.XtraPrinting.Preview.ProgressBarEditItem progressBarEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar repositoryItemProgressBar1;
        private DevExpress.XtraPrinting.Preview.PrintPreviewBarItem printPreviewBarItem1;
        private DevExpress.XtraPrinting.Preview.PrintPreviewStaticItem printPreviewStaticItem2;
        private DevExpress.XtraPrinting.Preview.ZoomTrackBarEditItem zoomTrackBarEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar repositoryItemZoomTrackBar1;
        private DevExpress.XtraPrinting.Preview.PrintPreviewBarItem printPreviewBarItemWholePage;
        private DevExpress.XtraPrinting.Preview.PrintPreviewBarItem printPreviewBarItemScale;
        private DevExpress.XtraPrinting.Preview.PrintPreviewBarItem printPreviewBarItemMultiplePages;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraPrinting.Preview.PrintPreviewRepositoryItemComboBox printPreviewRepositoryItemComboBox1;
    }
}

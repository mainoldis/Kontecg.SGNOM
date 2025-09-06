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
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            bindingSource = new System.Windows.Forms.BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)MvvmContext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)moduleLayout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
            SuspendLayout();
            // 
            // moduleLayout
            // 
            moduleLayout.AllowCustomization = false;
            moduleLayout.DataSource = bindingSource;
            moduleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            moduleLayout.Location = new System.Drawing.Point(0, 0);
            moduleLayout.Name = "moduleLayout";
            moduleLayout.Root = Root;
            moduleLayout.Size = new System.Drawing.Size(278, 382);
            moduleLayout.TabIndex = 0;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(278, 382);
            Root.TextVisible = false;
            // 
            // bindingSource
            // 
            bindingSource.DataSource = typeof(Kontecg.HumanResources.Dto.PersonDto);
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
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraDataLayout.DataLayoutControl moduleLayout;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private System.Windows.Forms.BindingSource bindingSource;
    }
}

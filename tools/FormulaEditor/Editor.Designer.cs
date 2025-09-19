namespace FormulaEditor
{
    partial class Editor
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
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            message = new System.Windows.Forms.ToolStripStatusLabel();
            btnCheck = new System.Windows.Forms.Button();
            richEditor = new System.Windows.Forms.RichTextBox();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { message });
            statusStrip1.Location = new System.Drawing.Point(0, 603);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(966, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // message
            // 
            message.Name = "message";
            message.Size = new System.Drawing.Size(0, 17);
            // 
            // btnCheck
            // 
            btnCheck.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCheck.Location = new System.Drawing.Point(879, 568);
            btnCheck.Name = "btnCheck";
            btnCheck.Size = new System.Drawing.Size(75, 23);
            btnCheck.TabIndex = 2;
            btnCheck.Text = "Validate";
            btnCheck.UseVisualStyleBackColor = true;
            // 
            // richEditor
            // 
            richEditor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richEditor.Location = new System.Drawing.Point(12, 12);
            richEditor.Name = "richEditor";
            richEditor.Size = new System.Drawing.Size(942, 550);
            richEditor.TabIndex = 3;
            richEditor.Text = "";
            // 
            // Editor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(966, 625);
            Controls.Add(richEditor);
            Controls.Add(btnCheck);
            Controls.Add(statusStrip1);
            Name = "Editor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Editor";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.ToolStripStatusLabel message;
        private System.Windows.Forms.RichTextBox richEditor;
    }
}


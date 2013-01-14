namespace Ildss
{
    partial class Ildss
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
            this.lblDirectory = new System.Windows.Forms.Label();
            this.DirectorySelect = new System.Windows.Forms.FolderBrowserDialog();
            this.lblDirectoryOutput = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblDirectory
            // 
            this.lblDirectory.AutoSize = true;
            this.lblDirectory.Location = new System.Drawing.Point(35, 13);
            this.lblDirectory.Name = "lblDirectory";
            this.lblDirectory.Size = new System.Drawing.Size(52, 13);
            this.lblDirectory.TabIndex = 0;
            this.lblDirectory.Text = "Directory:";
            // 
            // DirectorySelect
            // 
            this.DirectorySelect.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.DirectorySelect.SelectedPath = "C:\\";
            // 
            // lblDirectoryOutput
            // 
            this.lblDirectoryOutput.AutoSize = true;
            this.lblDirectoryOutput.Location = new System.Drawing.Point(118, 13);
            this.lblDirectoryOutput.Name = "lblDirectoryOutput";
            this.lblDirectoryOutput.Size = new System.Drawing.Size(16, 13);
            this.lblDirectoryOutput.TabIndex = 1;
            this.lblDirectoryOutput.Text = "...";
            // 
            // Ildss
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 261);
            this.Controls.Add(this.lblDirectoryOutput);
            this.Controls.Add(this.lblDirectory);
            this.Name = "Ildss";
            this.Text = "ILDSS";
            this.Load += new System.EventHandler(this.Ildss_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDirectory;
        private System.Windows.Forms.FolderBrowserDialog DirectorySelect;
        private System.Windows.Forms.Label lblDirectoryOutput;
    }
}


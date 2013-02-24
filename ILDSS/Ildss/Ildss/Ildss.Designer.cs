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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ildss));
            this.lblDirectory = new System.Windows.Forms.Label();
            this.DirectorySelect = new System.Windows.Forms.FolderBrowserDialog();
            this.lblDirectoryOutput = new System.Windows.Forms.Label();
            this.btnManualIndex = new System.Windows.Forms.Button();
            this.btnMonitor = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.notifyTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.button2 = new System.Windows.Forms.Button();
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
            // btnManualIndex
            // 
            this.btnManualIndex.Location = new System.Drawing.Point(38, 51);
            this.btnManualIndex.Name = "btnManualIndex";
            this.btnManualIndex.Size = new System.Drawing.Size(158, 41);
            this.btnManualIndex.TabIndex = 2;
            this.btnManualIndex.Text = "Index Files";
            this.btnManualIndex.UseVisualStyleBackColor = true;
            this.btnManualIndex.Click += new System.EventHandler(this.btnManualIndex_Click);
            // 
            // btnMonitor
            // 
            this.btnMonitor.Location = new System.Drawing.Point(202, 51);
            this.btnMonitor.Name = "btnMonitor";
            this.btnMonitor.Size = new System.Drawing.Size(158, 39);
            this.btnMonitor.TabIndex = 3;
            this.btnMonitor.Text = "Monitor File System";
            this.btnMonitor.UseVisualStyleBackColor = true;
            this.btnMonitor.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(35, 115);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(99, 35);
            this.button3.TabIndex = 6;
            this.button3.Text = "Empty DB";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(230, 155);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 67);
            this.button1.TabIndex = 7;
            this.button1.Text = "Frequent Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // notifyTray
            // 
            this.notifyTray.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyTray.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyTray.Icon")));
            this.notifyTray.Text = "DocStore";
            this.notifyTray.Visible = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(35, 156);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 35);
            this.button2.TabIndex = 8;
            this.button2.Text = "To Storage";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // Ildss
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 261);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnMonitor);
            this.Controls.Add(this.btnManualIndex);
            this.Controls.Add(this.lblDirectoryOutput);
            this.Controls.Add(this.lblDirectory);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Ildss";
            this.Text = "ILDSS";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Ildss_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDirectory;
        private System.Windows.Forms.FolderBrowserDialog DirectorySelect;
        private System.Windows.Forms.Label lblDirectoryOutput;
        private System.Windows.Forms.Button btnManualIndex;
        private System.Windows.Forms.Button btnMonitor;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NotifyIcon notifyTray;
        private System.Windows.Forms.Button button2;
    }
}


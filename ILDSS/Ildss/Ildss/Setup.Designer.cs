namespace Ildss
{
    partial class Setup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setup));
            this.btnFinishSetup = new System.Windows.Forms.Button();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.btnDirectory = new System.Windows.Forms.Button();
            this.lblDirectory = new System.Windows.Forms.Label();
            this.dialogDirectoryBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFinishSetup
            // 
            this.btnFinishSetup.Location = new System.Drawing.Point(64, 206);
            this.btnFinishSetup.Name = "btnFinishSetup";
            this.btnFinishSetup.Size = new System.Drawing.Size(151, 36);
            this.btnFinishSetup.TabIndex = 0;
            this.btnFinishSetup.Text = "Finish Setup";
            this.btnFinishSetup.UseVisualStyleBackColor = true;
            this.btnFinishSetup.Click += new System.EventHandler(this.btnFinishSetup_Click);
            // 
            // txtDirectory
            // 
            this.txtDirectory.Location = new System.Drawing.Point(22, 28);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(250, 20);
            this.txtDirectory.TabIndex = 1;
            this.txtDirectory.Text = "F:\\";
            // 
            // btnDirectory
            // 
            this.btnDirectory.Location = new System.Drawing.Point(197, 54);
            this.btnDirectory.Name = "btnDirectory";
            this.btnDirectory.Size = new System.Drawing.Size(75, 23);
            this.btnDirectory.TabIndex = 2;
            this.btnDirectory.Text = "List...";
            this.btnDirectory.UseVisualStyleBackColor = true;
            this.btnDirectory.Click += new System.EventHandler(this.btnDirectory_Click);
            // 
            // lblDirectory
            // 
            this.lblDirectory.AutoSize = true;
            this.lblDirectory.Location = new System.Drawing.Point(19, 9);
            this.lblDirectory.Name = "lblDirectory";
            this.lblDirectory.Size = new System.Drawing.Size(92, 13);
            this.lblDirectory.TabIndex = 3;
            this.lblDirectory.Text = "Working Directory";
            // 
            // dialogDirectoryBrowser
            // 
            this.dialogDirectoryBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.dialogDirectoryBrowser.SelectedPath = "C:\\";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(116, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Thinkpad";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(36, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Dell";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblDirectory);
            this.Controls.Add(this.btnDirectory);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.btnFinishSetup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Setup";
            this.Text = "Setup";
            this.Load += new System.EventHandler(this.Setup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFinishSetup;
        private System.Windows.Forms.TextBox txtDirectory;
        private System.Windows.Forms.Button btnDirectory;
        private System.Windows.Forms.Label lblDirectory;
        private System.Windows.Forms.FolderBrowserDialog dialogDirectoryBrowser;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
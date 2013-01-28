namespace Ildss
{
    partial class Test
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
            this.btnReadEvents = new System.Windows.Forms.Button();
            this.txtNumEvents = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.myEvents = new System.Diagnostics.EventLog();
            this.lblTimeProcessed = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.myEvents)).BeginInit();
            this.SuspendLayout();
            // 
            // btnReadEvents
            // 
            this.btnReadEvents.Location = new System.Drawing.Point(466, 448);
            this.btnReadEvents.Name = "btnReadEvents";
            this.btnReadEvents.Size = new System.Drawing.Size(189, 50);
            this.btnReadEvents.TabIndex = 0;
            this.btnReadEvents.Text = "button1";
            this.btnReadEvents.UseVisualStyleBackColor = true;
            this.btnReadEvents.Click += new System.EventHandler(this.btnReadEvents_Click);
            // 
            // txtNumEvents
            // 
            this.txtNumEvents.Location = new System.Drawing.Point(284, 464);
            this.txtNumEvents.Name = "txtNumEvents";
            this.txtNumEvents.Size = new System.Drawing.Size(91, 20);
            this.txtNumEvents.TabIndex = 1;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1195, 394);
            this.listBox1.TabIndex = 2;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // myEvents
            // 
            this.myEvents.SynchronizingObject = this;
            // 
            // lblTimeProcessed
            // 
            this.lblTimeProcessed.AutoSize = true;
            this.lblTimeProcessed.Location = new System.Drawing.Point(776, 426);
            this.lblTimeProcessed.Name = "lblTimeProcessed";
            this.lblTimeProcessed.Size = new System.Drawing.Size(35, 13);
            this.lblTimeProcessed.TabIndex = 3;
            this.lblTimeProcessed.Text = "label1";
            // 
            // Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 554);
            this.Controls.Add(this.lblTimeProcessed);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.txtNumEvents);
            this.Controls.Add(this.btnReadEvents);
            this.Name = "Test";
            this.Text = "Test";
            ((System.ComponentModel.ISupportInitialize)(this.myEvents)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReadEvents;
        private System.Windows.Forms.TextBox txtNumEvents;
        private System.Windows.Forms.ListBox listBox1;
        private System.Diagnostics.EventLog myEvents;
        private System.Windows.Forms.Label lblTimeProcessed;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Ildss.Interface
{
    public partial class Setup : Form
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void Setup_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.directory != "")
            {
                txtDirectory.Text = Properties.Settings.Default.directory;
            }
            else
            {
                //txtDirectory.Text = @"E:\Desktop\ildss";
            }
        }

        private void btnFinishSetup_Click(object sender, EventArgs e)
        {
            // Check directory path is valid
            if (Directory.Exists(txtDirectory.Text) && Directory.Exists(textBox1.Text))
            {
                Properties.Settings.Default.directory = txtDirectory.Text;
                Properties.Settings.Default.storageDir = textBox1.Text;
                Properties.Settings.Default.firstrun = false; 
                Properties.Settings.Default.Save();

                //call the indexer asynchronously

                this.Hide();
                Main ildss = new Main();
                ildss.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid directory");
            }
        }

        private void btnDirectory_Click(object sender, EventArgs e)
        {
            if (dialogDirectoryBrowser.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = dialogDirectoryBrowser.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtDirectory.Text = @"F:\Documents\GitHub\docstore\TestDir";
            btnFinishSetup_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtDirectory.Text = @"C:\Users\Max\Documents\GitHub\docstore\TestDir";
            btnFinishSetup_Click(sender, e);
        }

        private void dialogDirectoryBrowser_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

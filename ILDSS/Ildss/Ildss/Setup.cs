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

namespace Ildss
{
    public partial class Setup : Form
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void Setup_Load(object sender, EventArgs e)
        {
            txtDirectory.Text = Properties.Settings.Default.directory;
        }

        private void btnFinishSetup_Click(object sender, EventArgs e)
        {
            // Check directory path is valid
            if (System.IO.Directory.Exists(txtDirectory.Text))
            {
                Properties.Settings.Default.directory = txtDirectory.Text;
                Properties.Settings.Default.firstrun = false; 
                Properties.Settings.Default.Save();

                this.Hide();
                Ildss ildss = new Ildss();
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
    }
}

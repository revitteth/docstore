using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ildss
{
    public partial class Ildss : Form
    {
        public Ildss()
        {
            InitializeComponent();
        }

        private void Ildss_Load(object sender, EventArgs e)
        {
            lblDirectoryOutput.Text = Properties.Settings.Default.directory;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            await Task.Factory.StartNew(() =>
            {
                new Indexer().IndexFiles(Properties.Settings.Default.directory);
                return "done";
            });

            //task.Wait();
            button1.Enabled = true;
            lblDirectory.Text = "Index Complete";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lblDirectory.Text = "WORKING BRO";
        }

    }
}

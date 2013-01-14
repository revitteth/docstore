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
    }
}

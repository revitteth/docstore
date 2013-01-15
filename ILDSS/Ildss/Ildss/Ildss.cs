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

        private async void btnManualIndex_Click(object sender, EventArgs e)
        {
            btnManualIndex.Enabled = false;
            btnManualIndex.Text = "Indexing...";

            // Start thread to do indexing
            /*
            await Task.Factory.StartNew(() =>
            {
                new Indexer().IndexFiles(Properties.Settings.Default.directory);
            });*/
            var progress = new Progress<int>(i => Console.WriteLine(i + " %"));
            await foo(progress);
            

            // Re enable button
            btnManualIndex.Enabled = true;
            btnManualIndex.Text = "Index";
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var progress = new Progress<int>(i => Console.WriteLine(i + " %"));
            await foo2(progress);

            btnMonitor.Enabled = false;
            btnMonitor.Text = "Monitoring...";
        }

        private Task foo(IProgress<int> onProgressPercentChanged)
        {
            return Task.Run( () =>
            {
                new Indexer().IndexFiles(Properties.Settings.Default.directory);
            });
        }

        private Task foo2(IProgress<int> onProgressPercentChanged)
        {
            return Task.Run(() =>
            {
                new DirectoryMonitor(Properties.Settings.Default.directory);
            });
        }

    }
}

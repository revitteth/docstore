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

using Ildss.Models;
using Ildss.Index;
using Ildss.Storage;

namespace Ildss.Interface
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Ildss_Load(object sender, EventArgs e)
        {
            lblDirectoryOutput.Text = Properties.Settings.Default.directory;
        }

        private async void btnManualIndex_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please close all open files within the directory " + Properties.Settings.Default.directory + " - Click OK to continue");

            // Disable button
            btnManualIndex.Enabled = false;
            btnManualIndex.Text = "Indexing...";

            var progress = new Progress<int>(i => Logger.write(i + " %"));
            await foo(progress);

            // Re enable button
            //btnManualIndex.Enabled = true;
            btnManualIndex.Text = "Indexed";

            var progress2 = new Progress<int>(i => Logger.write(i + " %"));
            await foo2(progress2);

            btnMonitor.Enabled = false;
            btnMonitor.Text = "Monitoring...";

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var progress = new Progress<int>(i => Logger.write(i + " %"));
            await foo2(progress);

            btnMonitor.Enabled = false;
            btnMonitor.Text = "Monitoring...";
        }

        private Task foo(IProgress<int> onProgressPercentChanged)
        {
            return Task.Run( () =>
            {
                KernelFactory.Instance.Get<IIndexer>("Initial").IndexFiles(Properties.Settings.Default.directory);
            });
        }

        private Task foo2(IProgress<int> onProgressPercentChanged)
        {
            return Task.Run(() =>
            {
                KernelFactory.Instance.Get<IMonitor>().Monitor(Properties.Settings.Default.directory);
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            foreach (var d in fic.Documents)
            {
                fic.Documents.Remove(d);
            }
            fic.SaveChanges();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
                {
                    KernelFactory.Instance.Get<IIndexer>("Frequent").IndexFiles(Properties.Settings.Default.directory);
                });
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // needs rewriting
            KernelFactory.Instance.Get<IStorage>().Store();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // needs rewriting
            KernelFactory.Instance.Get<IStorage>().StoreAll();
        }

    }
}

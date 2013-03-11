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

        private void Ildss_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyTray.Visible = true;
                notifyTray.ShowBalloonTip(500);
                this.Hide();

                notifyTray.BalloonTipTitle = "APP Hidden";
                notifyTray.BalloonTipText = "Your application has been minimized to the taskbar.";
                notifyTray.ShowBalloonTip(3000);
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyTray.Visible = false;
            }
        }

        private async void btnManualIndex_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please close all open files within the directory " + Properties.Settings.Default.directory + " - Click OK to continue");

            // Disable button
            btnManualIndex.Enabled = false;
            btnManualIndex.Text = "Indexing...";

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
            Task.Run(() =>
                {
                    // Thread problem - write all into a list, release fic then process them
                    // KernelFactory.Instance.Get<>();
                    var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                    foreach (var d in fic.Documents)
                    {
                        Console.WriteLine("Moving: " + d.DocumentHash + " to storage");
                        KernelFactory.Instance.Get<IStorage>().MoveToStorage(d.DocPaths.First().path, d.DocumentHash);
                    }
                });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                foreach (var d in fic.Documents)
                {
                    KernelFactory.Instance.Get<IStorage>().RetrieveFromStorage(d.DocPaths.First().path, d.DocumentHash);
                }
            });
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MahApps.Metro.Controls;

using Ildss;
using Ildss.Index;
using Ildss.Models;
using Ildss.Storage;
using System.Drawing;


namespace IldssUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        
        public MainWindow()
        {
            InitializeComponent();
            Settings.InitSettings();

            lblIndexFrequency.Text = lblIndexFrequency.Text + Settings.getIndexInterval() / 60000 + " minutes";

            if (Settings.getFirstRun())
            {
                TabControl.SetIsSelected(tabSettings, true);
                this.WindowState = WindowState.Normal;
            }
            else
            {
                txtWorkingDir.IsEnabled = false;
                txtStorageDir.IsEnabled = false;
                btnFinish.IsEnabled = false;
                btnLoadDefaults.IsEnabled = false;
                btnIndex.IsEnabled = false;
                btnMonitor.IsEnabled = false;
                txtWorkingDir.Text = Settings.getWorkingDir();
                txtStorageDir.Text = Settings.getStorageDir();
                this.Hide();
                Task.Run(() =>
                {
                    KernelFactory.Instance.Get<IEventManager>("Index");
                    KernelFactory.Instance.Get<IMonitor>().Monitor(Settings.getWorkingDir());
                });
            }

            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("../../cloud.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        private async void btnIndex_Click(object sender, RoutedEventArgs e)
        {
            btnIndex.Content = "Working...";
            await Task.Run(() =>
            {
                KernelFactory.Instance.Get<IEventManager>("Index");
            });
        }

        private async void btnMonitor_Click(object sender, RoutedEventArgs e)
        {
            await foo2();
            btnMonitor.Content = "Working...";
        }

        private Task foo2()
        {
            return Task.Run(() =>
            {
                KernelFactory.Instance.Get<IMonitor>().Monitor(Settings.getWorkingDir());
            });
        }

        private void btnEmptyDb_Click(object sender, RoutedEventArgs e)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            foreach (var d in fic.Documents)
            {
                fic.Documents.Remove(d);
            }
            foreach (var b in fic.Backups)
            {
                fic.Backups.Remove(b);
            }
            fic.SaveChanges();
        }

        private Task IncrementalBackup()
        {
            return Task.Run(() =>
                {
                    KernelFactory.Instance.Get<IStorage>().StoreIncr();
                });
        }

        private async void btnIncremental_Click(object sender, RoutedEventArgs e)
        {
            btnIncremental.Content = "In Progress...";
            await IncrementalBackup();
            btnIncremental.Content = "Incr Backup";
        }

        private Task FullBackup()
        {
            return Task.Run(() =>
            {
                KernelFactory.Instance.Get<IStorage>().StoreFull();
            });
        }

        private async void btnFullBackup_Click(object sender, RoutedEventArgs e)
        {
            btnFullBackup.Content = "In Progress...";
            await FullBackup();
            btnFullBackup.Content = "Full Backup";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            btnExportCSV.Content = "Exporting...";
            await Task.Run(() =>
                {
                    StatsExport.Export();
                });
            btnExportCSV.Content = "Export CSV";
        }

        private void txtWorkingDir_LostFocus(object sender, EventArgs e)
        {
            if (Directory.Exists(txtWorkingDir.Text))
            {
                txtWorkingDir.Background = System.Windows.Media.Brushes.LightGreen;
                txtStorageDir.Focus();
            }
            else
            {
                txtWorkingDir.Background = System.Windows.Media.Brushes.LightPink;
            }
        }

        private void txtStorageDir_LostFocus(object sender, EventArgs e)
        {
            if (Directory.Exists(txtStorageDir.Text))
            {
                txtStorageDir.Background = System.Windows.Media.Brushes.LightGreen;
                btnFinish.Focus();
            }
            else
            {
                txtStorageDir.Background = System.Windows.Media.Brushes.LightPink;
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(txtStorageDir.Text) & Directory.Exists(txtWorkingDir.Text))
            {
                Settings.setFirstRun(false);
                Settings.setStorageDir(txtStorageDir.Text);
                Settings.setWorkingDir(txtWorkingDir.Text);
                btnFinish.IsEnabled = false;
                btnLoadDefaults.IsEnabled = false;
                txtWorkingDir.IsEnabled = false;
                txtStorageDir.IsEnabled = false;
                TabControl.SetIsSelected(tabDashboard, true);
            }
        }

        private void btnLoadDefaults_Click(object sender, RoutedEventArgs e)
        {
            txtWorkingDir.Text = Settings.getWorkingDir();
            txtStorageDir.Text = Settings.getStorageDir();
        }

        private void btnSetInterval_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtInterval.Text) > 1 & Convert.ToInt32(txtInterval.Text) < 6000)
            {
                Settings.setIndexInterval(Convert.ToInt32(txtInterval.Text) * 60000);
            }
            lblIndexFrequency.Text = "Index Runs Every " + Settings.getIndexInterval() / 60000 + " minutes";
        }

        private async void btnS3_Click(object sender, RoutedEventArgs e)
        {
            btnS3.IsEnabled = false;
            int size = await CloudInterface.Program.UploadFileAsync(@"E:\Dropbox\Camera Uploads\2012-03-17 13.01.50.jpg");
            btnS3.IsEnabled = true;
            btnS3.Content = size.ToString();
        }

    }
}

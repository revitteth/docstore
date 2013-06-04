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
using CloudInterface;

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
            //Settings.Default.InitialiseSettings();

            if (Settings.Default.FirstRun)
            {
                TabControl.SetIsSelected(tabSettings, true);
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.Hide();
                Task.Run(() =>
                {
                    KernelFactory.Instance.Get<IEventManager>("Index");
                    KernelFactory.Instance.Get<IMonitor>().Monitor(Settings.Default.WorkingDir);
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

        private Task Monitor()
        {
            return Task.Run(() =>
            {
                KernelFactory.Instance.Get<IMonitor>().Monitor(Settings.Default.WorkingDir);
            });
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

        private void btnS3_Click(object sender, RoutedEventArgs e)
        {
            btnS3.IsEnabled = false;
            // need an index here - also ensure all files closed or make not as upload will fail
            KernelFactory.Instance.Get<IStorage>("Cloud").StoreIncrAsync();
            //KernelFactory.Instance.Get<IStorage>("Cloud").RemoveUnusedDocumentsAsync(); 
            btnS3.IsEnabled = true;
        }
    }
}

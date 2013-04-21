using System;
using System.Collections.Generic;
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

        }

        private async void btnIndex_Click(object sender, RoutedEventArgs e)
        {
            btnIndex.Content = "Working...";
            await Task.Run(() =>
            {
                KernelFactory.Instance.Get<IEventManager>("Index");
            });
            btnIndex.Content = "Index";
        }

        private async void btnMonitor_Click(object sender, RoutedEventArgs e)
        {
            var progress = new Progress<int>(i => Logger.write(i + " %"));
            await foo2(progress);
            btnMonitor.Content = "monitoring";
        }

        private Task foo2(IProgress<int> onProgressPercentChanged)
        {
            return Task.Run(() =>
            {
                KernelFactory.Instance.Get<IMonitor>().Monitor(Settings.WorkingDir);
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


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Ildss;
using Ildss.Index;
using Ildss.Models;
using Ildss.Storage;

namespace IldssUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnIndex_Click(object sender, RoutedEventArgs e)
        {
            var progress = new Progress<int>(i => Logger.write(i + " %"));
            await foo(progress);
            btnIndex.Content = "done";
        }

        private async void btnMonitor_Click(object sender, RoutedEventArgs e)
        {
            var progress = new Progress<int>(i => Logger.write(i + " %"));
            await foo2(progress);
            btnMonitor.Content = "monitoring";
        }

        private Task foo(IProgress<int> progress)
        {
            return Task.Run(() =>
            {
                KernelFactory.Instance.Get<IIndexer>("Initial").IndexFiles(Settings.WorkingDir);
            });
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

        private void btnIncremental_Click(object sender, RoutedEventArgs e)
        {
            KernelFactory.Instance.Get<IStorage>().StoreIncr();
        }

        private void btnFullBackup_Click(object sender, RoutedEventArgs e)
        {
            KernelFactory.Instance.Get<IStorage>().StoreFull();
        }


    }
}

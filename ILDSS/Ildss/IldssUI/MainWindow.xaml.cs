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
using System.Diagnostics;
using Ildss.Properties;

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
            this.Hide();
            Task.Run(() =>
            {
                KernelFactory.Instance.Get<IEventManager>("Index");
                KernelFactory.Instance.Get<IMonitor>().Monitor();
            });

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            btnExportCSV.Content = "Exporting...";
            await Task.Run(() =>
                {
                    StatsExport.Export();
                });
            btnExportCSV.Content = "Export CSV";
        }

        private async void btnS3_Click(object sender, RoutedEventArgs e)
        {
            btnS3.IsEnabled = false;
            await KernelFactory.Instance.Get<IStorage>("Cloud").StoreIncrAsync();
            btnS3.IsEnabled = true;
        }

        private async void btnIntelligence_Click(object sender, RoutedEventArgs e)
        {
            btnIntelligence.IsEnabled = false;
            await Task.Run(() =>
                {
                    KernelFactory.Instance.Get<IStorage>("Cloud").RemoveUnusedDocumentsAsync();
                });
            btnIntelligence.IsEnabled = true;
        }

        private async void docList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRetrieve.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnOpenLocation.IsEnabled = false;

            var path = docList.SelectedItem as DocPath;
            if (path != null)
            {
                if (path.Document.Status == Enums.DocStatus.Current | path.Document.Status == Enums.DocStatus.Indexed)
                {
                    btnOpenLocation.IsEnabled = true;
                }

                UpdateVersionListAsync(path);
            }
        }

        private void verList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisableButtons();

            if (e.AddedItems.Count != 0)
            {
                var version = e.AddedItems[0] as DocVersion;
                if (version.Document.Status == Enums.DocStatus.Archived | (version.Document.Status == Enums.DocStatus.Current))
                {
                    EnableButtons();
                }
            }
        }

        private async void btnRetrieve_Click(object sender, RoutedEventArgs e)
        {
            var path = docList.SelectedItem as DocPath;
            var version = verList.SelectedItem as DocVersion;

            DisableButtons();

            // initialise s3 download of version.versionkey
            var cs = KernelFactory.Instance.Get<IStorage>();
            await Task.Run(() =>
                {
                    cs.Retrieve(version.VersionKey, path.Path, path.Document);
                });

            UpdateDocListAsync();
            EnableButtons();
        }

        private async void btnRetrieveAll_Click(object sender, RoutedEventArgs e)
        {
            var path = docList.SelectedItem as DocPath;
            var version = verList.SelectedItem as DocVersion;


            DisableButtons();

            // initialise s3 download of version.versionkey
            var cs = KernelFactory.Instance.Get<IStorage>();
            await Task.Run(() =>
            {
                foreach (var p in path.Document.DocPaths)
                {
                    cs.Retrieve(version.VersionKey, p.Path, p.Document);
                }  
            });

            UpdateDocListAsync();
            EnableButtons();
        }

        private void txtSearch_Changed(object sender, TextChangedEventArgs e)
        {
            verList.ItemsSource = null;

            if (txtSearch.Text.Count() > 2)
            {
                // search the db
                UpdateDocListAsync(txtSearch.Text);
            }
            else
            {
                UpdateDocListAsync();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(this, "This file version will be delete from cloud storage forever, do you wish to continue?",
                "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                // Yes code here - use downloader
            }
            else
            {
                // No code here
            } 
        }

        private async void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Run(() =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                        {
                            if (!tabRetrieve.IsSelected)
                            {
                                UpdateDocListAsync();
                            }
                            if (tabDashboard.IsSelected)
                            {
                                CalculateUsage();
                            }

                            txtSearch.Clear();
                        }));
                });
        }

        private void UpdateDocListAsync()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                docList.ItemsSource = KernelFactory.Instance.Get<IFileIndexContext>().DocPaths.Take(20).ToList();
            }));
        }

        private void UpdateDocListAsync(string query)
        {
            docList.ItemsSource = KernelFactory.Instance.Get<IFileIndexContext>().DocPaths.
                        Where(i => i.Path.Contains(txtSearch.Text)).Take(20).ToList();
        }

        private void UpdateVersionListAsync(DocPath path)
        {
            verList.ItemsSource = KernelFactory.Instance.Get<IFileIndexContext>().DocVersions.
                                Where(i => i.Document.DocPaths.Any(j => j.Path == path.Path)).
                                OrderByDescending(k => k.DocEventTime).Take(20).ToList();
        }

        private void CalculateUsage()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {

                }));
        }

        private void docList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListView;
            var path = item.SelectedItem as DocPath;
            Process.Start(path.Path);
        }

        private void btnOpenLocation_Click(object sender, RoutedEventArgs e)
        {
            var path = docList.SelectedItem as DocPath;
            Process.Start("Explorer", string.Format("/Select, {0}", path.Path));
        }

        private void DisableButtons()
        {
            btnRetrieve.IsEnabled = false;
            btnRetrieveAll.IsEnabled = false;
            btnDelete.IsEnabled = false;                                                               
        }

        private void EnableButtons()
        {
            btnRetrieve.IsEnabled = true;
            btnRetrieveAll.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }
    }
}

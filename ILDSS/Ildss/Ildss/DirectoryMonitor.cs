using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Reactive.Linq;
using System.Reactive;

namespace Ildss
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class DirectoryMonitor : IMonitor
    {
        private DateTime LastChange;

        public void Monitor (string path)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(path, "*.*");

            fsw = new FileSystemWatcher(path);
            var fIndexer = KernelFactory.Instance.Get<IIndexer>("Frequent");
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            IObservable<EventPattern<FileSystemEventArgs>> fswCreated = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Created");
            fswCreated.Subscribe(
                pattern =>
                {
                    fIndexer.CheckDatabase();
                    fIndexer.IndexFile(pattern.EventArgs.FullPath);
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    fIndexer.CheckDatabase();
                    fIndexer.IndexFile(pattern.EventArgs.FullPath);
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswRenamed = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    fIndexer.CheckDatabase();
                    fIndexer.IndexFile(pattern.EventArgs.FullPath);
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    fIndexer.CheckDatabase();
                    fIndexer.IndexFile(pattern.EventArgs.FullPath);
                }
            );
        }
    }
}

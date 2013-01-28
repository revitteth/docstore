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
        private string _changedOffice = "";

        public void Monitor (string path)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(path, "*.*");

            fsw = new FileSystemWatcher(path);
            var fIndexer = KernelFactory.Instance.Get<IIndexChecker>();
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            IObservable<EventPattern<FileSystemEventArgs>> fswCreated = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Created");
            fswCreated.Subscribe(
                pattern =>
                {
                    //Thread.Sleep(100);
                    if (!pattern.EventArgs.Name.Contains(".tmp") && !pattern.EventArgs.Name.Contains(".TMP"))
                    {
                        if (!(File.GetAttributes(pattern.EventArgs.FullPath) == FileAttributes.Directory))
                        {
                            fIndexer.CheckDatabase(pattern.EventArgs.FullPath, "Created");
                        }
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    if (!pattern.EventArgs.Name.Contains(".tmp") && !pattern.EventArgs.Name.Contains(".TMP"))
                    {
                        fIndexer.CheckDatabase(pattern.EventArgs.FullPath, "Deleted");
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    if (!pattern.EventArgs.OldName.Contains(".tmp") && !pattern.EventArgs.Name.Contains(".tmp"))
                    {
                        fIndexer.CheckDatabase(pattern.EventArgs.FullPath, "Renamed", pattern.EventArgs.OldFullPath);
                    }
                    else
                    {
                        if (pattern.EventArgs.Name.Contains(".tmp"))
                        {
                            // the new file is a temp save the old files name
                            _changedOffice = pattern.EventArgs.OldFullPath;
                        }
                        else if (pattern.EventArgs.OldName.Contains(".tmp"))
                        {
                            // temp being renamed back to old document name
                            fIndexer.CheckDatabase(_changedOffice, "Changed");
                        }
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    if (!pattern.EventArgs.Name.Contains(".tmp") && !pattern.EventArgs.Name.Contains(".TMP"))
                    {
                        if (!(File.GetAttributes(pattern.EventArgs.FullPath) == FileAttributes.Directory))
                        {
                            fIndexer.CheckDatabase(pattern.EventArgs.FullPath, "Changed");
                        }
                    }
                }
            );
        }
    }
}

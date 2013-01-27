using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Security.Permissions;
using System.Reactive.Linq;
using System.Reactive;

namespace Ildss
{
    class TestMonitor : IMonitor
    {
        private string _changedOffice = "";

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

                    if (!pattern.EventArgs.Name.Contains(".tmp"))
                    {
                        Console.WriteLine(pattern.EventArgs.Name + " Created");
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    if (!pattern.EventArgs.Name.Contains(".tmp"))
                    {
                        Console.WriteLine(pattern.EventArgs.Name + " Deleted");
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    if (!pattern.EventArgs.OldName.Contains(".tmp") && !pattern.EventArgs.Name.Contains(".tmp"))
                    {
                        Console.WriteLine(pattern.EventArgs.OldName + " Renamed to " + pattern.EventArgs.Name);
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
                            Console.WriteLine(_changedOffice + " changed");
                        }
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    if (!pattern.EventArgs.Name.Contains(".tmp"))
                    {
                        Console.WriteLine("Changed: " + pattern.EventArgs.Name);
                    }
                }
            );
        }
    }
}
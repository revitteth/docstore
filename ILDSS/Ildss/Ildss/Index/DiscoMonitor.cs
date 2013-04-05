using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace Ildss.Index
{
    class DiscoMonitor : IMonitor
    {
        public void Monitor(string path)
        {
            var fsw = new FileSystemWatcher(path);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            // REMOVE THIS FOR NON TIME BASED USAGE!
            Thread.Sleep(10);

            Logger.write("Test Monitor Started");

            IObservable<EventPattern<FileSystemEventArgs>> fswCreated = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Created");
            fswCreated.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    Logger.write("FSW Event - Created " + pe.Name);
                }
            );
            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswCreated.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    Logger.write("FSW Event - Changed " + pe.Name);
                }
            );
            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswCreated.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    Logger.write("FSW Event - Deleted " + pe.Name);
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    Logger.write("FSW Event - Renamed " + pe.OldName + " to " + pe.Name);
                }
            );
        }
    }
}

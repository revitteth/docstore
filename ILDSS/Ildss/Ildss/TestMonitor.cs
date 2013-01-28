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
        private List<string> _ignoreFiles = new List<string> { ".tmp", ".TMP" };

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
                    var pe = pattern.EventArgs;
                    var fi = new FileInfo(pe.FullPath);
                    if (!_ignoreFiles.Any(pe.Name.Contains) && fi.Extension != "")
                    {
                        var finfomaniac = new FileInfo(pe.FullPath);
                        Console.WriteLine(pe.Name + " Created");
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    var fi = new FileInfo(pe.FullPath);
                    if (!_ignoreFiles.Any(pe.Name.Contains) && fi.Extension != "")
                    {
                        Console.WriteLine(pe.Name + " Deleted");
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    var fi = new FileInfo(pe.FullPath);
                    if (_ignoreFiles.Any(pe.Name.Contains) | !pe.Name.Contains("."))
                    {
                        _changedOffice = pe.OldFullPath;
                        Console.WriteLine("Saving old office name " + pe.OldName);
                        //Console.WriteLine(pe.OldName + " Renamed to " + pe.Name);
                    }
                    else if (_ignoreFiles.Any(pe.OldName.Contains) | !pe.OldName.Contains("."))
                    {
                        if (fi.FullName == _changedOffice)
                        {
                            // it was just an update to a file which saves using temp files
                            // call changed event on the file.
                            Console.WriteLine("changed a temp saver: " + _changedOffice);
                        }
                        else
                        {
                            Console.WriteLine("weird: " + _changedOffice + " -- " + fi.Name);
                        }
                    }
                    else
                    {
                        // conventional rename
                        Console.WriteLine(pe.OldName + " Renamed to " + pe.Name);
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    var fi = new FileInfo(pe.FullPath);
                    if (!_ignoreFiles.Any(pe.Name.Contains) && fi.Extension != "")
                    {
                        Console.WriteLine("Changed: " + pe.Name);
                    }
                }
            );
        }
    }
}
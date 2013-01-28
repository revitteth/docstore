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
                    var fi = new FileInfo(pattern.EventArgs.FullPath);
                    if (!_ignoreFiles.Any(pattern.EventArgs.Name.Contains) && fi.Extension != "")
                    {
                        var finfomaniac = new FileInfo(pattern.EventArgs.FullPath);
                        Console.WriteLine(pattern.EventArgs.Name + " Created");
                        Console.WriteLine("Extension " +  finfomaniac.Extension);
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    var fi = new FileInfo(pattern.EventArgs.FullPath);
                    if (!_ignoreFiles.Any(pattern.EventArgs.Name.Contains) && fi.Extension != "")
                    {
                        Console.WriteLine(pattern.EventArgs.Name + " Deleted");
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var fi = new FileInfo(pattern.EventArgs.FullPath);
                    if (_ignoreFiles.Any(pattern.EventArgs.Name.Contains) | !pattern.EventArgs.Name.Contains("."))
                    {
                        _changedOffice = pattern.EventArgs.OldFullPath;
                        Console.WriteLine("Saving old office name " + pattern.EventArgs.OldName);
                        //Console.WriteLine(pattern.EventArgs.OldName + " Renamed to " + pattern.EventArgs.Name);
                    }
                    else if (_ignoreFiles.Any(pattern.EventArgs.OldName.Contains) | !pattern.EventArgs.OldName.Contains("."))
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
                        Console.WriteLine(pattern.EventArgs.OldName + " Renamed to " + pattern.EventArgs.Name);
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    var fi = new FileInfo(pattern.EventArgs.FullPath);
                    if (!_ignoreFiles.Any(pattern.EventArgs.Name.Contains) && fi.Extension != "")
                    {
                        Console.WriteLine("Changed: " + pattern.EventArgs.Name);
                    }
                }
            );
        }
    }
}
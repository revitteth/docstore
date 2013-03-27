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

using Ildss.Index;

namespace Ildss.Index
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class DirectoryMonitor : IMonitor
    {
        private string _changedOffice = "";
        private IList<string> _ignoreFiles = KernelFactory.Instance.Get<ISettings>().ignoredExtensions;

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
                    if (!_ignoreFiles.Any(pe.Name.Contains) & pe.Name.Contains("."))
                    {
                        var fi = new FileInfo(pe.FullPath);
                        if (!(File.GetAttributes(pe.FullPath) == FileAttributes.Directory))
                        {
                            if (!_ignoreFiles.Any(pe.Name.Contains) && fi.Extension != "")
                            {
                                fIndexer.RespondToEvent(pe.FullPath, "Created");
                            }
                        }
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    if (!_ignoreFiles.Any(pe.Name.Contains) & pe.Name.Contains("."))
                    {
                        fIndexer.RespondToEvent(pe.FullPath, "Deleted");
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    var fi = new FileInfo(pe.FullPath);
                    if (File.GetAttributes(pe.FullPath) == FileAttributes.Directory)
                    {
                        fIndexer.RespondToEvent(pe.FullPath, "Renamed", pe.OldFullPath);
                    }
                    else
                    {
                        if (_ignoreFiles.Any(pe.Name.Contains) | !pe.Name.Contains("."))
                        {
                            _changedOffice = pe.OldFullPath;
                        }
                        else if (_ignoreFiles.Any(pe.OldName.Contains) | !pe.OldName.Contains("."))
                        {
                            if (fi.FullName == _changedOffice)
                            {
                                fIndexer.RespondToEvent(_changedOffice, "Changed");
                            }
                        }
                        else
                        {
                            fIndexer.RespondToEvent(pe.FullPath, "Renamed", pe.OldFullPath);
                        }
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    var fi = new FileInfo(pe.FullPath);
                    if (!_ignoreFiles.Any(pe.Name.Contains) & fi.Extension != "")
                    {
                        if (!(File.GetAttributes(pe.FullPath) == FileAttributes.Directory))
                        {
                            fIndexer.RespondToEvent(pe.FullPath, "Changed");
                        }
                    }
                }
            );
        }
    }
}

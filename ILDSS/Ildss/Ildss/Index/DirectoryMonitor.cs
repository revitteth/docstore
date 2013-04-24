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
        private IList<string> _ignoredFiles = Settings.getIgnoredExtensions();

        public void Monitor (string path)
        {
            Logger.write("Directory Monitor Started");

            var fsw = new FileSystemWatcher(path);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;
            //fsw.InternalBufferSize = 32768;

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;

                    try
                    {
                        if (!_ignoredFiles.Any(pe.OldFullPath.Contains) & !_ignoredFiles.Any(pe.FullPath.Contains))
                        {
                            bool isDir = false;
                            if (File.GetAttributes(pe.OldFullPath) == FileAttributes.Directory | File.GetAttributes(pe.FullPath) == FileAttributes.Directory)
                            {
                                isDir = true;
                            }
                            var fs = new FSEvent() { Type = Settings.EventType.Rename, FileInf = new FileInfo(pe.FullPath), OldPath = pe.OldFullPath, isDirectory = isDir };
                            // potential thread safety issue here!!!!
                            KernelFactory.Instance.Get<IEventManager>("Index").AddEvent(fs);

                            Logger.write("FSW Event - Renamed " + pe.Name);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.write("Rename Exception: Now called:" + pe.Name + " Error msg: " + e.Message);
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswCreated = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Created");
            fswCreated.Subscribe(
                pattern =>
                {
                    KernelFactory.Instance.Get<IEventManager>("Index").IndexRequired = true;
                }

            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    KernelFactory.Instance.Get<IEventManager>("Index").IndexRequired = true;
                }

            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    KernelFactory.Instance.Get<IEventManager>("Index").IndexRequired = true;
                }

            );

        }
    }
}

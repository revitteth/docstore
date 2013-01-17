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
    public class DirectoryMonitor : IDirectoryMonitor
    {

        public void MonitorFileSystem (string path)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(path, "*.*");

            fsw = new FileSystemWatcher(path);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            //Indexer indexer = new Indexer();  IF I NEED THIS USE KERNELFACTORY AND GET IT AS AN INSTANCE

            IObservable<EventPattern<FileSystemEventArgs>> fswCreated = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Created");
            fswCreated.Subscribe(
                pattern => {
                    if ((File.GetAttributes(pattern.EventArgs.FullPath) & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        //do nowt - CURE temp files problem on this one geoff!!!! when making shortcuts.
                    }
                    else
                    {
                        DocEvent de = new DocEvent()
                        {
                            name = pattern.EventArgs.Name,
                            path = pattern.EventArgs.FullPath,
                            type = WatcherChangeTypes.Created.ToString()
                        };
                        KernelFactory.Instance.Get<IEventQueue>().AddEvent(de);
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var hash = KernelFactory.Instance.Get<IHash>().HashFile(pattern.EventArgs.FullPath);
                    var fic = KernelFactory.Instance.Get<IFileIndexContainer>();

                    Console.WriteLine(hash);

                    // FIRST JOB IS TO CHANGE THE FILE NAME IN PATHS - NO WONDER IT DOESNT WORK!!!

                    Document ddd = fic.Documents.First(i => i.DocPaths.Any(j => j.path == pattern.EventArgs.OldFullPath) == true);

                    DocEvent de = new DocEvent()
                    {
                        name = pattern.EventArgs.Name,
                        old_name = pattern.EventArgs.OldName,
                        path = pattern.EventArgs.FullPath,
                        old_path = pattern.EventArgs.OldFullPath,
                        type = WatcherChangeTypes.Renamed.ToString(),
                        Document = ddd
                    };
                    KernelFactory.Instance.Get<IEventQueue>().AddEvent(de);
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern => {

                    // NEED TO DO MORE HERE! DELETE AT LEAST PATH AND IF LAST PATH, DOCUMENT AS WELL

                    DocEvent de = new DocEvent()
                    {
                        name = pattern.EventArgs.Name,
                        path = pattern.EventArgs.FullPath,
                        type = WatcherChangeTypes.Deleted.ToString()
                    };
                    KernelFactory.Instance.Get<IEventQueue>().AddEvent(de);
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            var thingy = fswChanged.Subscribe(
                pattern => {
                    var fi = new FileInfo(pattern.EventArgs.FullPath);

                    var fic = KernelFactory.Instance.Get<IFileIndexContainer>();

                    var updatedDocument = fic.Documents.First(i => i.DocPaths.Any(j=>j.path == pattern.EventArgs.FullPath) == true);
                    //updatedDocument.DocumentHash = KernelFactory.Instance.Get<IHash>().HashFile(pattern.EventArgs.FullPath);
                    Console.WriteLine(updatedDocument);

                    DocEvent de = new DocEvent()
                    {
                        name = pattern.EventArgs.Name,
                        path = pattern.EventArgs.FullPath,
                        type = WatcherChangeTypes.Changed.ToString(),
                        Document = updatedDocument,
                        last_access = fi.LastAccessTime,
                        last_write = fi.LastWriteTime
                    };

                    if (fic.DocEvents.Any(i => i.IsEqual(de)))
                    {
                        // already exists pal
                    }
                    else
                    {
                        KernelFactory.Instance.Get<IEventQueue>().AddEvent(de);
                    }
                }
            );
        }
    }
}

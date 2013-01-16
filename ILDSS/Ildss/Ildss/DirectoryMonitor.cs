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
    class DirectoryMonitor
    {
        public DirectoryMonitor(string path)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(path, "*.*");

            fsw = new FileSystemWatcher(path);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            MonitorFileSystem(fsw);
        }

        private static void MonitorFileSystem (FileSystemWatcher fsw)
        {              
            Indexer indexer = new Indexer();

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
                            date_time = (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond)),
                            name = pattern.EventArgs.Name,
                            path = pattern.EventArgs.FullPath,
                            type = WatcherChangeTypes.Created.ToString()
                        };
                        //IldssModule.evQueue.AddEvent(de);
                        //indexer.IndexFile(pattern.EventArgs.FullPath);
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var hashting = new Hash().HashFile(pattern.EventArgs.FullPath);
                    FileIndexContainer fic = new FileIndexContainer();

                    Document ddd = fic.Documents.First(i => i.DocumentHash == hashting);

                    //Console.WriteLine(ddd.DocumentHash);

                    DocEvent de = new DocEvent()
                    {
                        date_time = (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond)),
                        name = pattern.EventArgs.Name,
                        old_name = pattern.EventArgs.OldName,
                        path = pattern.EventArgs.FullPath,
                        old_path = pattern.EventArgs.OldFullPath,
                        type = WatcherChangeTypes.Renamed.ToString(),
                        Document = ddd
                    };
                    //EventQueue.AddEvent(de);
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern => {

                    // NEED TO DO MORE HERE! DELETE AT LEAST PATH AND IF LAST PATH, DOCUMENT AS WELL

                    DocEvent de = new DocEvent()
                    {
                        date_time = (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond)),
                        name = pattern.EventArgs.Name,
                        path = pattern.EventArgs.FullPath,
                        type = WatcherChangeTypes.Deleted.ToString()
                    };
                    //EventQueue.AddEvent(de);
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            var thingy = fswChanged.Subscribe(
                pattern => {
                    DocEvent de = new DocEvent()
                    {
                        date_time = (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond)),
                        name = pattern.EventArgs.Name,
                        path = pattern.EventArgs.FullPath,
                        type = WatcherChangeTypes.Changed.ToString()                         
                    };
                    //EventQueue.AddEvent(de);
                }
            );
        }
    }
}

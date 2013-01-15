using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Ildss
{
    public class FileWatcher
    {
        public FileWatcher(string path, string filter, TimeSpan throttle)
        {
            Path = path;
            Filter = filter;
            Throttle = throttle;
        }
 
        public string Path { get; private set; }
        public string Filter { get; private set; }
        public TimeSpan Throttle { get; private set; }
 
        public IObservable<System.IO.FileChangedEvent> GetObservable()
        {
            return Observable.Create<FileChangedEvent>(observer =>
            {
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Path, Filter) { EnableRaisingEvents = true };
                FileSystemEventHandler created = (_, __) => observer.OnNext(new FileChangedEvent());
                FileSystemEventHandler changed = (_, __) => observer.OnNext(new FileChangedEvent());
                RenamedEventHandler renamed = (_, __) => observer.OnNext(new FileChangedEvent());
                FileSystemEventHandler deleted = (_, __) => observer.OnNext(new FileChangedEvent());
                ErrorEventHandler error = (_, errorArg) => observer.OnError(errorArg.GetException());
 
                fileSystemWatcher.Created += created;
                fileSystemWatcher.Changed += changed;
                fileSystemWatcher.Renamed += renamed;
                fileSystemWatcher.Deleted += deleted;
                fileSystemWatcher.Error += error;
 
                return () =>
                {
                    fileSystemWatcher.Created -= created;
                    fileSystemWatcher.Changed -= changed;
                    fileSystemWatcher.Renamed -= renamed;
                    fileSystemWatcher.Deleted -= deleted;
                    fileSystemWatcher.Error -= error;
                    fileSystemWatcher.Dispose();
                };
 
            }).Throttle(Throttle);
        }
    }
 

}
 
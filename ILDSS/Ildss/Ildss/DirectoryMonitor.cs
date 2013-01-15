using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Reactive.Linq;
using System.Reactive;


namespace Ildss
{
    class DirectoryMonitor
    {
        private FileSystemWatcher watcher;
        private static string lastCreated;

        public DirectoryMonitor(string path)
        {
            // Set watcher to point to ILDSS directory
            watcher = new FileSystemWatcher(path);
            watcher.NotifyFilter = NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.FileName |
                                   NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = true;

            // Event Handlers
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching
            watcher.EnableRaisingEvents = true;

            // Until thread is killed by program.cs
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {              
            Indexer indexer = new Indexer();

            // work out what has been changed update database accordingly
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    // fully index the file
                    indexer.IndexFile(e.FullPath);
                    lastCreated = e.FullPath;
                    return;
                    //break;
                case WatcherChangeTypes.Changed:
                    // keep trying every 2 minutes until it works

                    // Add event to existing file
                    if(e.FullPath != lastCreated)
                    indexer.RegisterEvent(e);
                    break;
                case WatcherChangeTypes.Deleted:
                    // remove Document entry. And events?
                    Console.WriteLine("deleted");
                    break;
            }


            // REGULAR EXPRESSION TO GET RID OF ~bullshit.tmp

            Console.WriteLine(e.FullPath + " " + e.ChangeType + " " + DateTime.Now.ToString());

        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // update database 
        }

    }
}

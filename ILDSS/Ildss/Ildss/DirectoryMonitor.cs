using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Ildss
{
    class DirectoryMonitor
    {
        private FileSystemWatcher watcher;

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

            // Until thread is killed by program.cs?
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // work out what has been changed update database accordingyl
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    // hash it, add to database
                    break;
                case WatcherChangeTypes.Changed:
                    // update hash, update size.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


// TO DO
// Ensure new files/directories are added to index
// I.e. create/delete event triggers indexing? THINK ABOUT THIS


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
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching
            watcher.EnableRaisingEvents = true;

            // Until thread is killed by program.cs?
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // work out what has been changed update database accordingyl

            Console.WriteLine(e.FullPath + " " + e.ChangeType + " " + DateTime.Now.ToString());
            FileIndexContainer index = new FileIndexContainer();
            string fp = e.FullPath.ToString();
            File file = index.Files.Single(f => f.path == fp); 
            // NOT FINDING FILES IN DB NOW SO MAKE THAT BIT

            Event ev = new Event{date_time = DateTime.Now, FileFileId = file.FileId, type = e.ChangeType.ToString()};
            index.Events.Add(ev);
            index.SaveChanges();

        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // update database 
        }


    }
}

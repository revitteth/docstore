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
            Indexer indexer = new Indexer();

            // work out what has been changed update database accordingly
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    // fully index the file
                    indexer.IndexFile(e.FullPath);
                    return;
                    //break;
                case WatcherChangeTypes.Changed:
                    // Add event to existing file
                    WriteDatabase(e);
                    break;
                case WatcherChangeTypes.Deleted:
                    // remove Document entry. And events?
                    Console.WriteLine("deleted");
                    break;
            }



   

            //indexer.IndexFile(e.FullPath);

            // REGULAR EXPRESSION TO GET RID OF ~bullshit.tmp

            Console.WriteLine(e.FullPath + " " + e.ChangeType + " " + DateTime.Now.ToString());

        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // update database 
        }

        private static void WriteDatabase(FileSystemEventArgs e)
        {
            var t = DateTime.Now;
            var time = t.AddTicks(-(t.Ticks % TimeSpan.TicksPerSecond));


            using (FileIndexContainer fic = new FileIndexContainer())
            {
                var docPaths = from docpaths in fic.DocPaths
                               where docpaths.path == e.FullPath
                               select docpaths;

                var docPathDefault = docPaths.FirstOrDefault();

                var doc = from documents in fic.Documents
                          where documents.DocumentHash == docPathDefault.DocumentDocumentHash
                          select documents;

                var ev = from events in fic.DocEvents
                         where events.date_time.CompareTo(time) == 0
                         select events;

                var docDefault = doc.FirstOrDefault();

                // Event doesn't exist (events have unique times)
                if (ev.Count() == 0)
                {
                    DocEvent de = new DocEvent()
                    {
                        date_time = time,
                        type = e.ChangeType.ToString(),
                        DocumentDocumentHash = docDefault.DocumentHash
                    };
                    fic.DocEvents.Add(de);
                    fic.SaveChanges();
                }

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ildss.Models;

namespace Ildss
{
    class EventCollector : ICollector
    {
        public void Collect(string path)
        {
            // call an indexer to check all the read/write times of the files in the DB
            var indexer = KernelFactory.Instance.Get<IIndexer>("Frequent");
            indexer.IndexFiles(path);
        }

        public void Register(string path)
        {
            // if it is unique (TODO: build a comparer) add it to database
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var fi = new FileInfo(path);
            fi.Refresh();

            var docu = fic.DocPaths.First(i => i.path == path).Document;

            // Check read events
            if (fic.DocEvents.Any(i => i.type == "Read" && i.DocumentId == docu.DocumentId))
            {
                var recentRead = fic.DocEvents.OrderByDescending(i => i.time).First(j => j.type == "Read");
                if (DateTime.Compare(recentRead.time, fi.LastAccessTime) < 0)
                {
                    // add a new read event to the document if file's last access is bigger than last access in DB
                    var readEvent = new DocEvent() { time = fi.LastAccessTime, type = "Read" };
                    docu.DocEvents.Add(readEvent);
                    Console.WriteLine("More recent read added");
                }
            }
            else
            {
                // no existing reads - add the access time of the file
                var readEvent = new DocEvent() { time = fi.LastAccessTime, type = "Read" };
                docu.DocEvents.Add(readEvent);
                Console.WriteLine("No existing Reads, one added");
            }

            // Check write events
            if (fic.DocEvents.Any(i => i.type == "Write" && i.DocumentId == docu.DocumentId))
            {
                var recentWrite = fic.DocEvents.OrderByDescending(i => i.time).First(j => j.type == "Write");
                if (DateTime.Compare(recentWrite.time, fi.LastWriteTime) < 0)
                {
                    // add a new write event to the document
                    var writeEvent = new DocEvent() { time = fi.LastWriteTime, type = "Write" };
                    docu.DocEvents.Add(writeEvent);
                    Console.WriteLine("More recent write added");
                }
            }
            else
            {
                // no existing writes - add the write time of the file
                var writeEvent = new DocEvent() { time = fi.LastWriteTime, type = "Write" };
                docu.DocEvents.Add(writeEvent);
                Console.WriteLine("No existing Writes, one added");
            }

            fic.SaveChanges();

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ildss.Models;
using Ildss.Index;

namespace Ildss.Index
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
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var fi = new FileInfo(path);
            fi.Refresh();

            try
            {
                using (Stream stream = new FileStream(fi.FullName, FileMode.Open))
                {
                    // File/Stream manipulating code here
                }
            }
            catch
            {
                Logger.write("Error, file not accessible " + fi.Name);
                //check here why it failed and ask user to retry if the file is in use.
                return;
            }

            // Don't register events for directories
            if (!(File.GetAttributes(fi.FullName) == FileAttributes.Directory))
            {
                var docu = fic.DocPaths.First(i => i.Path == path).Document;

                // Check read events
                if (fic.DocEvents.Any(i => i.Type == Settings.EventType.Read && i.DocumentId == docu.DocumentId))
                {
                    var recentRead = fic.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Read);
                    if (DateTime.Compare(recentRead.Time, fi.LastAccessTime.AddMilliseconds(-fi.LastAccessTime.Millisecond)) < 0)
                    {
                        // add a new read event to the document if file's last access is bigger than last access in DB
                        var readEvent = new DocEvent() { Time = fi.LastAccessTime, Type = Settings.EventType.Read };
                        docu.DocEvents.Add(readEvent);
                        Logger.write("R Time New  " + path);
                    }
                }
                else
                {
                    // no existing reads - add the access time of the file
                    var readEvent = new DocEvent() { Time = fi.LastAccessTime, Type = Settings.EventType.Read };
                    docu.DocEvents.Add(readEvent);
                    Logger.write("R Time Init " + path);
                }

                // Check write events
                if (fic.DocEvents.Any(i => i.Type == Settings.EventType.Write && i.DocumentId == docu.DocumentId))
                {
                    var recentWrite = fic.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Write);
                    if (DateTime.Compare(recentWrite.Time, fi.LastWriteTime.AddMilliseconds(-fi.LastWriteTime.Millisecond)) < 0)
                    {
                        // add a new write event to the document
                        var writeEvent = new DocEvent() { Time = fi.LastWriteTime, Type = Settings.EventType.Write };
                        docu.DocEvents.Add(writeEvent);
                        Logger.write("W Time New  " + path);
                    }
                }
                else
                {
                    // no existing writes - add the write time of the file
                    var writeEvent = new DocEvent() { Time = fi.LastWriteTime, Type = Settings.EventType.Write };
                    docu.DocEvents.Add(writeEvent);
                    Logger.write("W Time Init " + path);
                }

                fic.SaveChanges();
            }

        }
    }
}

using Ildss.Crypto;
using Ildss.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ildss.Index
{
    class EventManager : IEventManager
    {
        private IList<FSEvent> _events = new List<FSEvent>();
        private Timer timer = new Timer(Settings.IndexInterval);

        public void AddEvent(FSEvent eve)
        {
            _events.Add(eve);
        }

        public void print()
        {
            foreach(var e in _events)
            {
                Logger.write(e.Type + " " + e.FileInf.Name);
            }
            _events.Clear();
        }


        // every half hour look for new events
        public EventManager()
        {
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler(IntervalIndex);
            GC.KeepAlive(timer);
        }

        public void IntervalIndex(object source, ElapsedEventArgs e)
        {
            try
            {
                var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                // add FSEvents for the following files:

                // all based on path now - renaming is handled by the FSW
                // go through directories comparing read/write times - if different add to events
                DirectoryTraverse(Settings.WorkingDir, fic);

                print();        
                // write the FSEvents to the database and flush the list


                WriteChangesToDB(fic);

                // if different
                // check to see if the file has more than one path
                // if not just update its size + hash
                // if it has, create a new document + remove path from old document and add to new one
                // if don't exist

            }
            catch (Exception ex)
            {
                Logger.write(ex.Message);
                // possibly dump all changes to DB?
            }
        }


        public void DirectoryTraverse(string dir, IFileIndexContext fic)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        // check here to see if read/write times different
                        if (fic.DocPaths.Any(i => i.Path == f))
                        {
                            // path matches
                            var doc = fic.DocPaths.First(i => i.Path == f).Document;
                            CheckReadWrite(f, doc, fic);
                        }
                        else
                        {
                            // new document found
                            // add the event with the file info - this should mean the file is created on evaluation of events
                            _events.Add(new FSEvent() { Type = Settings.EventType.Create, FileInf = new FileInfo(f) });
                            Logger.write("CREATE event registered " + f);
                        }
                    }
                    DirectoryTraverse(d, fic);
                }
            }
            catch (System.Exception e)
            {
                Logger.write(e.Message);
            }
        }

        public void CheckReadWrite(string path, Document doc, IFileIndexContext fic)
        {
            var fi = new FileInfo(path);
               
            // READ Events
            if (fic.DocEvents.Any(i => i.Type == Settings.EventType.Read && i.DocumentId == doc.DocumentId))
            {
                var recentRead = fic.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Read);
                if (DateTime.Compare(recentRead.Time, fi.LastAccessTime.AddMilliseconds(-fi.LastAccessTime.Millisecond)) < 0)
                {
                    // add the event to the list with the Type 'Read'
                    _events.Add(new FSEvent() { Type = Settings.EventType.Read, FileInf = fi });
                    Logger.write("READ event registered " + fi.Name);
                }
            }

            // WRITE Events
            if (fic.DocEvents.Any(i => i.Type == Settings.EventType.Read && i.DocumentId == doc.DocumentId))
            {
                var recentWrite = fic.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Write);
                if (DateTime.Compare(recentWrite.Time, fi.LastWriteTime.AddMilliseconds(-fi.LastWriteTime.Millisecond)) < 0)
                {
                    // add the event to the list with the Type 'Write'
                    _events.Add(new FSEvent() { Type = Settings.EventType.Write, FileInf = fi });
                    Logger.write("WRITE event registered " + fi.Name);
                }
            }


        }

        public void WriteChangesToDB(IFileIndexContext fic)
        {
            foreach (var e in _events)
            {
                var hash = KernelFactory.Instance.Get<IHash>().HashFile(e.FileInf.FullName);
                
                // CREATE
                if (e.Type == Settings.EventType.Create)
                {
                    // path doesn't exist - check against hash
                    if (fic.Documents.Any(i => i.DocumentHash == hash))
                    {
                        // if hash exists just add the path to the existing document
                        var doc = fic.Documents.First(i => i.DocumentHash == hash);
                        doc.DocPaths.Add(new DocPath() { Path = e.FileInf.FullName, Directory = e.FileInf.DirectoryName, Name = e.FileInf.Name });
                    }
                    else
                    {
                        // no path or hash found in DB
                        var doc = new Document() { DocumentHash = hash, Size = e.FileInf.Length, Status = Settings.DocStatus.Indexed };
                        doc.DocPaths.Add(new DocPath() { Path = e.FileInf.FullName, Directory = e.FileInf.DirectoryName, Name = e.FileInf.Name });
                    }
                    fic.SaveChanges();

                }
                // READ
                else if (e.Type == Settings.EventType.Read)
                {
                    // just add the read event
                    
                    fic.DocEvents.Add();
                }
                // WRITE
                else if (e.Type == Settings.EventType.Write)
                {
                    // add the write event plus update the file hash, possibly branch it out to a new document too + check it hasn't updated to = another document
                }
                else if (e.Type == Settings.EventType.Rename)
                {
                   // file has had path changed - may be a directory etc.
                }                     

            }

        }
    }
}

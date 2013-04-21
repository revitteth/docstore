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
        private IFileIndexContext fic = KernelFactory.Instance.Get<IFileIndexContext>();

        public void AddEvent(FSEvent eve)
        {
            _events.Add(eve);
        }

        public void print()
        {
            foreach(var e in _events)
            {
                Logger.write(e.Type + " " + e.FileInf.FullName);
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
                DirectoryTraverse(Settings.WorkingDir);


                WriteChangesToDB();

                // TODO
                // 1. delete documents with no paths
                // 2. rename logic

                // if different
                // check to see if the file has more than one path
                // if not just update its size + hash
                // if it has, create a new document + remove path from old document and add to new one
                // if don't exist

                print();

            }
            catch (Exception ex)
            {
                Logger.write(ex.Message);
                // possibly dump all changes to DB?
            }
        }

        private static IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Logger.write(ex.Message);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Logger.write(ex.Message);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }

        public void DirectoryTraverse(string dir)
        {
            try
            {
                foreach (string file in GetFiles(dir))
                {
                    if (!Settings.IgnoredExtensions.Any(file.Contains))
                    {
                        // check here to see if read/write times different
                        if (fic.DocPaths.Any(i => i.Path == file))
                        {
                            // path matches
                            var doc = fic.DocPaths.First(i => i.Path == file).Document;
                            CheckReadWrite(file, doc);
                        }
                        else
                        {
                            // new document found
                            // add the event with the file info - this should mean the file is created on evaluation of events
                            _events.Add(new FSEvent() { Type = Settings.EventType.Create, FileInf = new FileInfo(file) });
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Logger.write(e.Message);
            }
        }

        public void CheckReadWrite(string path, Document doc)
        {
            var fi = new FileInfo(path);
               
            // READ Events
            if (fic.DocEvents.Any(i => i.Type == Settings.EventType.Read && i.DocumentId == doc.DocumentId))
            {
                var recentRead = fic.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Read);
                if (DateTime.Compare(recentRead.Time, fi.LastAccessTime.AddMilliseconds(-fi.LastAccessTime.Millisecond)) < 0)
                {
                    // add the event to the list with the Type 'Read'
                    _events.Add(new FSEvent() { Type = Settings.EventType.Read, FileInf = fi, DocumentId = doc.DocumentId });
                }
            }

            // WRITE Events
            if (fic.DocEvents.Any(i => i.Type == Settings.EventType.Write && i.DocumentId == doc.DocumentId))
            {
                var recentWrite = fic.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Write);
                if (DateTime.Compare(recentWrite.Time, fi.LastWriteTime.AddMilliseconds(-fi.LastWriteTime.Millisecond)) < 0)
                {
                    // add the event to the list with the Type 'Write'
                    _events.Add(new FSEvent() { Type = Settings.EventType.Write, FileInf = fi, DocumentId = doc.DocumentId });
                }
            }


        }

        public void UpdateReadWrite(Document doc, FSEvent eve)
        {
            // READ Events
            if (fic.DocEvents.Any(i => i.Type == Settings.EventType.Read && i.DocumentId == doc.DocumentId))
            {
                var recentRead = fic.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Read);
                if (DateTime.Compare(recentRead.Time, eve.FileInf.LastAccessTime.AddMilliseconds(-eve.FileInf.LastAccessTime.Millisecond)) < 0)
                {
                    // add event to doc
                    doc.DocEvents.Add(new DocEvent() { Type = Settings.EventType.Read, Time = eve.FileInf.LastAccessTime });
                }
            }
            else
            {
                doc.DocEvents.Add(new DocEvent() { Type = Settings.EventType.Read, Time = eve.FileInf.LastAccessTime });
            }

            // WRITE Events
            if (fic.DocEvents.Any(i => i.Type == Settings.EventType.Write && i.DocumentId == doc.DocumentId))
            {
                var recentWrite = fic.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Write);
                if (DateTime.Compare(recentWrite.Time, eve.FileInf.LastWriteTime.AddMilliseconds(-eve.FileInf.LastWriteTime.Millisecond)) < 0)
                {
                    // add event to doc
                    doc.DocEvents.Add(new DocEvent() { Type = Settings.EventType.Write, Time = eve.FileInf.LastWriteTime });
                }
            }
            else
            {
                doc.DocEvents.Add(new DocEvent() { Type = Settings.EventType.Write, Time = eve.FileInf.LastWriteTime });
            }
        }

        public void WriteChangesToDB()
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
                        doc.DocEvents.Add(new DocEvent() { Type = e.Type, Time = DateTime.Now });
                        UpdateReadWrite(doc, e);
                    }
                    else
                    {
                        // no path or hash found in DB
                        var doc = new Document() { DocumentHash = hash, Size = e.FileInf.Length, Status = Settings.DocStatus.Indexed };
                        doc.DocPaths.Add(new DocPath() { Path = e.FileInf.FullName, Directory = e.FileInf.DirectoryName, Name = e.FileInf.Name });
                        doc.DocEvents.Add(new DocEvent() { Type = e.Type, Time = DateTime.Now });
                        UpdateReadWrite(doc, e);
                        fic.Documents.Add(doc);
                    }

                }
                // READ
                else if (e.Type == Settings.EventType.Read)
                {
                    // just add the read event
                    var doc = fic.Documents.First(i => i.DocumentHash == hash);
                    doc.DocEvents.Add(new DocEvent() { Time = e.FileInf.LastAccessTime, Type = e.Type });
                }
                // WRITE
                else if (e.Type == Settings.EventType.Write)
                {
                    // add the write event plus update the file hash, possibly branch it out to a new document too + check it hasn't updated to = another document
                    var doc = fic.Documents.First();
                    if (fic.Documents.Any(i => i.DocumentHash == hash))
                    {
                        // file hash matches an existing document (move the path to the matching document)
                        var matchingDoc = fic.Documents.First(i => i.DocumentHash == hash);
                        matchingDoc.DocPaths.Add(fic.DocPaths.First(i => i.Path == e.FileInf.FullName));
                        matchingDoc.DocEvents.Add(new DocEvent() { Time = e.FileInf.LastWriteTime, Type = e.Type });
                        Logger.write("Changed (new hash matches existing document) " + e.FileInf.Name);
                    }
                    else
                    {
                        if (fic.DocPaths.Any(i => i.Path == e.FileInf.FullName))
                        {
                            var currentPath = fic.DocPaths.First(i => i.Path == e.FileInf.FullName);
                            var relatedDoc = currentPath.Document;

                            if (relatedDoc.DocPaths.Count() == 1)
                            {
                                // update the doc
                                relatedDoc.DocumentHash = hash;
                                relatedDoc.Size = e.FileInf.Length;
                                relatedDoc.Status = Settings.DocStatus.Indexed;
                                relatedDoc.DocEvents.Add(new DocEvent() { Time = e.FileInf.LastWriteTime, Type = e.Type });
                                Logger.write("Changed (same document, updated hash, status indexed) " + e.FileInf.Name);
                            }
                            else if (relatedDoc.DocPaths.Count() > 1)
                            {
                                // create new doc and add the path to it
                                var newDoc = new Document() { DocumentHash = hash, Size = e.FileInf.Length, Status = Settings.DocStatus.Indexed };
                                newDoc.DocPaths.Add(currentPath);
                                newDoc.DocEvents.Add(new DocEvent() { Time = e.FileInf.LastWriteTime, Type = e.Type });
                                fic.Documents.Add(newDoc);
                                Logger.write("Changed (new hash, new document, status indexed");
                            }
                            else
                            {
                                // can't happen
                                Logger.write("Error, impossible logic");
                            }
                        }
                    }

                    fic.SaveChanges();
                }
                else if (e.Type == Settings.EventType.Rename)
                {
                   // file has had path changed - may be a directory etc.
                }

                // is it possible/more efficient to do this outside the for loop?
                fic.SaveChanges();
            }

        }

    }
}

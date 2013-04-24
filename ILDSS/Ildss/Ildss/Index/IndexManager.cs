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
    class IndexManager : IEventManager
    {
        private IList<FSEvent> _events = new List<FSEvent>();
        private IFileIndexContext fic = KernelFactory.Instance.Get<IFileIndexContext>();
        private Timer _indexTimer = new Timer(Settings.getIndexInterval());

        public bool IndexRequired { get; set; }

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

        public IndexManager()
        {
            Logger.write("Started Indexer with Interval " + Settings.getIndexInterval() / 1000 + " seconds");
            IntervalIndex(null, null);
            _indexTimer.Start();
            _indexTimer.Elapsed += new ElapsedEventHandler(IntervalIndex);
            GC.KeepAlive(_indexTimer);
        }

        public void IntervalIndex(object source, ElapsedEventArgs e)
        {
            _indexTimer.Enabled = false;
            //try
            //{
                if (IndexRequired == true)
                {
                    IndexRequired = false;
                    var fic = KernelFactory.Instance.Get<IFileIndexContext>();

                    // all based on path now - renaming is handled by the FSW
                    // go through directories comparing read/write times - if different add to events
                    DirectoryTraverse(Settings.getWorkingDir());


                    WriteChangesToDB();

                    // TODO
                    // 1. delete documents with no paths DONE. -> convert this to ARCHIVING
                    // 2. rename directory logic is a bit wonky - seems ok now just give it a few more rename trials on directories
                    // 3. Take events from one doc to another on update (i.e. don't lose the history!)
                    // 4. DONE. Work out where to update null hash (for office documents - IMPORTANT) & any open documents when indexing DONE.

                    MaintainDocuments();
                }

           // }
            //catch (Exception ex)
            //{
            //    Logger.write(ex.Message);
            //    // possibly dump all changes to DB?
            //}
            _indexTimer.Enabled = true;
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
            Logger.write("Indexing...");
            try
            {
                foreach (string file in GetFiles(dir))
                {
                    if (!Settings.getIgnoredExtensions().Any(file.Contains))
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
                // CREATE
                if (e.Type == Settings.EventType.Create)
                {
                    var hash = KernelFactory.Instance.Get<IHash>().HashFile(e.FileInf.FullName);
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
                    var doc = fic.DocPaths.First(j => j.Path == e.FileInf.FullName).Document;
                    doc.DocEvents.Add(new DocEvent() { Time = e.FileInf.LastAccessTime, Type = e.Type });
                }
                // WRITE
                else if (e.Type == Settings.EventType.Write)
                {
                    var hash = KernelFactory.Instance.Get<IHash>().HashFile(e.FileInf.FullName);
                    // add the write event plus update the file hash, possibly branch it out to a new document too + check it hasn't updated to = another document
                    if (fic.Documents.Any(i => i.DocumentHash == hash))
                    {
                        // file hash matches an existing document (move the path to the matching document)
                        var matchingDoc = fic.Documents.First(i => i.DocumentHash == hash);
                        matchingDoc.DocPaths.Add(fic.DocPaths.First(i => i.Path == e.FileInf.FullName));
                        matchingDoc.DocEvents.Add(new DocEvent() { Time = e.FileInf.LastWriteTime, Type = e.Type });
                        //Logger.write("Changed (new hash matches existing document) " + e.FileInf.Name);
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
                                //Logger.write("Changed (same document, updated hash, status indexed) " + e.FileInf.Name);
                            }
                            else if (relatedDoc.DocPaths.Count() > 1)
                            {
                                // create new doc and add the path to it
                                var newDoc = new Document() { DocumentHash = hash, Size = e.FileInf.Length, Status = Settings.DocStatus.Indexed };
                                newDoc.DocPaths.Add(currentPath);
                                newDoc.DocEvents.Add(new DocEvent() { Time = e.FileInf.LastWriteTime, Type = e.Type });
                                fic.Documents.Add(newDoc);
                                //Logger.write("Changed (new hash, new document, status indexed");
                            }
                            else
                            {
                                // can't happen
                                Logger.write("Error, impossible logic");
                            }
                        }
                    }

                }
                else if (e.Type == Settings.EventType.Rename)
                {
                    if (e.isDirectory)
                    {
                        Logger.write("Rename Directory " + e.OldPath + " to " + e.FileInf.FullName);
                        foreach (var directory in fic.DocPaths.Where(i => i.Directory.Contains(e.OldPath)))
                        {
                            directory.Directory = directory.Directory.Replace(e.OldPath, e.FileInf.FullName); // subdirectories
                        }
                        foreach (var file in fic.DocPaths.Where(i => i.Path.Contains(e.OldPath)))
                        {
                            file.Path = file.Path.Replace(e.OldPath, e.FileInf.FullName);   // file paths
                        }
                    }
                    else
                    {
                        var renamed = fic.DocPaths.First(i => i.Path == e.OldPath);
                        renamed.Path = e.FileInf.FullName;
                        renamed.Directory = e.FileInf.FullName.Replace(e.FileInf.Name, "");
                        renamed.Name = e.FileInf.Name;
                    }
                }
                //fic.SaveChanges();
            }
            fic.SaveChanges();
            _events.Clear();
            Logger.write("Done");
        }

        public void MaintainDocuments()
        {
            List<Document> docsToRemove = new List<Document>();
            List<DocPath> pathsToRemove = new List<DocPath>();

            foreach (var docu in fic.Documents.Distinct())
            {
                if (!docu.DocPaths.Any())
                {
                    docsToRemove.Add(docu);
                }
                else
                {
                    foreach (var path in docu.DocPaths)
                    {
                        if (!File.Exists(path.Path))
                        {
                            pathsToRemove.Add(path);
                            //Logger.write("Deleting Path " + path.Name);
                        }
                    }
                }
                if (docu.DocumentHash == null & docu.DocPaths.Any())
                {
                    Logger.write("NULL Hash - repairing");
                    docu.DocumentHash = KernelFactory.Instance.Get<IHash>().HashFile(docu.DocPaths.FirstOrDefault().Path);
                }
            }

            // this is dangerous atm
            foreach (var docToRemove in docsToRemove)
            {
                fic.Documents.Remove(docToRemove);                
                //Logger.write("Deleted (had no paths) " + docToRemove.DocumentId + " " + docToRemove.DocPaths.FirstOrDefault().Name);
            }

            foreach (var pathToRemove in pathsToRemove)
            {
                fic.DocPaths.Remove(pathToRemove);
            }

            fic.SaveChanges();
            docsToRemove.Clear();
        }

    }
}

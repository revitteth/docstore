using Ildss.Crypto;
using Ildss.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Log;

namespace Ildss.Index
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class IndexManager : IEventManager
    {
        private IList<FileSystemEvent> _events = new List<FileSystemEvent>();
        private IFileIndexContext _fic;
        private Timer _indexTimer = new Timer();
        public bool IndexRequired { get; set; }

        public IndexManager()
        {
            _fic = KernelFactory.Instance.Get<IFileIndexContext>();
            IndexRequired = true;
            Logger.Write("Started Indexer with Interval " + Settings.Default.IndexInterval.ToString("c"));
            IntervalIndex(null, null);
            _indexTimer.Interval = Settings.Default.IndexInterval.TotalMilliseconds;
            _indexTimer.Start();
            _indexTimer.Elapsed += new ElapsedEventHandler(IntervalIndex);
            GC.KeepAlive(_indexTimer);
        }

        public void AddEvent(FileSystemEvent eve)
        {
            _events.Add(eve);
        }

        public void IntervalIndex(object source, ElapsedEventArgs e)
        {
            _indexTimer.Interval = Settings.Default.IndexInterval.TotalMilliseconds;
            _indexTimer.Enabled = false;
            try
            {
                if (IndexRequired == true)
                {
                    IndexRequired = false;
                    Logger.Write("Indexing...");

                    // all based on path now - renaming is handled by the FSW
                    // go through directories comparing read/write times - if different add to events
                    DirectoryTraverse(Settings.Default.WorkingDir);


                    WriteChangesToDB();

                    MaintainDocuments();

                    Logger.Write("Finished Indexing");

                    // TODO
                    // 1. delete documents with no paths DONE. -> convert this to ARCHIVING
                    // 2. rename directory logic is a bit wonky - seems ok now just give it a few more rename trials on directories
                    // 3. Take events from one doc to another on update (i.e. don't lose the history!)
                    // 4. DONE. Work out where to update null hash (for office documents - IMPORTANT) & any open documents when indexing DONE.
                                      
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                // possibly dump all changes to DB?
            }
            _indexTimer.Enabled = true;
        }

        private IEnumerable<string> GetFiles(string path)
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
                    Logger.Write(ex.Message);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message);
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
                    if (!Settings.Default.IgnoredExtensions.Cast<string>().ToList().Any(file.Contains))
                    {
                        try
                        {
                            // path matches
                            var doc = _fic.DocPaths.First(i => i.Path == file).Document;
                            CheckFileTimes(file, doc);
                        }
                        catch (Exception ex)
                        {
                            //Logger.write("ERROR: : : " + ex.Message);
                            // new document found
                            // add the event with the file info - this should mean the file is created on evaluation of events
                            var fi = new FileInfo(file);
                            fi.Refresh();

                            _events.Add(new FileSystemEvent() { 
                                Type = Enums.EventType.Create, 
                                FileInf = fi, 
                                LastWrite = fi.LastWriteTime, 
                                LastAccess = fi.LastAccessTime,
                                CreationTime = fi.CreationTime
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
        }

        public void CheckFileTimes(string path, Document doc)
        {
            var fi = new FileInfo(path);
            fi.Refresh();

            // READ Events
            if (doc.DocEvents.Any(i => i.Type == Enums.EventType.Read))
            {
                var recentRead = doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Enums.EventType.Read);
                if (DateTime.Compare(recentRead.Time, fi.LastAccessTime.AddMilliseconds(-fi.LastAccessTime.Millisecond)) < 0)
                {
                    // add the event to the list with the Type 'Read'
                    _events.Add(new FileSystemEvent() { 
                        Type = Enums.EventType.Read, 
                        FileInf = fi, 
                        DocumentId = doc.DocumentId,
                        LastWrite = fi.LastWriteTime,
                        LastAccess = fi.LastAccessTime,
                        CreationTime = fi.CreationTime
                    });
                }
            }

            // WRITE Events
            if (doc.DocEvents.Any(i => i.Type == Enums.EventType.Write))
            {
                var recentWrite = doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Enums.EventType.Write);
                if (DateTime.Compare(recentWrite.Time, fi.LastWriteTime.AddMilliseconds(-fi.LastWriteTime.Millisecond)) < 0)
                {
                    // add the event to the list with the Type 'Write'
                    _events.Add(new FileSystemEvent() { 
                        Type = Enums.EventType.Write, 
                        FileInf = fi, 
                        DocumentId = doc.DocumentId,
                        LastWrite = fi.LastWriteTime,
                        LastAccess = fi.LastAccessTime,
                        CreationTime = fi.CreationTime
                    });
                }
            }
        }

        public void RestoreFileTimes(FileSystemEvent eve)
        {
            try
            {
                eve.FileInf.LastAccessTime = eve.LastAccess;
                eve.FileInf.LastWriteTime = eve.LastWrite;
                eve.FileInf.CreationTime = eve.CreationTime;
            }
            catch (UnauthorizedAccessException ex)
            {
                // this occurs when the time cannot be written (mostly files in .git?)
            }
        }

        public void UpdateFileTimes(Document doc, FileSystemEvent eve)
        {
            // READ Events
            if (doc.DocEvents.Any(i => i.Type == Enums.EventType.Read))
            {
                var recentRead = doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Enums.EventType.Read);
                if (DateTime.Compare(recentRead.Time, eve.FileInf.LastAccessTime.AddMilliseconds(-eve.FileInf.LastAccessTime.Millisecond)) < 0)
                {
                    // add event to doc
                    doc.DocEvents.Add(new DocEvent() { Type = Enums.EventType.Read, Time = eve.FileInf.LastAccessTime });
                }
            }
            else
            {
                doc.DocEvents.Add(new DocEvent() { Type = Enums.EventType.Read, Time = eve.FileInf.LastAccessTime });
            }

            // WRITE Events
            if (doc.DocEvents.Any(i => i.Type == Enums.EventType.Write))
            {
                var recentWrite = doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Enums.EventType.Write);
                if (DateTime.Compare(recentWrite.Time, eve.FileInf.LastWriteTime.AddMilliseconds(-eve.FileInf.LastWriteTime.Millisecond)) < 0)
                {
                    // add event to doc
                    doc.DocEvents.Add(new DocEvent() { Type = Enums.EventType.Write, Time = eve.FileInf.LastWriteTime });
                }
            }
            else
            {
                doc.DocEvents.Add(new DocEvent() { Type = Enums.EventType.Write, Time = eve.FileInf.LastWriteTime });
            }
        }

        public void WriteChangesToDB()
        {
            foreach (var e in _events)
            {
                // CREATE
                if (e.Type == Enums.EventType.Create)
                {
                    var hash = KernelFactory.Instance.Get<IHash>().HashFile(e.FileInf.FullName);
                    // path doesn't exist - check against hash
                    if (_fic.Documents.Any(i => i.DocumentHash == hash))
                    {
                        // if hash exists just add the path to the existing document
                        var doc = _fic.Documents.First(i => i.DocumentHash == hash);
                        
                        if (doc.Status == Enums.DocStatus.Archived)
                        {
                            doc.Status = Enums.DocStatus.Current;
                        }

                        doc.DocPaths.Add(new DocPath() { Path = e.FileInf.FullName, Directory = e.FileInf.DirectoryName, Name = e.FileInf.Name });
                        doc.DocEvents.Add(new DocEvent() { Type = e.Type, Time = e.CreationTime });
                        UpdateFileTimes(doc, e);
                    }
                    else
                    {
                        // no path or hash found in DB
                        var doc = new Document() { DocumentHash = hash, Size = e.FileInf.Length, Status = Enums.DocStatus.Indexed };
                        doc.DocPaths.Add(new DocPath() { Path = e.FileInf.FullName, Directory = e.FileInf.DirectoryName, Name = e.FileInf.Name });
                        doc.DocEvents.Add(new DocEvent() { Type = e.Type, Time = e.CreationTime });
                        UpdateFileTimes(doc, e);
                        _fic.Documents.Add(doc);
                    }
                }
                // READ
                else if (e.Type == Enums.EventType.Read)
                {
                    // just add the read event
                    var doc = _fic.DocPaths.First(j => j.Path == e.FileInf.FullName).Document;
                    doc.DocEvents.Add(new DocEvent() { Time = e.LastAccess, Type = e.Type });
                }
                // WRITE
                else if (e.Type == Enums.EventType.Write)
                {
                    var hash = KernelFactory.Instance.Get<IHash>().HashFile(e.FileInf.FullName);
                    // add the write event plus update the file hash, possibly branch it out to a new document too + check it hasn't updated to = another document
                    if (_fic.Documents.Any(i => i.DocumentHash == hash))
                    {
                        // file hash matches an existing document (move the path to the matching document)
                        var matchingDoc = _fic.Documents.First(i => i.DocumentHash == hash);

                        if (matchingDoc.Status == Enums.DocStatus.Archived)
                        {
                            matchingDoc.Status = Enums.DocStatus.Current;
                        }

                        matchingDoc.DocPaths.Add(_fic.DocPaths.First(i => i.Path == e.FileInf.FullName));
                        matchingDoc.DocEvents.Add(new DocEvent() { Time = e.LastWrite, Type = e.Type });
                        //Logger.write("Changed (new hash matches existing document) " + e.FileInf.Name);
                    }
                    else
                    {
                        if (_fic.DocPaths.Any(i => i.Path == e.FileInf.FullName))
                        {
                            var currentPath = _fic.DocPaths.First(i => i.Path == e.FileInf.FullName);
                            var relatedDoc = currentPath.Document;

                            if (relatedDoc.DocPaths.Count() == 1)
                            {
                                // update the doc
                                relatedDoc.DocumentHash = hash;
                                relatedDoc.Size = e.FileInf.Length;
                                relatedDoc.Status = Enums.DocStatus.Indexed;
                                relatedDoc.DocEvents.Add(new DocEvent() { Time = e.LastWrite, Type = e.Type });
                                //Logger.write("Changed (same document, updated hash, status indexed) " + e.FileInf.Name);
                            }
                            else if (relatedDoc.DocPaths.Count() > 1)
                            {
                                // create new doc and add the path to it
                                var newDoc = new Document() { DocumentHash = hash, Size = e.FileInf.Length, Status = Enums.DocStatus.Indexed };
                                newDoc.DocPaths.Add(currentPath);
                                newDoc.DocEvents.Add(new DocEvent() { Time = e.LastWrite, Type = e.Type });
                                _fic.Documents.Add(newDoc);
                                //Logger.write("Changed (new hash, new document, status indexed");
                            }
                            else
                            {
                                // can't happen
                                Logger.Write("Error, impossible logic");
                            }
                        }
                    }
                }

                RestoreFileTimes(e);
                _fic.SaveChanges();
            }
            
            _events.Clear();
        }

        public void MaintainDocuments()
        {
            // don't remove any paths - these are required for restoring documents

            // Remove pathless documents (or update hashes if null hash but paths)
            foreach (var docu in _fic.Documents)
            {
                if (docu.DocumentHash == null)
                {
                    Logger.Write("NULL Hash - repairing");
                    docu.DocumentHash = KernelFactory.Instance.Get<IHash>().HashFile(docu.DocPaths.FirstOrDefault().Path);
                }
            }

            _fic.SaveChanges();
        }

    }
}

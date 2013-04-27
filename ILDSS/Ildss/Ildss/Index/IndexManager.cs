﻿using Ildss.Crypto;
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
        private IList<FSEvent> _events = new List<FSEvent>();
        private IFileIndexContext fic = KernelFactory.Instance.Get<IFileIndexContext>();
        private Timer _indexTimer = new Timer();
        public bool IndexRequired { get; set; }

        public void AddEvent(FSEvent eve)
        {
            _events.Add(eve);
        }

        public IndexManager()
        {
            IndexRequired = true;
            Logger.Write("Started Indexer with Interval " + Settings.getIndexInterval() / 1000 + " seconds");
            IntervalIndex(null, null);
            _indexTimer.Interval = Settings.getIndexInterval();
            _indexTimer.Start();
            _indexTimer.Elapsed += new ElapsedEventHandler(IntervalIndex);
            GC.KeepAlive(_indexTimer);
        }

        public void IntervalIndex(object source, ElapsedEventArgs e)
        {
            _indexTimer.Interval = Settings.getIndexInterval();
            _indexTimer.Enabled = false;
            try
            {
                if (IndexRequired == true)
                {
                    IndexRequired = false;
                    Logger.Write("Indexing...");
                    var fic = KernelFactory.Instance.Get<IFileIndexContext>();

                    // all based on path now - renaming is handled by the FSW
                    // go through directories comparing read/write times - if different add to events
                    DirectoryTraverse(Settings.getWorkingDir());


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
                    if (!Settings.getIgnoredExtensions().Any(file.Contains))
                    {
                        try
                        {
                            // path matches
                            var doc = fic.DocPaths.First(i => i.Path == file).Document;
                            CheckFileTimes(file, doc);
                        }
                        catch (Exception ex)
                        {
                            //Logger.write("ERROR: : : " + ex.Message);
                            // new document found
                            // add the event with the file info - this should mean the file is created on evaluation of events
                            var fi = new FileInfo(file);
                            fi.Refresh();

                            _events.Add(new FSEvent() { 
                                Type = Settings.EventType.Create, 
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
            if (doc.DocEvents.Any(i => i.Type == Settings.EventType.Read))
            {
                var recentRead = doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Read);
                if (DateTime.Compare(recentRead.Time, fi.LastAccessTime.AddMilliseconds(-fi.LastAccessTime.Millisecond)) < 0)
                {
                    // add the event to the list with the Type 'Read'
                    _events.Add(new FSEvent() { 
                        Type = Settings.EventType.Read, 
                        FileInf = fi, 
                        DocumentId = doc.DocumentId,
                        LastWrite = fi.LastWriteTime,
                        LastAccess = fi.LastAccessTime,
                        CreationTime = fi.CreationTime
                    });
                }
            }

            // WRITE Events
            if (doc.DocEvents.Any(i => i.Type == Settings.EventType.Write))
            {
                var recentWrite = doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Write);
                if (DateTime.Compare(recentWrite.Time, fi.LastWriteTime.AddMilliseconds(-fi.LastWriteTime.Millisecond)) < 0)
                {
                    // add the event to the list with the Type 'Write'
                    _events.Add(new FSEvent() { 
                        Type = Settings.EventType.Write, 
                        FileInf = fi, 
                        DocumentId = doc.DocumentId,
                        LastWrite = fi.LastWriteTime,
                        LastAccess = fi.LastAccessTime,
                        CreationTime = fi.CreationTime
                    });
                }
            }
        }

        public void RestoreFileTimes(FSEvent eve)
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

        public void UpdateFileTimes(Document doc, FSEvent eve)
        {
            // READ Events
            if (doc.DocEvents.Any(i => i.Type == Settings.EventType.Read))
            {
                var recentRead = doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Read);
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
            if (doc.DocEvents.Any(i => i.Type == Settings.EventType.Write))
            {
                var recentWrite = doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Write);
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
                        doc.DocEvents.Add(new DocEvent() { Type = e.Type, Time = e.CreationTime });
                        UpdateFileTimes(doc, e);
                    }
                    else
                    {
                        // no path or hash found in DB
                        var doc = new Document() { DocumentHash = hash, Size = e.FileInf.Length, Status = Settings.DocStatus.Indexed };
                        doc.DocPaths.Add(new DocPath() { Path = e.FileInf.FullName, Directory = e.FileInf.DirectoryName, Name = e.FileInf.Name });
                        doc.DocEvents.Add(new DocEvent() { Type = e.Type, Time = e.CreationTime });
                        UpdateFileTimes(doc, e);
                        fic.Documents.Add(doc);
                    }
                }
                // READ
                else if (e.Type == Settings.EventType.Read)
                {
                    // just add the read event
                    var doc = fic.DocPaths.First(j => j.Path == e.FileInf.FullName).Document;
                    doc.DocEvents.Add(new DocEvent() { Time = e.LastAccess, Type = e.Type });
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
                        matchingDoc.DocEvents.Add(new DocEvent() { Time = e.LastWrite, Type = e.Type });
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
                                relatedDoc.DocEvents.Add(new DocEvent() { Time = e.LastWrite, Type = e.Type });
                                //Logger.write("Changed (same document, updated hash, status indexed) " + e.FileInf.Name);
                            }
                            else if (relatedDoc.DocPaths.Count() > 1)
                            {
                                // create new doc and add the path to it
                                var newDoc = new Document() { DocumentHash = hash, Size = e.FileInf.Length, Status = Settings.DocStatus.Indexed };
                                newDoc.DocPaths.Add(currentPath);
                                newDoc.DocEvents.Add(new DocEvent() { Time = e.LastWrite, Type = e.Type });
                                fic.Documents.Add(newDoc);
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
                // does reading the fic also read the items in memory?
                //fic.SaveChanges();
            }
            fic.SaveChanges();
            _events.Clear();
        }

        public void MaintainDocuments()
        {
            List<Document> docsToRemove = new List<Document>();
            List<DocPath> pathsToRemove = new List<DocPath>();


            // Remove non-existant paths
            foreach (var path in fic.DocPaths)
            {
                if (!File.Exists(path.Path))
                {
                    pathsToRemove.Add(path);
                    //Logger.write("Deleting Path " + path.Name);
                }
            }

            foreach (var pathToRemove in pathsToRemove)
            {
                fic.DocPaths.Remove(pathToRemove);
            }

            fic.SaveChanges();
            pathsToRemove.Clear();


            // Remove pathless documents (or update hashes if null hash but paths)
            foreach (var docu in fic.Documents)
            {
                if (!docu.DocPaths.Any())
                {
                    docsToRemove.Add(docu);
                }
                else if (docu.DocumentHash == null)
                {
                    Logger.Write("NULL Hash - repairing");
                    docu.DocumentHash = KernelFactory.Instance.Get<IHash>().HashFile(docu.DocPaths.FirstOrDefault().Path);
                }
            }

            // this is dangerous atm
            foreach (var docToRemove in docsToRemove)
            {
                fic.Documents.Remove(docToRemove);                
                //Logger.write("Deleted (had no paths) " + docToRemove.DocumentId + " " + docToRemove.DocPaths.FirstOrDefault().Name);
            }

            fic.SaveChanges();
            docsToRemove.Clear();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Security.Permissions;

namespace Ildss
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class Indexer : IIndexer
    {
        private FileInfo fi { get; set; }
        private List<Document> nullDocs, nullDocsSecondary = new List<Document>();
        private List<DocPath> nullDocPaths = new List<DocPath>();
        private DateTime accessTime, writeTime;
        
        public void IndexFiles(string path)
        {
            if (System.IO.File.Exists(path))
            {
                IndexFile(path);
            }
            else if (System.IO.Directory.Exists(path))
            {
                IndexDirectory(path);
            }
            //CheckIndex();
        }

        // Recursively index subdirectories
        private void IndexDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = System.IO.Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                IndexFile(fileName);
            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                IndexDirectory(subdirectory);
        }

        public void IndexFile(string path)
        {
            fi = new FileInfo(path);
            fi.Refresh();

            // Preserve access and write times
            accessTime = fi.LastAccessTime;
            writeTime = fi.LastWriteTime;

            // Hash File
            var h = KernelFactory.Instance.Get<IHash>();
            string fileHash = h.HashFile(path);

            var fic = KernelFactory.Instance.Get<FileIndexContext>();

            var document = new Document();
            var docpath = new DocPath();

            if (fic.Documents.Any(i => i.DocumentHash == fileHash))
            {
                // File hashes match (same file)
                if (fic.DocPaths.Any(i => i.path == fi.FullName))
                {
                    // same document, same directory, same name
                }
                    /*
                else if (fic.DocPaths.Any(i => i.name == fi.Name))
                {
                    // same document, different directory, same name
                    docpath.path = fi.FullName;
                    docpath.directory = fi.DirectoryName;
                    docpath.name = fi.Name;
                    docpath.Document = fic.Documents.First(i => i.DocumentHash == fileHash);
                    fic.DocPaths.Add(docpath);
                }
                else if (fic.DocPaths.Any(i => i.Document.DocumentHash == fileHash && i.directory == fi.DirectoryName))
                {
                    // same document, same directory, different name
                    docpath = fic.DocPaths.First(i => i.Document.DocumentHash == fileHash && i.directory == fi.DirectoryName);
                    docpath.path = fi.FullName;
                    docpath.directory = fi.DirectoryName;
                    docpath.name = fi.Name;
                }*/
                else
                {
                    document = fic.Documents.FirstOrDefault(i => i.DocumentHash == fileHash);
                    // same document, different directory, different name
                    if (fic.DocPaths.Any(i => i.path == path))
                    {
                        //if path exists update it
                        docpath = fic.DocPaths.FirstOrDefault(i => i.path == path);
                    }
                    else
                    {
                        docpath.path = fi.FullName;
                        docpath.directory = fi.DirectoryName;
                        docpath.name = fi.Name;
                    }
                    document.DocPaths.Add(docpath);
                }

            }
            else
            {
                // Different file (file has been edited)
                if (fic.DocPaths.Any(i => i.path == fi.FullName))
                {
                    Console.WriteLine("hello ====== " + fic.DocPaths.Count(i => i.name == fi.Name));
                    if (fic.DocPaths.Count(i => i.name == fi.Name) == 1)
                    {
                        // If document only has one path, update the document
                        Console.WriteLine("Updating Hash and size of old document");
                        document = fic.DocPaths.First(i => i.path == fi.FullName).Document;
                        document.size = fi.Length;
                        document.DocumentHash = fileHash;
                    }
                    else
                    {
                        Console.WriteLine("Writing Hash and size into new document");
                        // matching paths - one of many identical files has been edited, update the path and add to new document
                        document = new Document() { DocumentHash = fileHash, size = fi.Length };
                        docpath = fic.DocPaths.First(i => i.path == fi.FullName);
                        docpath.Document = document;
                        document.DocPaths.Add(docpath);
                        fic.Documents.Add(document);
                    }
                }
                else
                {
                    Console.WriteLine("New file");
                    // completely new file & path
                    document.DocumentHash = fileHash;
                    document.size = fi.Length;
                    docpath.Document = document;
                    docpath.name = fi.Name;
                    docpath.directory = fi.DirectoryName;
                    docpath.path = fi.FullName;
                    fic.Documents.Add(document);
                    fic.DocPaths.Add(docpath);
                }
            }

            if(EventOccurred(path, accessTime, writeTime))
            {
                var docevent = new DocEvent() { //Document = fic.Documents.FirstOrDefault(i => i.DocPaths.Any(j => j.path == path)),
                                                last_write = writeTime,
                                                last_access = accessTime,
                                                type = "index"
                };
                document.DocEvents.Add(docevent);
            }

            fic.SaveChanges();

            File.SetLastAccessTime(path, accessTime);
            File.SetLastWriteTime(path, writeTime);
        }


        private bool EventOccurred(string filePath, DateTime access, DateTime write)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

            foreach (DocEvent d in fic.DocEvents)
            {
                TimeSpan diffWrite = write.Subtract(d.last_write);
                TimeSpan diffAccess = access.Subtract(d.last_write);

                if (diffWrite.Duration() < TimeSpan.FromSeconds(100) && diffAccess.Duration() < TimeSpan.FromSeconds(100))
                {
                    return false;
                }
            }

            return true;
        }

    }
}

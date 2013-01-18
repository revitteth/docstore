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
    class Indexer : IIndexer
    {
        private FileInfo fi { get; set; }
        private List<Document> nullDocs = new List<Document>();
        private List<DocPath> nullDocPaths = new List<DocPath>();
        
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
           // CheckIndex(path);
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

            // Hash File
            var h = KernelFactory.Instance.Get<IHash>();
            string fileHash = h.HashFile(path);

            var fic = KernelFactory.Instance.Get<FileIndexContext>();

            var nameMatch = fic.DocPaths.Any(i => i.name == fi.Name);
            var document = new Document();
            var docpath = new DocPath();

            if (fic.Documents.Any(i => i.DocumentHash == fileHash))
            {
                // File hashes match (same file)
                if (path == fic.Documents.First(i => i.DocumentHash == fileHash).DocPaths.First(i => i.path == path).path)
                {
                    // paths match - do nothing it all exists
                }
                else
                {
                    // same document, different path - add path to document
                    docpath.path = fi.FullName;
                    docpath.name = fi.Name;
                    docpath.Document = fic.Documents.First(i => i.DocumentHash == fileHash);
                    document.DocPaths.Add(docpath);
                }
            }
            else
            {
                // Different file 
                if (fic.DocPaths.Any(i => i.path == fi.FullName))
                {
                    // matching paths - file has been updated
                    document = fic.DocPaths.First(i => i.path == fi.FullName).Document;
                    document.size = fi.Length;
                    document.DocumentHash = fileHash;
                }
                else
                {
                    // completely new file & path
                    document.DocumentHash = fileHash;
                    document.size = fi.Length;
                    docpath.Document = document;
                    docpath.name = fi.Name;
                    docpath.path = fi.FullName;
                    fic.Documents.Add(document);
                    fic.DocPaths.Add(docpath);
                }
            }

            fic.SaveChanges();
            Console.WriteLine("they should be in now");
        }

        public void CheckIndex()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var documents = fic.Documents;
            var paths = fic.DocPaths;

            if (documents.Any() && paths.Any())
            {
                foreach (DocPath p in paths)
                {
                    if (File.Exists(p.path))
                    {
                        // do nothing
                    }
                    else if (documents.Any(i => i.DocumentHash == p.Document.DocumentHash))
                    {
                        // hash matches one in documents
                        p.Document = documents.First((i => i.DocumentHash == p.Document.DocumentHash));
                    }
                    else
                    {
                        nullDocPaths.Add(p);
                    }
                }

                foreach (Document d in documents)
                {
                    if (!(documents.Any()))
                    {
                        nullDocs.Add(d);
                    }
                }


                foreach (Document d in nullDocs)
                {
                    documents.Remove(d);
                }

                foreach (DocPath p in nullDocPaths)
                {
                    paths.Remove(p);
                }
                fic.SaveChanges();
            }
        }
    }
}

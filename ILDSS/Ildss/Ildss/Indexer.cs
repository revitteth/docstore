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
           CheckIndex();
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
                // PROBLEM - wen no files in DB this errors
                if (fic.DocPaths.Any(i => i.name == fi.Name))
                {
                    Console.WriteLine("blap");
                    if (fic.DocPaths.Any(i => i.path == fi.FullName))
                    {
                        // paths match - do nothing it all exists
                    }
                    else
                    {
                        Console.WriteLine("the one i'm looking for");
                        // same document, different path - add path to document
                        docpath.path = fi.FullName;
                        docpath.name = fi.Name;
                        docpath.Document = fic.Documents.First(i => i.DocumentHash == fileHash);
                        fic.DocPaths.Add(docpath);
                    }
                }
                else
                {
                    // file names are different
                    Console.WriteLine("the one i'm looking for");
                    // same document, different path - add path to document
                    docpath.path = fi.FullName;
                    docpath.name = fi.Name;
                    docpath.Document = fic.Documents.First(i => i.DocumentHash == fileHash);
                }
            }
            else
            {
                // Different file 
                if (fic.DocPaths.Any(i => i.name == fi.Name))
                {
                    // if it is a unique bad boy then just update the document
                    // if it is one of many then make a new document!!!! 
                    if (fic.DocPaths.Count(i => i.name == fi.Name) == 1)
                    {
                        // Unique
                        Console.WriteLine("Updating Hash and size of old document");
                        // matching paths - file has been updated, update document
                        document = fic.DocPaths.First(i => i.name == fi.Name).Document;
                        document.size = fi.Length;
                        document.DocumentHash = fileHash;
                    }
                    else
                    {
                        Console.WriteLine("Writing Hash and size into new document");
                        // matching paths - file has been updated, update document
                        document = new Document() { DocumentHash = fileHash, size = fi.Length };
                        docpath = fic.DocPaths.First(i => i.name == fi.Name);
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
                    docpath.path = fi.FullName;
                    fic.Documents.Add(document);
                    fic.DocPaths.Add(docpath);
                }
            }

            fic.SaveChanges();
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

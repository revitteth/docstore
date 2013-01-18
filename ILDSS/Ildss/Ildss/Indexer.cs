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
            CheckIndex(path);
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

            Console.WriteLine("WTF?");

            var documents = fic.Documents.Where(i => i.DocumentHash == fileHash);
            var document = new Document();
            var docpath = new DocPath();

            Console.WriteLine("hello?!");

            if (documents.Any())
            {
                Console.WriteLine("there are docs");
                // Check hash against all existing documents including ones from checkindex (documents with no paths)
                //if match
                if (documents.Any(i => i.DocPaths.Any(j => j.path == fi.FullName)))
                {
                    // path exists, do nothing.
                }
                else if (documents.Any(i => i.DocPaths.Any(j => j.name == fi.Name)))
                {
                    // file name with same hash exists (different path) - get it and add it to paths
                    docpath.Document = documents.First(i => i.DocPaths.Any(j => j.name == fi.Name));
                    docpath.name = fi.Name;
                    docpath.path = fi.FullName;
                    fic.DocPaths.Add(docpath);
                }
                else
                {
                    // no matching path, only matching hash - create new path
                    docpath.Document = documents.First(i => i.DocPaths.Any(j => j.name == fi.Name));
                    docpath.name = fi.Name;
                    docpath.path = fi.FullName;
                    fic.DocPaths.Add(docpath);
                }
            }
            else
            {
                // new document, new path
                document.DocumentHash = fileHash;
                document.size = fi.Length;
                fic.Documents.Add(document);
                docpath.path = fi.FullName;
                docpath.name = fi.Name;
                docpath.Document = document;
                fic.DocPaths.Add(docpath);
                Console.WriteLine("main else");
            }

            fic.SaveChanges();
            Console.WriteLine("they should be in now");
           
        }

        public void CheckIndex(string path)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var documents = fic.Documents;
            var paths = fic.DocPaths;


            foreach (Document d in documents)
            {
                foreach (DocPath p in d.DocPaths)
                {
                    if (!File.Exists(p.path))
                    {
                        //paths.Remove(p);
                        nullDocPaths.Add(p);
                        Console.WriteLine("removing path " + p.path);
                    }
                }
                if (!(d.DocPaths.Any(i => i.DocumentId == d.DocumentId)))
                {
                    //documents.Remove(d);
                    nullDocs.Add(d);
                    Console.WriteLine("removing " + d.DocumentId);
                }
            }

            foreach (Document d in nullDocs)
            {
                documents.Remove(d);
                // check if the hashes of these are matched in the next index!!!! If so - hook em back up with event history
                // if not, sack em off.
            }

            foreach (DocPath p in nullDocPaths)
            {
                paths.Remove(p);
            }
            fic.SaveChanges();
        }

    }
}

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

            var hashMatch = fic.Documents.Any(i => i.DocumentHash == fileHash);
            var nameMatch = fic.DocPaths.Any(i => i.name == fi.Name);
            var document = new Document();
            var docpath = new DocPath();

            if(hashMatch){
                var matchingDocument = fic.Documents.First(i => i.DocumentHash == fileHash);
                              
                // Hashes match and we have the document.
                // check if it has a matching path - if so, leave well alone.
                if (matchingDocument.DocPaths.Any(i => i.path == fi.FullName))
                {
                    // do nothing - hashes and  paths match
                }
                else
                {
                    // document exists in db (by matching hash) - just add the new path
                    var dp = new DocPath() { path = fi.FullName, name = fi.Name, Document = matchingDocument };
                    fic.DocPaths.Add(dp);
                }
            }
            else // Hashes do not match
            {
                if(nameMatch)
                {
                    // paths match hashes don't - add path to a new document. - MAY NEED TO LOOP THROUGH THEM ALL HERE!!!!!
                    document = new Document() { DocumentHash = fileHash, size = fi.Length }; //  GET THE OLD DOCPATH AND UPDATE IT DON'T JUST CREATE A NEW ONE!
                    docpath = fic.DocPaths.First(i => i.name == fi.Name);
                    docpath.Document = document;
                    docpath.name = fi.Name;
                    docpath.path = fi.FullName;
                    fic.Documents.Add(document);
                }
                else
                {
                    // if nothing matches
                    // new document, new path
                    document.DocumentHash = fileHash;
                    document.size = fi.Length;
                    fic.Documents.Add(document);
                    docpath.path = fi.FullName;
                    docpath.name = fi.Name;
                    docpath.Document = document;
                    fic.DocPaths.Add(docpath);
                    Console.WriteLine("Fresh index of a file " + docpath.path);
                }
            }

            fic.SaveChanges();
            Console.WriteLine("they should be in now");
           
        }

        public void CheckIndex()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            //var fileHash = KernelFactory.Instance.Get<IHash>().HashFile(path);
            var documents = fic.Documents;
            var paths = fic.DocPaths;


            foreach (Document d in documents)
            {
                if (!(d.DocPaths.Any(i => i.DocumentId == d.DocumentId)))
                {
                    //documents.Remove(d);
                    nullDocs.Add(d);
                    Console.WriteLine("removing " + d.DocumentId);
                }
                foreach (DocPath p in d.DocPaths)
                {
                    if (!File.Exists(p.path))
                    {
                        //paths.Remove(p);
                        nullDocPaths.Add(p);
                        Console.WriteLine("removing path " + p.path);
                    }
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

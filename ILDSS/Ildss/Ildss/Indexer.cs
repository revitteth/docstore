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
        
        public void IndexFiles(string path)
        {
            CheckIndex(path);
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

            string indexType = "Indexed";

            // Hash File
            var h = KernelFactory.Instance.Get<IHash>();
            string fileHash = h.HashFile(path);

            var fic = KernelFactory.Instance.Get<FileIndexContext>();

            Document result = new Document();
 
            // If document exists in DB use it, or create new document
            if(fic.Documents.Any(i => i.DocumentHash == fileHash))
            {
                result = fic.Documents.First(i => i.DocumentHash == fileHash);
                // check path is ok
            }
            else
            {
                result = new Document() { DocumentHash = fileHash, size = fi.Length };
                fic.Documents.Add(result);
            }

            DocPath docpath = new DocPath();

            // If path doesn't exist in DB
            if (!fic.DocPaths.Any(i => i.path == path))
            {
                docpath = new DocPath() { path = fi.FullName, Document = result };
                result.DocPaths.Add(docpath);
            }
            else
            {
                // update document.
                docpath = fic.DocPaths.First(i => i.path == path);
                docpath.Document = result;
            }

            var de = new DocEvent()
            {
                event_time = (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond)),
                Document = result,
                last_access = fi.LastAccessTime,
                last_write = fi.LastWriteTime,
                type = indexType
            };
            fic.DocEvents.Add(de);

            fic.SaveChanges();
        }

        public void CheckIndex(string path)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var documents = fic.Documents;
            var paths = fic.DocPaths;
            List<Document> nullDocs = new List<Document>();
            List<DocPath> nullDocPaths = new List<DocPath>();

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

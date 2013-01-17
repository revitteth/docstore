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
            if (System.IO.File.Exists(path))
            {
                IndexFile(path);
            }
            else if (System.IO.Directory.Exists(path))
            {
                IndexDirectory(path);
            }
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
            var lastAccess = fi.LastAccessTime;
            var lastWrite = fi.LastWriteTime;

            // Hash File
            var h = KernelFactory.Instance.Get<IHash>();
            string fileHash = h.HashFile(path);

            var fic = KernelFactory.Instance.Get<FileIndexContext>();

            Document result = new Document();
 
            // If document exists in DB use it, or create new document
            if(fic.Documents.Any(i => i.DocumentHash == fileHash))
            {
                result = fic.Documents.First(i => i.DocumentHash == fileHash);
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

            DocEvent de = new DocEvent()
            {
                name = fi.Name,
                path = fi.FullName,
                type = "Indexed",
                Document = result,
                last_access = lastAccess,
                last_write = lastWrite,
            };
            
            bool unique = true;

            foreach (DocEvent d in fic.DocEvents.Where(i => i.Document.DocumentHash == result.DocumentHash))
            {
                Console.WriteLine("looping");
                if (false)
                {
                    // found it break loop
                    Console.WriteLine("got a matching event");
                    unique = false;
                    break;
                }
                else
                {
                    Console.WriteLine("still unique!");
                    unique = true;
                }
            }
            if (unique)
            {
                fic.DocEvents.Add(de);
                Console.WriteLine("Wrote it!");
            }
            fic.SaveChanges();
        }

        public void RemoveFromIndex(string path)
        {
            // remove this file from the index
        }

    }
}

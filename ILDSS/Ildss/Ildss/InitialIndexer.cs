using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    class InitialIndexer : IIndexer
    {
        private FileInfo fi { get; set; }
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
        }

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
            var fi = new FileInfo(path);
            fi.Refresh();

            // Hash File
            var h = KernelFactory.Instance.Get<IHash>();
            string fileHash = h.HashFile(path);

            // Get DB Context
            var fic = KernelFactory.Instance.Get<FileIndexContext>();

            var newPath = new DocPath() { directory = fi.FullName.Replace(fi.Name, ""), name = fi.Name, path = fi.FullName };

            if (fic.Documents.Any(i => i.DocumentHash == fileHash)) 
            {
                fic.Documents.First(i => i.DocumentHash == fileHash).DocPaths.Add(newPath);
            }
            else
            {
                var newDocument = new Document() { DocumentHash = fileHash, size = fi.Length };
                newDocument.DocPaths.Add(newPath);
                fic.Documents.Add(newDocument);
            }

            fic.SaveChanges();
        }

        public void CheckDatabase(string path, string type, string oldpath = "")
        {
            // nothing
        }

    }
}

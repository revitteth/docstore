using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ildss.Models;
using Ildss.Crypto;

namespace Ildss.Index
{
    class InitialIndexer : IIndexer
    {
        private List<DocPath> _nullDocPaths = new List<DocPath>();
        private List<string> _ignoreFiles = new List<string> { ".tmp", ".TMP", ".gz", ".ini" };

        public void IndexFiles(string path)
        {
            FileInfo fi = new FileInfo(path);
            if (!_ignoreFiles.Any(fi.Name.Contains) & fi.Name.Contains("."))
            {
                Console.WriteLine("ignoreFile found in MAIN INDEXER");
            }

            if (System.IO.File.Exists(path) & !_ignoreFiles.Any(fi.Name.Contains))
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

            if (!_ignoreFiles.Any(fi.Name.Contains) & fi.Name.Contains("."))
            {

                // Hash File
                var h = KernelFactory.Instance.Get<IHash>();
                string fileHash = h.HashFile(path);

                // Get DB Context
                var fic = KernelFactory.Instance.Get<FileIndexContext>();

                var newPath = new DocPath() { directory = fi.FullName.Replace(fi.Name, ""), name = fi.Name, path = fi.FullName };
                //var newEvent = new DocEvent() { type = "Index", time = DateTime.Now };

                if (fic.Documents.Any(i => i.DocumentHash == fileHash))
                {
                    fic.Documents.First(i => i.DocumentHash == fileHash).DocPaths.Add(newPath);
                    //fic.Documents.First(i => i.DocumentHash == fileHash).DocEvents.Add(newEvent);
                }
                else
                {
                    var newDocument = new Document() { DocumentHash = fileHash, size = fi.Length, status = "current" };
                    newDocument.DocPaths.Add(newPath);
                    //newDocument.DocEvents.Add(newEvent);
                    fic.Documents.Add(newDocument);
                }

                fic.SaveChanges();

                // Register last read/write event times
                KernelFactory.Instance.Get<ICollector>().Register(path);
            }
        }

    }
}

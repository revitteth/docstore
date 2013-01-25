using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    class FrequentIndexer : IIndexer
    {
        List<DocPath> missingPaths = new List<DocPath>();

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

        public void CheckDatabase()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var hash = KernelFactory.Instance.Get<IHash>();

            foreach (var p in fic.DocPaths)
            {
                if (File.Exists(p.path))
                {
                    // file there, check hash (incase updated)
                    if (p.Document.DocumentHash != hash.HashFile(p.path))
                    {
                        // hashes are different - update it if only 1 path
                        if (fic.DocPaths.Count(i => i.DocPathId == p.DocPathId) <= 1)
                        {
                            var doc = fic.DocPaths.First(i => i.DocPathId == p.DocPathId);
                            doc.Document.DocumentHash = hash.HashFile(p.path);
                            doc.Document.size = new FileInfo(p.path).Length;
                            //fic.SaveChanges();
                        }
                        else
                        {
                            var fi = new FileInfo(p.path);
                            var doc = new Document() { DocumentHash = hash.HashFile(p.path), size = fi.Length };
                            p.Document = doc;
                            fic.Documents.Add(doc);
                            //fic.SaveChanges();
                        }
                    }
                    // else hashes and path match - do nothing
                }
                else
                {
                    // file not found- could have been moved!
                    missingPaths.Add(p);
                    Console.WriteLine(p.path + " IS MISSING");
                }
            }

            fic.SaveChanges();
        }

        public void IndexFile(string path)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var hash = KernelFactory.Instance.Get<IHash>();
            var fi = new FileInfo(path);

            var fileHash = hash.HashFile(path);

            // find moved, and new files

            if (missingPaths.Any(i => i.name == fi.Name && i.Document.DocumentHash == fileHash))
            {
                // file moved
                // add a new path for it
                // delete old one
                var doc = fic.Documents.First(i => i.DocumentHash == fileHash);
                var docpath = new DocPath() { name = fi.Name, path = fi.FullName, directory = fi.FullName.Replace(fi.Name, "") };
                doc.DocPaths.Add(docpath);

                // if called on every event this is ok may need to change if called less frequently!
                //fic.DocPaths.Remove(missingPaths.All(i => i.name == fi.Name && i.Document.DocumentHash == fileHash));
                missingPaths.Remove(missingPaths.First(i => i.name == fi.Name && i.Document.DocumentHash == fileHash));
            }
            else
            {
                // new file - add it and give it a new document
                var doc = new Document() { DocumentHash = fileHash, size = fi.Length };
                var docpath = new DocPath() { name = fi.Name, path = fi.FullName, directory = fi.FullName.Replace(fi.Name, "") };
                doc.DocPaths.Add(docpath);
                fic.Documents.Add(doc);
                fic.SaveChanges();
            }

        }

    }
}

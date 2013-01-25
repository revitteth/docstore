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


            foreach (var mp in missingPaths)
            {
                fic.DocPaths.Remove(mp);
            }

            fic.SaveChanges();


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
                            Console.WriteLine("different hash (1 path per document)");
                            var docpath = fic.DocPaths.First(i => i.DocPathId == p.DocPathId);
                            docpath.Document.DocumentHash = hash.HashFile(p.path);
                            docpath.Document.size = new FileInfo(p.path).Length;
                        }
                        else
                        {
                            Console.WriteLine("different hash (multiple paths for same document)");
                            var fi = new FileInfo(p.path);
                            var doc = new Document() { DocumentHash = hash.HashFile(p.path), size = fi.Length };
                            p.Document = doc;
                            fic.Documents.Add(doc);
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

            if (File.GetAttributes(path) == FileAttributes.Directory)
            {
                Console.WriteLine("directory...");
                return;
            }

            var fileHash = hash.HashFile(path);

            // find moved, and new files

            if (missingPaths.Any(i => i.name == fi.Name && i.Document.DocumentHash == fileHash))
            {
                Console.WriteLine("moved a file " + fi.Name);
                // file moved
                // add a new path for it
                // delete old one
                var doc = fic.Documents.First(i => i.DocumentHash == fileHash);
                var docpath = new DocPath() { name = fi.Name, path = fi.FullName, directory = fi.FullName.Replace(fi.Name, "") };
                doc.DocPaths.Add(docpath);

                // if called on every event this is ok may need to change if called less frequently!
                fic.DocPaths.Remove(missingPaths.First(i => i.name == fi.Name && i.Document.DocumentHash == fileHash));
                missingPaths.Remove(missingPaths.First(i => i.name == fi.Name && i.Document.DocumentHash == fileHash));
            }
            else
            {
                // IF STATEMENT HERE
                // IF already a document for hash ... assign path to that document
                if (fic.Documents.Any(i => i.DocumentHash == fileHash))
                {
                    var docpath = new DocPath();
                    // Document already in system
                    var doc = fic.Documents.First(i => i.DocumentHash == fileHash);
                    if (!fic.DocPaths.Any(i => i.path == fi.FullName))
                    {
                        docpath = new DocPath() { name = fi.Name, path = fi.FullName, directory = fi.FullName.Replace(fi.Name, "") };
                        doc.DocPaths.Add(docpath);
                    }
                    fic.SaveChanges();
                }
                else
                {
                    Console.WriteLine("new file " + fi.Name);
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
}
